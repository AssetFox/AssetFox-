using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CashFlow;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.CashFlowRule;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.User;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Microsoft.Data.SqlClient;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore
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
        public async Task UpsertLibrary_Does()
        {
            // Arrange
            var dto = CashFlowRuleLibraryDtos.Empty();

            // Act
            TestHelper.UnitOfWork.CashFlowRuleRepo.UpsertCashFlowRuleLibrary(dto);

            // Assert
            var libraryAfter = TestHelper.UnitOfWork.CashFlowRuleRepo.GetCashFlowRuleLibrariesNoChildren()
                .Single(lib => lib.Id == dto.Id);
            ObjectAssertions.EquivalentExcluding(dto, libraryAfter, x => x.Owner);
        }

        [Fact]
        public void UpsertOrDeleteScenarioCashFlowRules_SimulationInDb_Does()
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
            ObjectAssertions.EquivalentExcluding(dto, modifiedDto, x => x.CashFlowRules[0].CriterionLibrary, x => x.Owner);
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
        public void UpsertOrDeleteScenarioCashFlowRules_TwoAddsCollide_NothingChanges()
        {
            // Arrange
            Setup();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var scenarioRule = CreateScenarioTestData(simulation.Id);
            var scenarioDto = scenarioRule.ToDto();
            scenarioDto.Name = "Updated Name";
            scenarioDto.CriterionLibrary.MergedCriteriaExpression = "Updated Expression";
            scenarioDto.CashFlowDistributionRules[0].DurationInYears = 2;
            var collidingId = Guid.NewGuid();
            var collidingDto1 = CashFlowRuleDtos.Rule(collidingId);
            var collidingDto2 = CashFlowRuleDtos.Rule(collidingId);
            var dtosToUpsert = new List<CashFlowRuleDTO> { scenarioDto, collidingDto1, collidingDto2 };
            var dtosBefore = TestHelper.UnitOfWork.CashFlowRuleRepo.GetScenarioCashFlowRules(simulation.Id);

            // Act
            var exception = Assert.Throws<SqlException>(() => TestHelper.UnitOfWork.CashFlowRuleRepo.UpsertOrDeleteScenarioCashFlowRules(dtosToUpsert, simulation.Id));

            // Assert
            var dtosAfter = TestHelper.UnitOfWork.CashFlowRuleRepo
                .GetScenarioCashFlowRules(simulation.Id);
            ObjectAssertions.Equivalent(dtosBefore, dtosAfter);
        }

        [Fact]
        public void UpsertOrDeleteScenarioCashFlowRules_RuleInDb_Modifies()
        {
            // Arrange
            Setup();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var scenarioRule = CreateScenarioTestData(simulation.Id);
            var scenarioDto = scenarioRule.ToDto();
            scenarioDto.Name = "Updated Name";
            scenarioDto.CriterionLibrary.MergedCriteriaExpression = "Updated Expression";
            scenarioDto.CashFlowDistributionRules[0].DurationInYears = 2;

            var dtos = new List<CashFlowRuleDTO> { scenarioDto };

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
        [Fact]
        public async Task UpdateCashFlowRuleLibraryWithUserAccessChange_Does()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = CashFlowRuleLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            CashFlowRuleLibraryUserTestSetup.SetUsersOfCashFlowRuleLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user.Id);
            var libraryUsersBefore = TestHelper.UnitOfWork.CashFlowRuleRepo.GetLibraryUsers(library.Id);
            var libraryUserBefore = libraryUsersBefore.Single();
            Assert.Equal(LibraryAccessLevel.Modify, libraryUserBefore.AccessLevel);
            libraryUserBefore.AccessLevel = LibraryAccessLevel.Read;

            TestHelper.UnitOfWork.CashFlowRuleRepo.UpsertOrDeleteUsers(library.Id, libraryUsersBefore);

            var libraryUsersAfter = TestHelper.UnitOfWork.CashFlowRuleRepo.GetLibraryUsers(library.Id);
            var libraryUserAfter = libraryUsersAfter.Single();
            Assert.Equal(LibraryAccessLevel.Read, libraryUserAfter.AccessLevel);
        }
        [Fact]
        public async Task UpdateCashFlowRuleLibraryUsers_RequestAccessRemoval_Does()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = CashFlowRuleLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            CashFlowRuleLibraryUserTestSetup.SetUsersOfCashFlowRuleLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user.Id);
            var libraryUsersBefore = TestHelper.UnitOfWork.CashFlowRuleRepo.GetLibraryUsers(library.Id);
            var libraryUserBefore = libraryUsersBefore.Single();
            libraryUsersBefore.Remove(libraryUserBefore);

            TestHelper.UnitOfWork.CashFlowRuleRepo.UpsertOrDeleteUsers(library.Id, libraryUsersBefore);
            TestHelper.UnitOfWork.Context.SaveChanges();

            var libraryUsersAfter = TestHelper.UnitOfWork.CashFlowRuleRepo.GetLibraryUsers(library.Id);
            Assert.Empty(libraryUsersAfter);
        }
        [Fact]
        public async Task UpdateLibraryUsers_AddAccessForUser_Does()
        {
            var user1 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var user2 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = CashFlowRuleLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            CashFlowRuleLibraryUserTestSetup.SetUsersOfCashFlowRuleLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user1.Id);
            var usersBefore = TestHelper.UnitOfWork.CashFlowRuleRepo.GetLibraryUsers(library.Id);
            var newUser = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Read,
                UserId = user2.Id,
            };
            usersBefore.Add(newUser);

            TestHelper.UnitOfWork.CashFlowRuleRepo.UpsertOrDeleteUsers(library.Id, usersBefore);

            var libraryUsersAfter = TestHelper.UnitOfWork.CashFlowRuleRepo.GetLibraryUsers(library.Id);
            var user1After = libraryUsersAfter.Single(u => u.UserId == user1.Id);
            var user2After = libraryUsersAfter.Single(u => u.UserId == user2.Id);
            Assert.Equal(LibraryAccessLevel.Modify, user1After.AccessLevel);
            Assert.Equal(LibraryAccessLevel.Read, user2After.AccessLevel);
        }
    }
}
