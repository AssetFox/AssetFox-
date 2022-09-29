﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Interfaces.DefaultData;
using BridgeCareCore.Models.DefaultData;
using BridgeCareCore.Utils;
using BridgeCareCore.Utils.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

using Policy = BridgeCareCore.Security.SecurityConstants.Policy;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses
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
            List<Claim> claims = new List<Claim>();
            foreach (string claimstr in userClaims)
            {
                Claim claim = new Claim(ClaimTypes.Name, claimstr);
                claims.Add(claim);
            }
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var testUser = new ClaimsPrincipal(new ClaimsIdentity(claims));
            var controller = new AnalysisMethodController(EsecSecurityMocks.Admin, TestHelper.UnitOfWork,
                hubService, accessor, _mockAnalysisDefaultDataService.Object, _mockClaimHelper.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };
            return controller;
        }
        public AnalysisMethodEntity TestAnalysis(Guid simulationId, Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var returnValue = new AnalysisMethodEntity
            {
                Id = resolveId,
                SimulationId = simulationId,
                OptimizationStrategy = OptimizationStrategy.Benefit,
                SpendingStrategy = SpendingStrategy.NoSpending,
                ShouldApplyMultipleFeasibleCosts = false,
                ShouldDeteriorateDuringCashFlow = false,
                ShouldUseExtraFundsAcrossBudgets = false
            };
            return returnValue;
        }

        public BenefitEntity TestBenefit(Guid analysisMethodId, Guid? benefitId = null)
        {
            var resolveId = benefitId ?? Guid.NewGuid();
            var returnValue = new BenefitEntity
            {
                Id = resolveId,
                AnalysisMethodId = analysisMethodId,
                Limit = 1
            };
            return returnValue;
        }

        private AnalysisMethodEntity SetupForGet(Guid simulationId)
        {
            var entity = TestAnalysis(simulationId);

            TestHelper.UnitOfWork.Context.AnalysisMethod.Add(entity);
            TestHelper.UnitOfWork.Context.SaveChanges();
            return entity;
        }

        private CriterionLibraryEntity SetupForUpsert(Guid simulationId)
        {
            SetupForGet(simulationId);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibrary();
            TestHelper.UnitOfWork.Context.CriterionLibrary.Add(criterionLibrary);
            TestHelper.UnitOfWork.Context.SaveChanges();
            return criterionLibrary;
        }

        [Fact]
        public async Task ShouldReturnOkResultOnGet()
        {
            var controller = SetupController();
            // Act
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var result = await controller.AnalysisMethod(simulation.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnPost()
        {
            // Arrange
            var controller = SetupController();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var analysisEntity = TestAnalysis(simulation.Id);
            var attributeEntity = TestHelper.UnitOfWork.Context.Attribute.First();
            var dto = analysisEntity.ToDto();
            var benefit = TestBenefit(analysisEntity.Id);
            benefit.Attribute = attributeEntity;
            dto.Benefit = benefit.ToDto();
            dto.Benefit.Attribute = attributeEntity.Name;

            // Act
            var result =
                await controller.UpsertAnalysisMethod(simulation.Id, dto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldGetAnalysisMethod()
        {
            // Arrange
            var controller = SetupController();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var analysisMethodEntity = SetupForGet(simulation.Id);

            // Act
            var result = await controller.AnalysisMethod(simulation.Id);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dto = (AnalysisMethodDTO)Convert.ChangeType(okObjResult.Value, typeof(AnalysisMethodDTO));

            Assert.Equal(analysisMethodEntity.Id, dto.Id);
        }

        [Fact]
        public async Task ShouldCreateAnalysisMethod()
        {
            // Arrange
            var controller = SetupController();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var getResult = await controller.AnalysisMethod(simulation.Id);
            var analysisMethodDto = (AnalysisMethodDTO)Convert.ChangeType((getResult as OkObjectResult).Value,
                typeof(AnalysisMethodDTO));

            analysisMethodDto.Benefit = new BenefitDTO
            {
                Id = Guid.NewGuid(),
                Limit = 0.0,
                Attribute = TestHelper.UnitOfWork.Context.Attribute.First().Name
            };

            // Act
            await controller.UpsertAnalysisMethod(simulation.Id, analysisMethodDto);

            // Assert
            getResult = await controller.AnalysisMethod(simulation.Id);
            var upsertedAnalysisMethodDto = (AnalysisMethodDTO)Convert.ChangeType((getResult as OkObjectResult).Value,
                typeof(AnalysisMethodDTO));

            Assert.Equal(analysisMethodDto.Id, upsertedAnalysisMethodDto.Id);
            Assert.Equal(analysisMethodDto.Benefit.Id, upsertedAnalysisMethodDto.Benefit.Id);
        }

        [Fact]
        public async Task ShouldUpdateAnalysisMethod()
        {
            // Arrange
            var controller = SetupController();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var criterionLibrary = SetupForUpsert(simulation.Id);
            var getResult = await controller.AnalysisMethod(simulation.Id);
            var dto = (AnalysisMethodDTO)Convert.ChangeType((getResult as OkObjectResult).Value,
                typeof(AnalysisMethodDTO));
            var attributeEntity = TestHelper.UnitOfWork.Context.Attribute.First();
            dto.Attribute = attributeEntity.Name;
            dto.CriterionLibrary = criterionLibrary.ToDto();
            var analysisMethod = TestAnalysis(simulation.Id);
            var benefit = TestBenefit(analysisMethod.Id);
            benefit.Attribute = attributeEntity;
            dto.Benefit = benefit.ToDto();
            dto.Benefit.Attribute = attributeEntity.Name;

            // Act
            await controller.UpsertAnalysisMethod(simulation.Id, dto);

            // Assert
            var analysisMethodDto =
                TestHelper.UnitOfWork.AnalysisMethodRepo.GetAnalysisMethod(simulation.Id);

            Assert.Equal(dto.Id, analysisMethodDto.Id);
            Assert.Equal(dto.Attribute, analysisMethodDto.Attribute);
            Assert.Equal(dto.CriterionLibrary.Id, analysisMethodDto.CriterionLibrary.Id);
            Assert.Equal(dto.Benefit.Id, analysisMethodDto.Benefit.Id);
            Assert.Equal(dto.Benefit.Attribute, analysisMethodDto.Benefit.Attribute);
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
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, BridgeCareCore.Security.SecurityConstants.Role.Administrator));
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
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, BridgeCareCore.Security.SecurityConstants.Role.Editor));
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
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.B2C, BridgeCareCore.Security.SecurityConstants.Role.Administrator));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ViewAnalysisMethod);
            // Assert
            Assert.True(allowed.Succeeded);
        }
    }
}
