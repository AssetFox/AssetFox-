using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using BridgeCareCore.Services;
using BridgeCareCore.Utils;
using BridgeCareCore.Utils.Interfaces;
using BridgeCareCoreTests.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MoreLinq;
using Xunit;
using static BridgeCareCore.Security.SecurityConstants;
using Assert = Xunit.Assert;
using Claim = System.Security.Claims.Claim;

namespace BridgeCareCoreTests.Tests
{
    public class PerformanceCurveControllerTests
    {
        private static readonly Mock<IClaimHelper> _mockClaimHelper = new();



        private void Setup()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
        }

        private PerformanceCurveController CreateController(Mock<IUnitOfWork> unitOfWork)
        {
            var security = EsecSecurityMocks.AdminMock;
            var hubService = HubServiceMocks.DefaultMock();
            var contextAccessor = HttpContextAccessorMocks.DefaultMock();
            var expressionValidationService = new Mock<IExpressionValidationService>();
            var performanceCurvesService = new PerformanceCurvesService(unitOfWork.Object, hubService.Object, expressionValidationService.Object);
            var performanceCurvesPagingService = new PerformanceCurvesPagingService(unitOfWork.Object);
            var claimHelper = new Mock<IClaimHelper>();
            var controller = new PerformanceCurveController(
                security.Object,
                unitOfWork.Object,
                hubService.Object,
                contextAccessor.Object,
                performanceCurvesService,
                performanceCurvesPagingService,
                claimHelper.Object
                );
            return controller;
        }

        private PerformanceCurveController CreateTestController(List<string> userClaims)
        {
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var testUser = ClaimsPrincipals.WithNameClaims(userClaims);
            var controller = new PerformanceCurveController(
                EsecSecurityMocks.AdminMock.Object,
                TestHelper.UnitOfWork,
                hubService,
                accessor,
                TestServices.PerformanceCurves(TestHelper.UnitOfWork, hubService),
                new PerformanceCurvesPagingService(TestHelper.UnitOfWork),
                _mockClaimHelper.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };
            return controller;
        }

        [Fact]
        public async Task GetPerformanceCurveLibraries_Ok()
        {
            Setup();
            // Arrange
            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);

            // Act
            var result = await controller.GetPerformanceCurveLibraries();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]

        public async Task UpsertPerformanceCurveLibrary_NewLibrary_Ok()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = PerformanceCurveRepositoryMocks.New(unitOfWork);
            var libraryId = Guid.NewGuid();
            var dto = PerformanceCurveLibraryDtos.Empty(libraryId);
            var dto2 = PerformanceCurveLibraryDtos.Empty(libraryId);
            var controller = CreateController(unitOfWork);
            var request = new LibraryUpsertPagingRequestModel<PerformanceCurveLibraryDTO, PerformanceCurveDTO>()
            {
                Library = dto,
                IsNewLibrary = true,
                SyncModel = new PagingSyncModel<PerformanceCurveDTO>()
                {
                    AddedRows = new List<PerformanceCurveDTO>(),
                    RowsForDeletion = new List<Guid>(),
                    UpdateRows = new List<PerformanceCurveDTO>()
                }
            };

            // Act
            var result = await controller
                .UpsertPerformanceCurveLibrary(request);

            // Assert
            ActionResultAssertions.Ok(result);
            var libraryCall = repo.SingleInvocationWithName(nameof(IPerformanceCurveRepository.UpsertPerformanceCurveLibrary));
            ObjectAssertions.Equivalent(dto2, libraryCall.Arguments[0]);
            var curveCall = repo.SingleInvocationWithName(nameof(IPerformanceCurveRepository.UpsertOrDeletePerformanceCurves));
            ObjectAssertions.EmptyEnumerable<PerformanceCurveDTO>(curveCall.Arguments[0]);
            Assert.Equal(libraryId, curveCall.Arguments[1]);
        }

        [Fact]

        public async Task UpsertPerformanceCurveLibrary_NotNewLibrary_Ok()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = PerformanceCurveRepositoryMocks.New(unitOfWork);
            var libraryId = Guid.NewGuid();
            var dto = PerformanceCurveLibraryDtos.Empty(libraryId);
            var dto2 = PerformanceCurveLibraryDtos.Empty(libraryId);
            var dto3 = PerformanceCurveLibraryDtos.Empty(libraryId);
            dto.Name = "Updated name";
            dto2.Name = "Updated name";
            repo.Setup(r => r.GetPerformanceCurveLibrary(libraryId)).Returns(dto3);
            repo.Setup(r => r.GetPerformanceCurvesForLibraryOrderedById(libraryId)).Returns(new List<PerformanceCurveDTO>());
            var controller = CreateController(unitOfWork);
            var request = new LibraryUpsertPagingRequestModel<PerformanceCurveLibraryDTO, PerformanceCurveDTO>()
            {
                Library = dto,
                IsNewLibrary = false,
                SyncModel = new PagingSyncModel<PerformanceCurveDTO>()
                {
                    AddedRows = new List<PerformanceCurveDTO>(),
                    RowsForDeletion = new List<Guid>(),
                    UpdateRows = new List<PerformanceCurveDTO>()
                }
            };

            // Act
            var result = await controller
                .UpsertPerformanceCurveLibrary(request);

            // Assert
            ActionResultAssertions.Ok(result);
            var libraryCall = repo.SingleInvocationWithName(nameof(IPerformanceCurveRepository.UpsertPerformanceCurveLibrary));
            ObjectAssertions.Equivalent(dto2, libraryCall.Arguments[0]);
            var curveCall = repo.SingleInvocationWithName(nameof(IPerformanceCurveRepository.UpsertOrDeletePerformanceCurves));
            ObjectAssertions.EmptyEnumerable<PerformanceCurveDTO>(curveCall.Arguments[0]);
            Assert.Equal(libraryId, curveCall.Arguments[1]);
        }


        [Fact]
        public async Task DeletePerformanceCurveLibrary_PassesRequestToRepo()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = PerformanceCurveRepositoryMocks.New(unitOfWork);
            var libraryId = Guid.NewGuid();
            var controller = CreateController(unitOfWork);
            // Act
            var result = await controller.DeletePerformanceCurveLibrary(libraryId);

            // Assert
            Assert.IsType<OkResult>(result);
            var invocation = repo.SingleInvocationWithName(nameof(IPerformanceCurveRepository.DeletePerformanceCurveLibrary));
            Assert.Equal(libraryId, invocation.Arguments[0]);
        }

        [Fact]
        public async Task GetScenarioPerformanceCurves_SimulationExists_Ok()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = PerformanceCurveRepositoryMocks.New(unitOfWork);
            var libraryId = Guid.NewGuid();
            var controller = CreateController(unitOfWork);
            // Arrange
            var simulationId = Guid.NewGuid();
            repo.Setup(r => r.GetScenarioPerformanceCurvesOrderedById(simulationId)).Returns(new List<PerformanceCurveDTO>());
           
            var request = new PagingRequestModel<PerformanceCurveDTO>()
            {
                isDescending = false,
                Page = 1,
                RowsPerPage = 5,
                SyncModel = new PagingSyncModel<PerformanceCurveDTO>()
                {
                    AddedRows = new List<PerformanceCurveDTO>(),
                    RowsForDeletion = new List<Guid>(),
                    UpdateRows = new List<PerformanceCurveDTO>()
                },
                search = "",
                sortColumn = ""
            };
            // Act
            var result = await controller.GetScenarioPerformanceCurvePage(simulationId, request);

            // Assert
            var value = ActionResultAssertions.OkObject(result);
            var castValue = value as PagingPageModel<PerformanceCurveDTO>;
            Assert.Empty(castValue.Items);
        }

        [Fact]
        public async Task UpsertScenarioPerformanceCurves_SimulationExists_Ok()
        {
            var unitOfWork = UnitOfWorkMocks.EveryoneExists();
            var repo = PerformanceCurveRepositoryMocks.New(unitOfWork);
            var simulationId = Guid.NewGuid();
            var criterionLibraryId = Guid.NewGuid();
            var controller = CreateController(unitOfWork);
            // Arrange
            var performanceCurveId = Guid.NewGuid();
            var dto = PerformanceCurveDtos.Dto(criterionLibraryId, performanceCurveId, "Attribute", "Equation");
            repo.Setup(r => r.GetScenarioPerformanceCurvesOrderedById(simulationId)).Returns(new List<PerformanceCurveDTO>());
            var request = new PagingSyncModel<PerformanceCurveDTO>()
            {
                LibraryId = null,
                AddedRows = new List<PerformanceCurveDTO> { dto },
                RowsForDeletion = new List<Guid>(),
                UpdateRows = new List<PerformanceCurveDTO>()
            };
            // Act
            var result = await controller
                .UpsertScenarioPerformanceCurves(simulationId,
                    request);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact(Skip = "GetPerformanceCurveLibraries no longer gets child performance curves, may want to delete")]
        public async Task ShouldGetAllPerformanceCurveLibrariesWithoutPerformanceCurves()
        {
            Setup();
            var libraryId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            // Arrange
            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);
            var testLibrary = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            var curve = PerformanceCurveTestSetup.TestPerformanceCurveInDb(TestHelper.UnitOfWork, libraryId, curveId, TestAttributeNames.ActionType);

            // Act
            var result = await controller.GetPerformanceCurveLibraries();

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<PerformanceCurveLibraryDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(List<PerformanceCurveLibraryDTO>));
            var dto = dtos.Single(pc => pc.Id == libraryId);

            Assert.Empty(dto.PerformanceCurves);
        }

        [Fact]
        public async Task Upsert_AsksRepositoryToUpsertCurvesAndLibrary()
        {
            var repositoryMock = new Mock<IPerformanceCurveRepository>();
            var user = UserDtos.Admin();
            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
            var userRepositoryMock = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var esecSecurity = EsecSecurityMocks.Admin;
            var pagingService = new Mock<IPerformanceCurvesPagingService>();
            var service = new Mock<IPerformanceCurvesService>();

            var performanceCurves = new List<PerformanceCurveDTO>();
            var libraryId = Guid.NewGuid();
            var pagingSync = new PagingSyncModel<PerformanceCurveDTO>
            {
                LibraryId = libraryId,
            };
            var library = new PerformanceCurveLibraryDTO
            {
                Id = libraryId,
            };
            var pagingRequest = new LibraryUpsertPagingRequestModel<PerformanceCurveLibraryDTO, PerformanceCurveDTO>()
            {
                SyncModel = pagingSync,
                Library = library,
            };
            pagingService.Setup(s => s.GetSyncedLibraryDataset(pagingRequest)).Returns(performanceCurves);
            unitOfWork.Setup(u => u.PerformanceCurveRepo).Returns(repositoryMock.Object);
            var controller = PerformanceCurveControllerTestSetup.Create(
                esecSecurity,
                unitOfWork.Object,
                service.Object,
                pagingService.Object);
            

            await controller.UpsertPerformanceCurveLibrary(pagingRequest);

            var libraryCalls = repositoryMock.Invocations.Where(i => i.Method.Name == nameof(IPerformanceCurveRepository.UpsertPerformanceCurveLibrary));
            Assert.Single(libraryCalls);
            var curveCalls = repositoryMock.Invocations.Where(i => i.Method.Name == nameof(IPerformanceCurveRepository.UpsertOrDeletePerformanceCurves));
            Assert.Single(curveCalls);
        }

        [Fact]
        public async Task GetScenarioPerformanceCurves_SimulationInDbWithPerformanceCurve_Gets()
        {
            Setup();
            // Arrange
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);
            var performanceCurveId = Guid.NewGuid();
            ScenarioPerformanceCurveTestSetup.DtoForEntityInDb(TestHelper.UnitOfWork, simulation.Id, performanceCurveId);
            var request = new PagingRequestModel<PerformanceCurveDTO>()
            {
                isDescending = false,
                Page = 1,
                SyncModel = new PagingSyncModel<PerformanceCurveDTO>()
                {
                    AddedRows = new List<PerformanceCurveDTO>(),
                    RowsForDeletion = new List<Guid>(),
                    UpdateRows = new List<PerformanceCurveDTO>()
                },
                RowsPerPage = 5,
                search = "",
                sortColumn = ""
            };
            // Act
            var result = await controller.GetScenarioPerformanceCurvePage(simulation.Id, request);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var page = (PagingPageModel<PerformanceCurveDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(PagingPageModel<PerformanceCurveDTO>));
            Assert.Single(page.Items);
            Assert.Equal(performanceCurveId, page.Items[0].Id);
        }

        [Fact]
        public async Task UserIsViewPerformanceCurveFromLibraryAuthorized()
        {
            // Non-admin authorize
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ViewPerformanceCurveFromLibrary,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveViewPermittedFromLibraryAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Editor }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ViewPerformanceCurveFromLibrary);
            // Assert
            Assert.True(allowed.Succeeded);
        }

        [Fact]
        public async Task UserIsModifyPerformanceCurveFromScenarioAuthorized()
        {
            // Admin authorize
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ModifyPerformanceCurveFromScenario,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveModifyAnyFromScenarioAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ModifyPerformanceCurveFromScenario);
            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsDeletePerformanceCurveFromLibraryAuthorized()
        {
            // Non-admin unauthorize
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.DeletePerformanceCurveFromLibrary,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveDeletePermittedFromLibraryAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.ReadOnly }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.DeletePerformanceCurveFromLibrary);
            // Assert
            Assert.False(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsImportPerformanceCurveFromLibraryAuthorized()
        {
            // Non-admin authorize
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ImportPerformanceCurveFromLibrary,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveImportPermittedFromLibraryAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Editor }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ImportPerformanceCurveFromLibrary);
            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsViewPerformanceCurveFromLibraryAuthorized_B2C()
        {
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ViewPerformanceCurveFromLibrary,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveViewAnyFromLibraryAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveViewPermittedFromLibraryAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.B2C, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ViewPerformanceCurveFromLibrary);
            // Assert
            Assert.True(allowed.Succeeded);
        }
    }
}
