using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CashFlow;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.CashFlowRule;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class CashFlowRuleRepositoryTests
    {

        private void Setup()
        {
            var unitOfWork = TestHelper.UnitOfWork;
            AttributeTestSetup.CreateAttributes(unitOfWork);
            NetworkTestSetup.CreateNetwork(unitOfWork);
        }

        private ScenarioCashFlowRuleEntity CreateScenarioTestData(Guid simulationId)
        {
            var testScenarioCashFlowRule = new ScenarioCashFlowRuleEntity
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
            TestHelper.UnitOfWork.Context.AddEntity(testScenarioCashFlowRule);


            var testScenarioCashFlowDistributionRule = new ScenarioCashFlowDistributionRuleEntity
            {
                Id = Guid.NewGuid(),
                ScenarioCashFlowRuleId = testScenarioCashFlowRule.Id,
                DurationInYears = 1,
                CostCeiling = 500000,
                YearlyPercentages = "100"
            };
            TestHelper.UnitOfWork.Context.AddEntity(testScenarioCashFlowDistributionRule);
            testScenarioCashFlowRule.ScenarioCashFlowDistributionRules.Add(testScenarioCashFlowDistributionRule);
            return testScenarioCashFlowRule;
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
            var dto = CashFlowRuleLibraryDtos.Empty();

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
            var library = CashFlowRuleLibraryDtos.Empty();
            var rule = CashFlowRuleDtos.Rule();
            var rules = new List<CashFlowRuleDTO> { rule };
            TestHelper.UnitOfWork.CashFlowRuleRepo.UpsertCashFlowRuleLibrary(library);
            TestHelper.UnitOfWork.CashFlowRuleRepo.UpsertOrDeleteCashFlowRules(rules, library.Id);

            // Act
            var dtos = TestHelper.UnitOfWork.CashFlowRuleRepo.GetCashFlowRuleLibrariesNoChildren();

            var relevantDto = dtos.Single(dto => dto.Id == library.Id);
            Assert.Empty(relevantDto.CashFlowRules);
        }

        [Fact]
        public void GetScenarioCashFlowRules_SimulationInDb_Gets()
        {
            // Arrange
            Setup();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var scenarioRule = CreateScenarioTestData(simulation.Id);

            // Act
            var dtos = TestHelper.UnitOfWork.CashFlowRuleRepo.GetScenarioCashFlowRules(simulation.Id);

            // Assert
            var dto = dtos.Single();
            Assert.Equal(scenarioRule.Id, dto.Id);
            Assert.Equal(scenarioRule.CriterionLibraryScenarioCashFlowRuleJoin.CriterionLibrary.Id,
                dto.CriterionLibrary.Id);
            Assert.Single(dto.CashFlowDistributionRules);
            Assert.Equal(scenarioRule.ScenarioCashFlowDistributionRules.Single().Id,
                dto.CashFlowDistributionRules[0].Id);
        }

        [Fact]
        public void UpsertCashFlowRuleLibrary_LibraryInDb_Modifies()
        {
            // Arrange
            var dto = CashFlowRuleLibraryDtos.Empty();
            TestHelper.UnitOfWork.CashFlowRuleRepo.UpsertCashFlowRuleLibrary(dto);
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
            var rule = CashFlowRuleDtos.Rule();
            var rules = new List<CashFlowRuleDTO> { rule };
            var library = CashFlowRuleLibraryDtos.Empty();
            TestHelper.UnitOfWork.CashFlowRuleRepo.UpsertCashFlowRuleLibrary(library);
            TestHelper.UnitOfWork.CashFlowRuleRepo.UpsertOrDeleteCashFlowRules(rules, library.Id);
            rule.CriterionLibrary.MergedCriteriaExpression = null;
            rule.Name = "Updated Name";
            rule.CriterionLibrary.MergedCriteriaExpression = "Updated Expression";
            rule.CashFlowDistributionRules[0].DurationInYears = 2;

            // Act
            TestHelper.UnitOfWork.CashFlowRuleRepo.UpsertOrDeleteCashFlowRules(rules, library.Id);

            // Assert
            var modifiedDto = TestHelper.UnitOfWork.CashFlowRuleRepo.GetCashFlowRuleLibraries().Single(lib => lib.Id == library.Id);
            
            ObjectAssertions.EquivalentExcluding(rule, modifiedDto.CashFlowRules[0], x => x.CriterionLibrary);
        }

        [Fact]
        public void UpsertOrDeleteScenarioCashFlowRules_RuleInDb_Modifies()
        {
            // Arrange
            Setup();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var scenarioRule = CreateScenarioTestData(simulation.Id);

            var dtos = new List<CashFlowRuleDTO> { scenarioRule.ToDto() };
            dtos[0].Name = "Updated Name";
            dtos[0].CriterionLibrary.MergedCriteriaExpression = "Updated Expression";
            dtos[0].CashFlowDistributionRules[0].DurationInYears = 2;

            // Act
            TestHelper.UnitOfWork.CashFlowRuleRepo.UpsertOrDeleteScenarioCashFlowRules(dtos, simulation.Id);

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
        public void ShouldDeleteLibraryData()
        {
            // Arrange
            var library = CashFlowRuleLibraryDtos.Empty();
            TestHelper.UnitOfWork.CashFlowRuleRepo.UpsertCashFlowRuleLibrary(library);
            var rule = CashFlowRuleDtos.Rule();
            var rules = new List<CashFlowRuleDTO> { rule };
            TestHelper.UnitOfWork.CashFlowRuleRepo.UpsertOrDeleteCashFlowRules(rules, library.Id);
            Assert.True(TestHelper.UnitOfWork.Context.CashFlowRuleLibrary.Any(_ => _.Id == library.Id));
            Assert.True(TestHelper.UnitOfWork.Context.CashFlowRule.Any(_ => _.Id == rule.Id));
            Assert.True(TestHelper.UnitOfWork.Context.CashFlowDistributionRule.Any(_ =>
                    _.Id == rule.CashFlowDistributionRules[0].Id));
            // Act
            TestHelper.UnitOfWork.CashFlowRuleRepo.DeleteCashFlowRuleLibrary(library.Id);

            Assert.False(TestHelper.UnitOfWork.Context.CashFlowRuleLibrary.Any(_ => _.Id == library.Id));
            Assert.False(TestHelper.UnitOfWork.Context.CashFlowRule.Any(_ => _.Id == rule.Id));
            Assert.False(TestHelper.UnitOfWork.Context.CashFlowDistributionRule.Any(_ =>
                    _.Id == rule.CashFlowDistributionRules[0].Id));
        }
    }
}
