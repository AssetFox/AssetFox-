using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Models;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using BridgeCareCore.Utils;
using BridgeCareCore.Utils.Interfaces;
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

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class PerformanceCurveTests
    {
        private TestHelper _testHelper => TestHelper.Instance;
        private static readonly Mock<IClaimHelper> _mockClaimHelper = new();

        private void Setup()
        {
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.SetupDefaultHttpContext();
        }

        private PerformanceCurveController CreateTestController(List<string> userClaims)
        {
            List<Claim> claims = new List<Claim>();
            foreach (string claimstr in userClaims)
            {
                Claim claim = new Claim(ClaimTypes.Name, claimstr);
                claims.Add(claim);
            }
            var testUser = new ClaimsPrincipal(new ClaimsIdentity(claims));
            var controller = new PerformanceCurveController(
                _testHelper.MockEsecSecurityAdmin.Object,
                _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object,
                _testHelper.MockHttpContextAccessor.Object,
                TestUtils.TestServices.PerformanceCurves,
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
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);

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
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var libraryId = Guid.NewGuid();
            var library = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibrary(libraryId).ToDto();
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

        public async Task UpsertPerformanceCurveLibraryPage_Ok()
        {
            Setup();
            // Arrange
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var libraryId = Guid.NewGuid();
            var library = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibrary(libraryId).ToDto();
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
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);

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
            var simulation = _testHelper.CreateSimulation();
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var request = new PagingRequestModel<PerformanceCurveDTO>()
            {
                isDescending = false,
                Page = 1,
                RowsPerPage = 5,
                PagingSync = new PagingSyncModel<PerformanceCurveDTO>()
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
            var simulation = _testHelper.CreateSimulation();
            var performanceCurveId = Guid.NewGuid();
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var attribute = _testHelper.UnitOfWork.Context.Attribute.First();
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
        public async Task ShouldGetAllPerformanceCurveLibrariesWithPerformanceCurves()
        {
            Setup();
            var libraryId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            // Arrange
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var testLibrary = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(_testHelper.UnitOfWork, libraryId);
            var curve = PerformanceCurveTestSetup.TestPerformanceCurveInDb(_testHelper.UnitOfWork, libraryId, curveId);


            // Act
            var result = await controller.GetPerformanceCurveLibraries();

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<PerformanceCurveLibraryDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(List<PerformanceCurveLibraryDTO>));
            var dto = dtos.Single(pc => pc.Id == libraryId);

            Assert.Single(dto.PerformanceCurves);

            Assert.Equal(curveId, dto.PerformanceCurves[0].Id);
        }

        [Fact]
        public async Task Delete_PerformanceCurveLibraryExists_Deletes()
        {
            Setup();
            // Arrange
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var performanceCurveLibraryId = Guid.NewGuid();
            var performanceCurveId = Guid.NewGuid();
            var testLibrary = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(_testHelper.UnitOfWork, performanceCurveLibraryId);
            var curve = PerformanceCurveTestSetup.TestPerformanceCurveInDb(_testHelper.UnitOfWork, performanceCurveLibraryId, performanceCurveId);
            var curveDto = curve.ToDto();
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(_testHelper.UnitOfWork);
            var getResult = await controller.GetPerformanceCurveLibraries();
            var dtos = (List<PerformanceCurveLibraryDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                typeof(List<PerformanceCurveLibraryDTO>));

            var performanceCurveLibraryDTO = dtos.Single(dto => dto.Id == performanceCurveLibraryId);
            curveDto.CriterionLibrary = criterionLibrary.ToDto();
            var request = new LibraryUpsertPagingRequestModel<PerformanceCurveLibraryDTO, PerformanceCurveDTO>()
            {
                Library = performanceCurveLibraryDTO,
                PagingSync = new PagingSyncModel<PerformanceCurveDTO>()
                {
                    LibraryId = null,
                    UpdateRows = new List<PerformanceCurveDTO> { curveDto },
                    AddedRows = new List<PerformanceCurveDTO>(),
                    RowsForDeletion = new List<Guid>()
                }
            };
            await controller.UpsertPerformanceCurveLibrary(request);

            // Act
            var result = controller.DeletePerformanceCurveLibrary(performanceCurveLibraryId);

            // Assert
            Assert.IsType<OkResult>(result.Result);

            Assert.False(_testHelper.UnitOfWork.Context.PerformanceCurveLibrary.Any(_ => _.Id == performanceCurveLibraryId));
            Assert.False(_testHelper.UnitOfWork.Context.PerformanceCurve.Any(_ => _.Id == performanceCurveId));
            Assert.False(
                _testHelper.UnitOfWork.Context.CriterionLibraryPerformanceCurve.Any(_ =>
                    _.PerformanceCurveId == performanceCurveId));
            Assert.False(
                _testHelper.UnitOfWork.Context.PerformanceCurveEquation.Any(_ =>
                    _.PerformanceCurveId == performanceCurveId));
        }

        [Fact]
        public async Task GetScenarioPerformanceCurves_SimulationInDbWithPerformanceCurve_Gets()
        {
            Setup();
            // Arrange
            var simulation = _testHelper.CreateSimulation();
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var performanceCurveId = Guid.NewGuid();
            ScenarioPerformanceCurveTestSetup.EntityInDb(_testHelper.UnitOfWork, simulation.Id, performanceCurveId);
            var request = new PagingRequestModel<PerformanceCurveDTO>()
            {
                isDescending = false,
                Page = 1,
                PagingSync = new PagingSyncModel<PerformanceCurveDTO>()
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
        public async Task Post_UserIsUnauthorized_ReturnsUnauthorized()
        {
            Setup();
            // Arrange
            var simulation = _testHelper.CreateSimulation();
            _testHelper.SetupDefaultHttpContext();
            var mockedUnauthorized = new Mock<IEsecSecurity>();
            mockedUnauthorized.Setup(_ => _.GetUserInformation(It.IsAny<HttpRequest>()))
                .Returns(new UserInfo
                {
                    Name = "districtEngineer",
                    HasAdminAccess = false,
                    Email = "fake@pa.gov"
                });
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, mockedUnauthorized);
            var performanceCurveId = Guid.NewGuid();
            var attribute = _testHelper.UnitOfWork.Context.Attribute.First();
            var performanceCurve = ScenarioPerformanceCurveTestSetup.ScenarioEntity(simulation.Id, attribute.Id, performanceCurveId);
            performanceCurve.Attribute = attribute;
            var performanceCurveDto = performanceCurve.ToDto();
            var request = new PagingSyncModel<PerformanceCurveDTO>()
            {
                AddedRows = new List<PerformanceCurveDTO> { performanceCurveDto},
                RowsForDeletion = new List<Guid>(),
                UpdateRows = new List<PerformanceCurveDTO>()
            };
            // Act
            var upsertScenarioPerformanceCurveLibraryResult = await controller
                .UpsertScenarioPerformanceCurves(simulation.Id,
                    request);

            // Assert
            Assert.IsType<UnauthorizedResult>(upsertScenarioPerformanceCurveLibraryResult);
        }
        [Fact]
        public async Task UserIsViewPerformanceCurveFromLibraryAuthorized()
        {
            // Non-admin authorize
            // Arrange
            var authorizationService = _testHelper.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ViewPerformanceCurveFromLibrary,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveViewPermittedFromLibraryAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, BridgeCareCore.Security.SecurityConstants.Role.Editor));
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
            var authorizationService = _testHelper.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ModifyPerformanceCurveFromScenario,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveModifyAnyFromScenarioAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, BridgeCareCore.Security.SecurityConstants.Role.Administrator));
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
            var authorizationService = _testHelper.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.DeletePerformanceCurveFromLibrary,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveDeletePermittedFromLibraryAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, BridgeCareCore.Security.SecurityConstants.Role.ReadOnly));
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
            var authorizationService = _testHelper.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ImportPerformanceCurveFromLibrary,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveImportPermittedFromLibraryAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, BridgeCareCore.Security.SecurityConstants.Role.Editor));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ImportPerformanceCurveFromLibrary);
            // Assert
            Assert.True(allowed.Succeeded);
        }
    }
}
