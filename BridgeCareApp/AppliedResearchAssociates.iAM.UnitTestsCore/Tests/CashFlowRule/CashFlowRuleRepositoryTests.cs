using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CashFlow;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CashFlow;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class CashFlowRuleRepositoryTests
    {
        private CashFlowRuleLibraryEntity _testCashFlowRuleLibrary;
        private CashFlowRuleEntity _testCashFlowRule;
        private CashFlowDistributionRuleEntity _testCashFlowDistributionRule;
        private ScenarioCashFlowRuleEntity _testScenarioCashFlowRule;
        private ScenarioCashFlowDistributionRuleEntity _testScenarioCashFlowDistributionRule;

        private void Setup()
        {
            var unitOfWork = TestHelper.UnitOfWork;
            AttributeTestSetup.CreateAttributes(unitOfWork);
            NetworkTestSetup.CreateNetwork(unitOfWork);
        }

        private void CreateLibraryTestData()
        {
            _testCashFlowRuleLibrary = new CashFlowRuleLibraryEntity
            {
                Id = Guid.NewGuid(),
                Name = "TestCashFlowRuleLibrary",
                Description = "",
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
        public void GetLibraries_Does()
        {
            // Act and assert
            TestHelper.UnitOfWork.CashFlowRuleRepo.GetCashFlowRuleLibrariesNoChildren();
        }

        [Fact]
        public void UpsertLibrary_Does()
        {
            // Arrange
            Setup();
            var dto = new CashFlowRuleLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "library",
                CashFlowRules = new List<CashFlowRuleDTO>()
            };

            // Act
            TestHelper.UnitOfWork.CashFlowRuleRepo.UpsertCashFlowRuleLibrary(dto);

            // Assert
            var libraryAfter = TestHelper.UnitOfWork.CashFlowRuleRepo.GetCashFlowRuleLibrariesNoChildren()
                .Single(lib => lib.Id == dto.Id);
            ObjectAssertions.Equivalent(dto, libraryAfter);
        }

        [Fact]
        public void UpsertScenarioCashFlowRules_SimulationInDb_Does()
        {
            // Arrange
            Setup();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var dtos = new List<CashFlowRuleDTO>();
            // Act
            TestHelper.UnitOfWork.CashFlowRuleRepo.UpsertOrDeleteScenarioCashFlowRules(dtos, simulation.Id);

            // Assert
            var dtosAfter = TestHelper.UnitOfWork.CashFlowRuleRepo.GetScenarioCashFlowRules(simulation.Id);
            Assert.Empty(dtosAfter);
        }

        [Fact]
        public void DeleteCashFlowRuleLibrary_LibaryDoesNotExist_DoesNotThrow()
        {
            // Act and assert
            TestHelper.UnitOfWork.CashFlowRuleRepo.DeleteCashFlowRuleLibrary(Guid.NewGuid());
        }

        [Fact]
        public void GetCashFlowRuleLibrariesNoChildren_DoesNotGetChildren()
        {
            // Arrange
            Setup();
            CreateLibraryTestData();

            // Act
            var dtos = TestHelper.UnitOfWork.CashFlowRuleRepo.GetCashFlowRuleLibrariesNoChildren();

            var relevantDto = dtos.Single(dto => dto.Id == _testCashFlowRuleLibrary.Id);
            Assert.NotNull(relevantDto);
            Assert.Empty(relevantDto.CashFlowRules);
        }

        [Fact]
        public void GetScenarioCashFlowRules_SimulationInDb_Gets()
        {
            // Arrange
            Setup();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            CreateScenarioTestData(simulation.Id);

            // Act
            var dtos = TestHelper.UnitOfWork.CashFlowRuleRepo.GetScenarioCashFlowRules(simulation.Id);

            // Assert
            var dto = dtos.Single();
            Assert.Equal(_testScenarioCashFlowRule.Id, dto.Id);
            Assert.Equal(_testScenarioCashFlowRule.CriterionLibraryScenarioCashFlowRuleJoin.CriterionLibrary.Id,
                dto.CriterionLibrary.Id);
            Assert.Single(dto.CashFlowDistributionRules);
            Assert.Equal(_testScenarioCashFlowDistributionRule.Id,
                dto.CashFlowDistributionRules[0].Id);
        }

        [Fact]
        public void UpsertCashFlowRuleLibrary_LibraryInDb_Modifies()
        {
            // Arrange
            Setup();
            CreateLibraryTestData();

            var dto = _testCashFlowRuleLibrary.ToDto();
            dto.CashFlowRules[0].CriterionLibrary.MergedCriteriaExpression = null;
            dto.Description = "Updated Description";

            // Act
            TestHelper.UnitOfWork.CashFlowRuleRepo.UpsertCashFlowRuleLibrary(dto);

            // Assert
            var modifiedDto = TestHelper.UnitOfWork.CashFlowRuleRepo.GetCashFlowRuleLibraries().Single(lib => lib.Id == dto.Id);
            ObjectAssertions.EquivalentExcluding(dto, modifiedDto, x => x.CashFlowRules[0].CriterionLibrary);
        }

        [Fact]
        public void UpsertOrDeleteCashFlowRules_RuleInDb_Modifies()
        {
            // Arrange
            Setup();
            CreateLibraryTestData();

            _testCashFlowRule.CashFlowDistributionRules.Add(_testCashFlowDistributionRule);
            _testCashFlowRuleLibrary.CashFlowRules.Add(_testCashFlowRule);
            
            var dto = _testCashFlowRuleLibrary.ToDto();
            var rule = dto.CashFlowRules[0];
            rule.CriterionLibrary.MergedCriteriaExpression = null;
            rule.Name = "Updated Name";
            rule.CriterionLibrary.MergedCriteriaExpression = "Updated Expression";
            rule.CashFlowDistributionRules[0].DurationInYears = 2;

            // Act
            TestHelper.UnitOfWork.CashFlowRuleRepo.UpsertOrDeleteCashFlowRules(dto.CashFlowRules, dto.Id);

            // Assert
            var modifiedDto = TestHelper.UnitOfWork.CashFlowRuleRepo.GetCashFlowRuleLibraries().Single(lib => lib.Id == dto.Id);
            ObjectAssertions.EquivalentExcluding(dto, modifiedDto, x => x.CashFlowRules[0].CriterionLibrary);
        }

        //[Fact]
        //public async Task ShouldModifyScenarioData()
        //{
        //    // Arrange
        //    Setup();
        //    var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
        //    CreateAuthorizedController();
        //    CreateScenarioTestData(simulation.Id);

        //    _testScenarioCashFlowRule.ScenarioCashFlowDistributionRules.Add(_testScenarioCashFlowDistributionRule);

        //    var dtos = new List<CashFlowRuleDTO> { _testScenarioCashFlowRule.ToDto() };
        //    dtos[0].Name = "Updated Name";
        //    dtos[0].CriterionLibrary.MergedCriteriaExpression = "Updated Expression";
        //    dtos[0].CashFlowDistributionRules[0].DurationInYears = 2;

        //    var sync = new PagingSyncModel<CashFlowRuleDTO>()
        //    {
        //        UpdateRows = new List<CashFlowRuleDTO>() { dtos[0] }
        //    };

        //    // Act
        //    await _controller.UpsertScenarioCashFlowRules(simulation.Id, sync);

        //    // Assert
        //    var modifiedDtos = TestHelper.UnitOfWork.CashFlowRuleRepo
        //        .GetScenarioCashFlowRules(simulation.Id);
        //    Assert.Single(modifiedDtos);

        //    Assert.Equal(dtos[0].Name, modifiedDtos[0].Name);
        //    Assert.Equal(dtos[0].CriterionLibrary.MergedCriteriaExpression,
        //        modifiedDtos[0].CriterionLibrary.MergedCriteriaExpression);

        //    Assert.Equal(dtos[0].CashFlowDistributionRules[0].DurationInYears,
        //        modifiedDtos[0].CashFlowDistributionRules[0].DurationInYears);
        //}

        //[Fact]
        //public async Task ShouldDeleteLibraryData()
        //{
        //    // Arrange
        //    Setup();
        //    CreateAuthorizedController();
        //    CreateLibraryTestData();

        //    // Act
        //    var result = await _controller.DeleteCashFlowRuleLibrary(_testCashFlowRuleLibrary.Id);

        //    // Assert
        //    Assert.IsType<OkResult>(result);

        //    Assert.True(
        //        !TestHelper.UnitOfWork.Context.CashFlowRuleLibrary.Any(_ => _.Id == _testCashFlowRuleLibrary.Id));
        //    Assert.True(!TestHelper.UnitOfWork.Context.CashFlowRule.Any(_ => _.Id == _testCashFlowRule.Id));
        //    Assert.True(
        //        !TestHelper.UnitOfWork.Context.CriterionLibraryCashFlowRule.Any(_ =>
        //            _.CashFlowRuleId == _testCashFlowRule.Id));
        //    Assert.True(
        //        !TestHelper.UnitOfWork.Context.CashFlowDistributionRule.Any(_ =>
        //            _.Id == _testCashFlowDistributionRule.Id));
        //}

        //[Fact]
        //public async Task UserIsViewCashFlowFromLibraryAuthorized()
        //{
        //    // non-admin authorize test
        //    // Arrange
        //    var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
        //    {
        //        services.AddAuthorization(options =>
        //        {
        //            options.AddPolicy(Policy.ViewCashFlowFromLibrary,
        //                policy => policy.RequireClaim(ClaimTypes.Name,
        //                                              BridgeCareCore.Security.SecurityConstants.Claim.CashFlowViewAnyFromLibraryAccess,
        //                                              BridgeCareCore.Security.SecurityConstants.Claim.CashFlowViewPermittedFromLibraryAccess));
        //        });
        //    });
        //    var roleClaimsMapper = new RoleClaimsMapper();
        //    var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Editor }));
        //    // Act
        //    var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ViewCashFlowFromLibrary);
        //    // Assert
        //    Assert.True(allowed.Succeeded);
        //}
        //[Fact]
        //public async Task UserIsModifyCashFlowFromScenarioAuthorized()
        //{
        //    // admin authorize test
        //    // Arrange
        //    var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
        //    {
        //        services.AddAuthorization(options =>
        //        {
        //            options.AddPolicy(Policy.ModifyCashFlowFromScenario,
        //                policy => policy.RequireClaim(ClaimTypes.Name,
        //                                              BridgeCareCore.Security.SecurityConstants.Claim.CashFlowModifyPermittedFromScenarioAccess,
        //                                              BridgeCareCore.Security.SecurityConstants.Claim.CashFlowModifyAnyFromScenarioAccess));
        //        });
        //    });
        //    var roleClaimsMapper = new RoleClaimsMapper();
        //    var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
        //    // Act
        //    var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ModifyCashFlowFromScenario);
        //    // Assert
        //    Assert.True(allowed.Succeeded);
        //}
        //[Fact]
        //public async Task UserIsModifyCashFlowFromLibraryAuthorized()
        //{
        //    // non-admin unauthorized test
        //    // Arrange
        //    var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
        //    {
        //        services.AddAuthorization(options =>
        //        {
        //            options.AddPolicy(Policy.ModifyCashFlowFromLibrary,
        //                policy => policy.RequireClaim(ClaimTypes.Name,
        //                                              BridgeCareCore.Security.SecurityConstants.Claim.CashFlowModifyAnyFromLibraryAccess));
        //        });
        //    });
        //    var roleClaimsMapper = new RoleClaimsMapper();
        //    var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Editor }));
        //    // Act
        //    var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ModifyCashFlowFromLibrary);
        //    // Assert
        //    Assert.False(allowed.Succeeded);
        //}
        //[Fact]
        //public async Task UserIsViewCashFlowFromLibraryAuthorized_B2C()
        //{
        //    // Arrange
        //    var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
        //    {
        //        services.AddAuthorization(options =>
        //        {
        //            options.AddPolicy(Policy.ViewCashFlowFromLibrary,
        //                policy => policy.RequireClaim(ClaimTypes.Name,
        //                                              BridgeCareCore.Security.SecurityConstants.Claim.CashFlowViewAnyFromLibraryAccess,
        //                                              BridgeCareCore.Security.SecurityConstants.Claim.CashFlowViewPermittedFromLibraryAccess));
        //        });
        //    });
        //    var roleClaimsMapper = new RoleClaimsMapper();
        //    var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.B2C, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
        //    // Act
        //    var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ViewCashFlowFromLibrary);
        //    // Assert
        //    Assert.True(allowed.Succeeded);
        //}
    }
}
