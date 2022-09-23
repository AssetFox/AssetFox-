using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Models;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
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

        private void Setup()
        {
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.SetupDefaultHttpContext();
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

            // Act
            var result = await controller
                .UpsertPerformanceCurveLibrary(PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibrary(libraryId).ToDto());

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

            // Act
            var result = await controller.GetScenarioPerformanceCurves(simulation.Id);

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

            // Act
            var result = await controller
                .UpsertScenarioPerformanceCurves(simulation.Id,
                    new List<PerformanceCurveDTO> { performanceCurveDto });

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
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
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(_testHelper.UnitOfWork);
            var getResult = await controller.GetPerformanceCurveLibraries();
            var dtos = (List<PerformanceCurveLibraryDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                typeof(List<PerformanceCurveLibraryDTO>));

            var performanceCurveLibraryDTO = dtos.Single(dto => dto.Id == performanceCurveLibraryId);
            performanceCurveLibraryDTO.PerformanceCurves[0].CriterionLibrary =
                criterionLibrary.ToDto();

            await controller.UpsertPerformanceCurveLibrary(performanceCurveLibraryDTO);

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

            // Act
            var result = await controller.GetScenarioPerformanceCurves(simulation.Id);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<PerformanceCurveDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(List<PerformanceCurveDTO>));
            Assert.Single(dtos);
            Assert.Equal(performanceCurveId, dtos[0].Id);
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
                    HasAdminClaim = true,
//                    Role = Role.DistrictEngineer,
                    Email = "fake@pa.gov"
                });
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, mockedUnauthorized);
            var performanceCurveId = Guid.NewGuid();
            var attribute = _testHelper.UnitOfWork.Context.Attribute.First();
            var performanceCurve = ScenarioPerformanceCurveTestSetup.ScenarioEntity(simulation.Id, attribute.Id, performanceCurveId);
            performanceCurve.Attribute = attribute;
            var performanceCurveDto = performanceCurve.ToDto();

            // Act
            var upsertScenarioPerformanceCurveLibraryResult = await controller
                .UpsertScenarioPerformanceCurves(simulation.Id,
                    new List<PerformanceCurveDTO> { performanceCurveDto });

            // Assert
            Assert.IsType<UnauthorizedResult>(upsertScenarioPerformanceCurveLibraryResult);
        }
        //private IAuthorizationService BuildAuthorizationService(Action<IServiceCollection> setupServices = null)
        //{
        //    var services = new ServiceCollection();
        //    services.AddAuthorizationCore();
        //    services.AddLogging();
        //    services.AddOptions();
        //    setupServices?.Invoke(services);
        //    return services.BuildServiceProvider().GetRequiredService<IAuthorizationService>();
        //}

        [Fact]
        public async Task UserIsViewPerformanceCurveFromLibraryAuthorized()
        {
            // Arrange
            var authorizationService = _testHelper.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ViewPerformanceCurveFromLibrary,
                        policy => policy.RequireClaim(BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveViewAnyFromLibraryAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveViewPermittedFromLibraryAccess));
                });
            });
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] {
                    new Claim(BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveViewAnyFromLibraryAccess,
                              BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveViewPermittedFromLibraryAccess )
                }));

            // Act
            var allowed = await authorizationService.AuthorizeAsync(user, Policy.ViewPerformanceCurveFromLibrary);

            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsViewPerformanceCurveFromScenarioAuthorized()
        {
            // Arrange
            var authorizationService = _testHelper.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ViewPerformanceCurveFromScenario,
                        policy => policy.RequireClaim(BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveViewAnyFromScenarioAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveViewPermittedFromScenarioAccess));
                });
            });
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] {
                    new Claim(BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveViewAnyFromScenarioAccess,
                              BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveViewPermittedFromScenarioAccess )
                }));

            // Act
            var allowed = await authorizationService.AuthorizeAsync(user, Policy.ViewPerformanceCurveFromScenario);

            // Assert
            Assert.True(allowed.Succeeded);
        }

        [Fact]
        public async Task UserIsModifyPerformanceCurveFromScenarioAuthorized()
        {
            // Arrange
            var authorizationService = _testHelper.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ModifyPerformanceCurveFromScenario,
                        policy => policy.RequireClaim(BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveModifyAnyFromScenarioAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveModifyPermittedFromScenarioAccess));
                });
            });
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] {
                    new Claim(BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveViewAnyFromLibraryAccess,
                              BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveViewPermittedFromLibraryAccess )
                }));

            // Act
            var allowed = await authorizationService.AuthorizeAsync(user, Policy.ModifyPerformanceCurveFromScenario);

            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsModifyPerformanceCurveFromLibraryAuthorized()
        {
            // Arrange
            var authorizationService = _testHelper.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ModifyPerformanceCurveFromLibrary,
                        policy => policy.RequireClaim(BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveUpdateAnyFromLibraryAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveUpdatePermittedFromLibraryAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveViewPermittedFromLibraryAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveAddAnyFromLibraryAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveAddPermittedFromLibraryAccess));
                });
            });
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] {
                    new Claim(BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveUpdateAnyFromLibraryAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveUpdatePermittedFromLibraryAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveViewPermittedFromLibraryAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveAddAnyFromLibraryAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveAddPermittedFromLibraryAccess )
                }));

            // Act
            var allowed = await authorizationService.AuthorizeAsync(user, Policy.ModifyPerformanceCurveFromLibrary);

            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsDeleteFromLibraryAuthorized()
        {
            // Arrange
            var authorizationService = _testHelper.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.DeletePerformanceCurveFromLibrary,
                        policy => policy.RequireClaim(BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveDeleteAnyFromLibraryAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveDeletePermittedFromLibraryAccess));
                });
            });
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] {
                    new Claim(BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveDeletePermittedFromLibraryAccess, BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveDeleteAnyFromLibraryAccess )
                }));

            // Act
            var allowed = await authorizationService.AuthorizeAsync(user, Policy.DeletePerformanceCurveFromLibrary);

            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsImportFromLibraryAuthorized()
        {
            // Arrange
            var authorizationService = _testHelper.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ImportPerformanceCurveFromLibrary,
                        policy => policy.RequireClaim(BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveImportAnyFromLibraryAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveImportPermittedFromLibraryAccess));
                });
            });
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] {
                    new Claim(BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveImportAnyFromLibraryAccess,
                              BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveImportPermittedFromLibraryAccess )
                }));

            // Act
            var allowed = await authorizationService.AuthorizeAsync(user, Policy.ImportPerformanceCurveFromLibrary);

            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsImportFromScenarioAuthorized()
        {
            // Arrange
            var authorizationService = _testHelper.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ImportPerformanceCurveFromScenario,
                        policy => policy.RequireClaim(BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveImportAnyFromScenarioAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveImportPermittedFromScenarioAccess));
                });
            });
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] {
                    new Claim(BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveImportAnyFromScenarioAccess,
                              BridgeCareCore.Security.SecurityConstants.Claim.PerformanceCurveImportPermittedFromScenarioAccess )
                }));

            // Act
            var allowed = await authorizationService.AuthorizeAsync(user, Policy.ImportPerformanceCurveFromScenario);

            // Assert
            Assert.True(allowed.Succeeded);
        }
    }
}
