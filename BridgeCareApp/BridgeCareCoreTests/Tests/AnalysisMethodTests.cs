using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.UnitTestsCore;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Interfaces.DefaultData;
using BridgeCareCore.Models.DefaultData;
using BridgeCareCore.Utils;
using BridgeCareCore.Utils.Interfaces;
using BridgeCareCoreTests.Helpers;
using BridgeCareCoreTests.Tests.AnalysisMethod;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

using Policy = BridgeCareCore.Security.SecurityConstants.Policy;

namespace BridgeCareCoreTests.Tests
{
    public class AnalysisMethodTests
    {
        private static readonly Guid BenefitId = Guid.Parse("be2497dd-3acd-4cdd-88a8-adeb9893f1df");
        private readonly Mock<IAnalysisDefaultDataService> _mockAnalysisDefaultDataService = new Mock<IAnalysisDefaultDataService>();
        private readonly Mock<IClaimHelper> _mockClaimHelper = new();

        private AnalysisMethodController SetupController()
        {
            var unitOfWork = TestHelper.UnitOfWork;
            AttributeTestSetup.CreateAttributes(unitOfWork);
            NetworkTestSetup.CreateNetwork(unitOfWork);
            
            _mockAnalysisDefaultDataService.Setup(m => m.GetAnalysisDefaultData()).ReturnsAsync(new AnalysisDefaultData());
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new AnalysisMethodController(EsecSecurityMocks.Admin, unitOfWork,
                hubService, accessor, _mockAnalysisDefaultDataService.Object, _mockClaimHelper.Object);
            return controller;
        }

        private AnalysisMethodController CreateTestController(List<string> userClaims)
        {
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var testUser = ClaimsPrincipals.WithNameClaims(userClaims);
            var controller = new AnalysisMethodController(EsecSecurityMocks.Admin, TestHelper.UnitOfWork,
                hubService, accessor, _mockAnalysisDefaultDataService.Object, _mockClaimHelper.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };
            return controller;
        }

        private AnalysisMethodController CreateController(Mock<IUnitOfWork> mockUnitOfWork)
        {
            var claimHelper = ClaimHelperMocks.New();
            var _ = UserRepositoryMocks.EveryoneExists(mockUnitOfWork);
            var analysisDefaultDataServiceMock = AnalysisDefaultDataServiceMocks.DefaultMock();
            var controller = new AnalysisMethodController(
                EsecSecurityMocks.Admin,
                mockUnitOfWork.Object,
                HubServiceMocks.Default(),
                HttpContextAccessorMocks.Default(),
                analysisDefaultDataServiceMock.Object,
                claimHelper.Object);
            return controller;
        }

        [Fact]
        public async Task Get_RepositoryReturnsAnalysisMethodDto_ControllerReturnsOneToo()
        {
            var analysisMethodId = Guid.NewGuid();
            var simulationId = Guid.NewGuid();
            var dto = AnalysisMethodDtos.Default(analysisMethodId);
            var unitOfWork = UnitOfWorkMocks.New();
            var analysisMethodRepository = AnalysisMethodRepositoryMocks.DefaultMock(unitOfWork);
            analysisMethodRepository.Setup(a => a.GetAnalysisMethod(simulationId)).Returns(dto);
            var controller = CreateController(unitOfWork);
            var result = await controller.AnalysisMethod(simulationId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        // WjTodo wip
        public async Task UpsertAnalysisMethod_RepositoryDoesNotThrow_Ok()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repository = AnalysisMethodRepositoryMocks.DefaultMock(unitOfWork);
            var controller = CreateController(unitOfWork);
            var dto = AnalysisMethodDtos.Default(Guid.NewGuid());

            var result = await controller.UpsertAnalysisMethod(Guid.NewGuid(), dto);

            ActionResultAssertions.Ok(result);
        }

        [Fact]
        public async Task UserIsViewAnalysisMethodAuthorized()
        {
            // Admin authorize
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ViewAnalysisMethod,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.AnalysisMethodViewAnyAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ViewAnalysisMethod);
            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsModifyAnalysisMethodAuthorized()
        {
            // Non-admin authorize
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ModifyAnalysisMethod,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.AnalysisMethodModifyPermittedAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Editor }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ModifyAnalysisMethod);
            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsViewAnalysisMethodAuthorized_B2C()
        {
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ViewAnalysisMethod,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.AnalysisMethodViewAnyAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.B2C, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ViewAnalysisMethod);
            // Assert
            Assert.True(allowed.Succeeded);
        }
    }
}
