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


        private UserDTO AdminUser => new UserDTO
        {
            Username = "Admin",
            HasInventoryAccess = true,
            Id = TestDataForCommittedProjects.AuthorizedUser
        };

        private void Setup()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
        }

        private PerformanceCurveController CreateTestController(List<string> userClaims)
        {
            List<Claim> claims = new List<Claim>();
            foreach (string claimName in userClaims)
            {
                Claim claim = new Claim(ClaimTypes.Name, claimName);
                claims.Add(claim);
            }
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var testUser = new ClaimsPrincipal(new ClaimsIdentity(claims));
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

        public async Task UpsertPerformanceCurveLibrary_Ok()
        {
            Setup();
            // Arrange
            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);
            var libraryId = Guid.NewGuid();
            var library = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibrary(libraryId);
            var request = new LibraryUpsertPagingRequestModel<PerformanceCurveLibraryDTO, PerformanceCurveDTO>()
            {
                Library = library,
                PagingSync = new PagingSyncModel<PerformanceCurveDTO>()
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
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task UpsertPerformanceCurveLibraryPage_Ok()
        {
            Setup();
            // Arrange
            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);
            var libraryId = Guid.NewGuid();
            var library = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibrary(libraryId);
            var request = new LibraryUpsertPagingRequestModel<PerformanceCurveLibraryDTO, PerformanceCurveDTO>()
            {
                Library = library,
                PagingSync = new PagingSyncModel<PerformanceCurveDTO>()
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
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Delete_PerformanceCurveLibraryDoesNotExist_Ok()
        {
            Setup();

            // Arrange
            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);

            // Act
            var result = await controller.DeletePerformanceCurveLibrary(Guid.NewGuid());

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task GetScenarioPerformanceCurves_SimulationExists_Ok()
        {
            Setup();

            // Arrange
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);
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
            var result = await controller.GetScenarioPerformanceCurvePage(simulation.Id, request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task UpsertScenarioPerformanceCurves_SimulationExists_Ok()
        {
            Setup();
            // Arrange
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var performanceCurveId = Guid.NewGuid();
            var controller = PerformanceCurveControllerTestSetup.SetupController(EsecSecurityMocks.Admin);
            var attribute = TestHelper.UnitOfWork.Context.Attribute.First();
            var performanceCurve = ScenarioPerformanceCurveTestSetup.ScenarioEntity(simulation.Id, attribute.Id, performanceCurveId);
            performanceCurve.Attribute = attribute;
            var performanceCurveDto = performanceCurve.ToDto();
            var request = new PagingSyncModel<PerformanceCurveDTO>()
            {
                LibraryId = null,
                AddedRows = new List<PerformanceCurveDTO> { performanceCurveDto },
                RowsForDeletion = new List<Guid>(),
                UpdateRows = new List<PerformanceCurveDTO>()
            };
            // Act
            var result = await controller
                .UpsertScenarioPerformanceCurves(simulation.Id,
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
            // wjwjwj this test
            var repositoryMock = new Mock<IPerformanceCurveRepository>();
            var user = AdminUser;
            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
            var userRepositoryMock = UserRepositoryMocks.EveryoneExists();
            unitOfWork.Setup(u => u.UserRepo).Returns(userRepositoryMock.Object);
            var esecSecurity = EsecSecurityMocks.Admin;
            var pagingService = new Mock<IPerformanceCurvesPagingService>();
            var service = new Mock<IPerformanceCurvesService>();

            var performanceCurves = new List<PerformanceCurveDTO>();
            var libraryId = Guid.NewGuid();
            var pagingSync = new PagingSyncModel<PerformanceCurveDTO>
            {
                LibraryId = libraryId,
            };
            pagingService.Setup(s => s.GetSyncedLibraryDataset(libraryId, pagingSync)).Returns(performanceCurves);
            unitOfWork.Setup(u => u.PerformanceCurveRepo).Returns(repositoryMock.Object);
            var controller = PerformanceCurveControllerTestSetup.Create(
                esecSecurity,
                unitOfWork.Object,
                service.Object,
                pagingService.Object);
            var library = new PerformanceCurveLibraryDTO
            {
                Id = libraryId,
            };
            var pagingRequest = new LibraryUpsertPagingRequestModel<PerformanceCurveLibraryDTO, PerformanceCurveDTO>()
            {
                PagingSync = pagingSync,
                Library = library,
            };

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
