using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CashFlow;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CashFlow;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCore.Utils;
using BridgeCareCore.Utils.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

using Policy = BridgeCareCore.Security.SecurityConstants.Policy;

namespace BridgeCareCoreTests.Tests
{
    public class CashFlowRuleTests
    {
        private CashFlowController _controller;

        private CashFlowRuleLibraryEntity _testCashFlowRuleLibrary;
        private CashFlowRuleEntity _testCashFlowRule;
        private CashFlowDistributionRuleEntity _testCashFlowDistributionRule;
        private ScenarioCashFlowRuleEntity _testScenarioCashFlowRule;
        private ScenarioCashFlowDistributionRuleEntity _testScenarioCashFlowDistributionRule;
        private readonly Mock<IClaimHelper> _mockClaimHelper = new();

        private void Setup()
        {
            var unitOfWork = TestHelper.UnitOfWork;
            AttributeTestSetup.CreateAttributes(unitOfWork);
            NetworkTestSetup.CreateNetwork(unitOfWork);
        }
        private CashFlowController CreateTestController(List<string> userClaims)
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
            var controller = new CashFlowController(EsecSecurityMocks.AdminMock.Object, TestHelper.UnitOfWork,
                hubService, accessor, _mockClaimHelper.Object, new CashFlowPagingService(TestHelper.UnitOfWork));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };
            return controller;
        }
        private void CreateAuthorizedController()
        {
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            _controller = new CashFlowController(EsecSecurityMocks.AdminMock.Object, TestHelper.UnitOfWork,
                hubService, accessor, _mockClaimHelper.Object, new CashFlowPagingService(TestHelper.UnitOfWork));
        }

        private void CreateUnauthorizedController()
        {
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            _controller = new CashFlowController(EsecSecurityMocks.DbeMock.Object, TestHelper.UnitOfWork,
                hubService, accessor, _mockClaimHelper.Object, new CashFlowPagingService(TestHelper.UnitOfWork));
        }

        private void CreateLibraryTestData()
        {
            _testCashFlowRuleLibrary = new CashFlowRuleLibraryEntity
            {
                Id = Guid.NewGuid(),
                Name = "TestCashFlowRuleLibrary",
                Description = ""
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testCashFlowRuleLibrary);


            _testCashFlowRule = new CashFlowRuleEntity
            {
                Id = Guid.NewGuid(),
                CashFlowRuleLibraryId = _testCashFlowRuleLibrary.Id,
                Name = "TestCashFlowRule",
                CriterionLibraryCashFlowRuleJoin = new CriterionLibraryCashFlowRuleEntity
                {
                    CriterionLibrary = new CriterionLibraryEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = "TestCriterionLibrary",
                        Description = "",
                        MergedCriteriaExpression = "",
                        IsSingleUse = true
                    }
                }
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testCashFlowRule);


            _testCashFlowDistributionRule = new CashFlowDistributionRuleEntity
            {
                Id = Guid.NewGuid(),
                CashFlowRuleId = _testCashFlowRule.Id,
                DurationInYears = 1,
                CostCeiling = 500000,
                YearlyPercentages = "100"
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testCashFlowDistributionRule);
        }

        private void CreateScenarioTestData(Guid simulationId)
        {
            _testScenarioCashFlowRule = new ScenarioCashFlowRuleEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                Name = "TestCashFlowRule",
                CriterionLibraryScenarioCashFlowRuleJoin = new CriterionLibraryScenarioCashFlowRuleEntity
                {
                    CriterionLibrary = new CriterionLibraryEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = "TestCriterionLibrary",
                        Description = "",
                        MergedCriteriaExpression = "",
                        IsSingleUse = true
                    }
                }
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testScenarioCashFlowRule);


            _testScenarioCashFlowDistributionRule = new ScenarioCashFlowDistributionRuleEntity
            {
                Id = Guid.NewGuid(),
                ScenarioCashFlowRuleId = _testScenarioCashFlowRule.Id,
                DurationInYears = 1,
                CostCeiling = 500000,
                YearlyPercentages = "100"
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testScenarioCashFlowDistributionRule);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnLibraryGet()
        {
            // Arrange
            Setup();
            CreateAuthorizedController();

            // Act
            var result = await _controller.GetCashFlowRuleLibraries();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldGetLibraryTargetConditionGoalPageData()
        {
            Setup();
            CreateAuthorizedController();
            // Arrange
            CreateLibraryTestData();

            // Act
            var request = new PagingRequestModel<CashFlowRuleDTO>();
            var result = await _controller.GetLibraryCashFlowRulePage(_testCashFlowRuleLibrary.Id, request);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var page = (PagingPageModel<CashFlowRuleDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(PagingPageModel<CashFlowRuleDTO>));
            var dtos = page.Items;
            var dto = dtos.Single();
            Assert.Equal(_testCashFlowRule.Id, dto.Id);
        }

        [Fact]
        public async Task ShouldGetScenarioTargetConditionGoalPageData()
        {
            Setup();
            CreateAuthorizedController();
            // Arrange
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            CreateScenarioTestData(simulation.Id);

            // Act
            var request = new PagingRequestModel<CashFlowRuleDTO>();
            var result = await _controller.GetScenarioCashFlowRulePage(simulation.Id, request);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var page = (PagingPageModel<CashFlowRuleDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(PagingPageModel<CashFlowRuleDTO>));
            var dtos = page.Items;
            var dto = dtos.Single();
            Assert.Equal(_testScenarioCashFlowRule.Id, dto.Id);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnScenarioGet()
        {
            // Arrange
            Setup();
            CreateAuthorizedController();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);

            // Act
            var result = await _controller.GetScenarioCashFlowRules(simulation.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnLibraryPost()
        {
            // Arrange
            Setup();
            CreateAuthorizedController();
            var dto = new CashFlowRuleLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                CashFlowRules = new List<CashFlowRuleDTO>()
            };

            var libraryRequest = new LibraryUpsertPagingRequestModel<CashFlowRuleLibraryDTO, CashFlowRuleDTO>()
            {
                IsNewLibrary = true,
                Library = dto,
            };

            // Act
            var result = await _controller.UpsertCashFlowRuleLibrary(libraryRequest);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnScenarioPost()
        {
            // Arrange
            Setup();
            CreateAuthorizedController();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var dtos = new List<CashFlowRuleDTO>();
            var sync = new PagingSyncModel<CashFlowRuleDTO>();
            // Act
            var result = await _controller.UpsertScenarioCashFlowRules(simulation.Id, sync);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnDelete()
        {
            // Arrange
            Setup();
            CreateAuthorizedController();

            // Act
            var result = await _controller.DeleteCashFlowRuleLibrary(Guid.Empty);

            // Assert
            Assert.IsType<OkResult>(result);
        }


        [Fact]
        public async Task ShouldGetLibraryNoChildData()
        {
            // Arrange
            Setup();
            CreateAuthorizedController();
            CreateLibraryTestData();

            // Act
            var result = await _controller.GetCashFlowRuleLibraries();

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<CashFlowRuleLibraryDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(List<CashFlowRuleLibraryDTO>));
            var relevantDto = dtos.Single(dto => dto.Id == _testCashFlowRuleLibrary.Id);

            Assert.NotNull(relevantDto);
            Assert.Empty(relevantDto.CashFlowRules);
        }

        [Fact]
        public async Task ShouldGetScenarioData()
        {
            // Arrange
            Setup();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            CreateAuthorizedController();
            CreateScenarioTestData(simulation.Id);

            // Act
            var result = await _controller.GetScenarioCashFlowRules(simulation.Id);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<CashFlowRuleDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(List<CashFlowRuleDTO>));
            Assert.Single(dtos);

            Assert.Equal(_testScenarioCashFlowRule.Id, dtos[0].Id);
            Assert.Equal(_testScenarioCashFlowRule.CriterionLibraryScenarioCashFlowRuleJoin.CriterionLibrary.Id,
                dtos[0].CriterionLibrary.Id);
            Assert.Single(dtos[0].CashFlowDistributionRules);

            Assert.Equal(_testScenarioCashFlowDistributionRule.Id,
                dtos[0].CashFlowDistributionRules[0].Id);
        }

        [Fact]
        public async Task ShouldModifyLibraryData()
        {
            // Arrange
            Setup();
            CreateAuthorizedController();
            CreateLibraryTestData();

            _testCashFlowRule.CashFlowDistributionRules.Add(_testCashFlowDistributionRule);
            _testCashFlowRuleLibrary.CashFlowRules.Add(_testCashFlowRule);

            var dto = _testCashFlowRuleLibrary.ToDto();
            dto.Description = "Updated Description";
            dto.CashFlowRules[0].Name = "Updated Name";
            dto.CashFlowRules[0].CriterionLibrary.MergedCriteriaExpression = "Updated Expression";
            dto.CashFlowRules[0].CashFlowDistributionRules[0].DurationInYears = 2;

            var sync = new PagingSyncModel<CashFlowRuleDTO>()
            {
                UpdateRows = new List<CashFlowRuleDTO>() { dto.CashFlowRules[0] }
            };

            var libraryRequest = new LibraryUpsertPagingRequestModel<CashFlowRuleLibraryDTO, CashFlowRuleDTO>()
            {
                IsNewLibrary = false,
                Library = dto,
                SyncModel = sync
            };

            // Act
            await _controller.UpsertCashFlowRuleLibrary(libraryRequest);

            // Assert
            var modifiedDto = TestHelper.UnitOfWork.CashFlowRuleRepo.GetCashFlowRuleLibraries().Single(lib => lib.Id == dto.Id);
        }

        [Fact]
        public async Task ShouldModifyScenarioData()
        {
            // Arrange
            Setup();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            CreateAuthorizedController();
            CreateScenarioTestData(simulation.Id);

            _testScenarioCashFlowRule.ScenarioCashFlowDistributionRules.Add(_testScenarioCashFlowDistributionRule);

            var dtos = new List<CashFlowRuleDTO> { _testScenarioCashFlowRule.ToDto() };
            dtos[0].Name = "Updated Name";
            dtos[0].CriterionLibrary.MergedCriteriaExpression = "Updated Expression";
            dtos[0].CashFlowDistributionRules[0].DurationInYears = 2;

            var sync = new PagingSyncModel<CashFlowRuleDTO>()
            {
                UpdateRows = new List<CashFlowRuleDTO>() { dtos[0] }
            };

            // Act
            await _controller.UpsertScenarioCashFlowRules(simulation.Id, sync);

            // Assert
            var modifiedDtos = TestHelper.UnitOfWork.CashFlowRuleRepo
                .GetScenarioCashFlowRules(simulation.Id);
            Assert.Single(modifiedDtos);

            Assert.Equal(dtos[0].Name, modifiedDtos[0].Name);
            Assert.Equal(dtos[0].CriterionLibrary.MergedCriteriaExpression,
                modifiedDtos[0].CriterionLibrary.MergedCriteriaExpression);

            Assert.Equal(dtos[0].CashFlowDistributionRules[0].DurationInYears,
                modifiedDtos[0].CashFlowDistributionRules[0].DurationInYears);
        }

        [Fact]
        public async Task ShouldDeleteLibraryData()
        {
            // Arrange
            Setup();
            CreateAuthorizedController();
            CreateLibraryTestData();

            // Act
            var result = await _controller.DeleteCashFlowRuleLibrary(_testCashFlowRuleLibrary.Id);

            // Assert
            Assert.IsType<OkResult>(result);

            Assert.True(
                !TestHelper.UnitOfWork.Context.CashFlowRuleLibrary.Any(_ => _.Id == _testCashFlowRuleLibrary.Id));
            Assert.True(!TestHelper.UnitOfWork.Context.CashFlowRule.Any(_ => _.Id == _testCashFlowRule.Id));
            Assert.True(
                !TestHelper.UnitOfWork.Context.CriterionLibraryCashFlowRule.Any(_ =>
                    _.CashFlowRuleId == _testCashFlowRule.Id));
            Assert.True(
                !TestHelper.UnitOfWork.Context.CashFlowDistributionRule.Any(_ =>
                    _.Id == _testCashFlowDistributionRule.Id));
        }

        [Fact]
        public async Task UserIsViewCashFlowFromLibraryAuthorized()
        {
            // non-admin authorize test
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ViewCashFlowFromLibrary,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.CashFlowViewAnyFromLibraryAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.CashFlowViewPermittedFromLibraryAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Editor }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ViewCashFlowFromLibrary);
            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsModifyCashFlowFromScenarioAuthorized()
        {
            // admin authorize test
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ModifyCashFlowFromScenario,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.CashFlowModifyPermittedFromScenarioAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.CashFlowModifyAnyFromScenarioAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ModifyCashFlowFromScenario);
            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsModifyCashFlowFromLibraryAuthorized()
        {
            // non-admin unauthorized test
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ModifyCashFlowFromLibrary,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.CashFlowModifyAnyFromLibraryAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Editor }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ModifyCashFlowFromLibrary);
            // Assert
            Assert.False(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsViewCashFlowFromLibraryAuthorized_B2C()
        {
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ViewCashFlowFromLibrary,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.CashFlowViewAnyFromLibraryAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.CashFlowViewPermittedFromLibraryAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.B2C, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ViewCashFlowFromLibrary);
            // Assert
            Assert.True(allowed.Succeeded);
        }
    }
}
