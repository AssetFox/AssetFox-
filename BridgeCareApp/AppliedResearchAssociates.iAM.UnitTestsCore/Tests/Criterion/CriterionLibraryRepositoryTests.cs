using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Criterion
{
    public class CriterionLibraryRepositoryTests
    {
        [Fact]
        public async Task UpsertCriterionLibrary_LibraryNotInDb_Adds()
        {
            var repo = TestHelper.UnitOfWork.CriterionLibraryRepo;
            var dto = CriterionLibraryTestSetup.TestCriterionLibrary();

            // Act
            var id = repo.UpsertCriterionLibrary(dto);

            // Assert
            Assert.Equal(id, dto.Id);
            var dtoAfter = await repo.CriteriaLibrary(id);
            ObjectAssertions.EquivalentExcluding(dto, dtoAfter, x => x.Owner);
        }

        [Fact]
        public void DeleteCriterionLibrary_LibraryNotInDb_DoesNotThrow()
        {
            TestHelper.UnitOfWork.CriterionLibraryRepo.DeleteCriterionLibrary(Guid.NewGuid());
        }

        [Fact]
        public void DeleteCriterionLibrary_LibraryInDb_Deletes()
        {
            // Arrange
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(TestHelper.UnitOfWork);
            Assert.True(TestHelper.UnitOfWork.Context.CriterionLibrary.Any(_ => _.Id == criterionLibrary.Id));

            // Act
            TestHelper.UnitOfWork.CriterionLibraryRepo.DeleteCriterionLibrary(criterionLibrary.Id);

            // Assert
            Assert.False(TestHelper.UnitOfWork.Context.CriterionLibrary.Any(_ => _.Id == criterionLibrary.Id));
        }

        [Fact]
        public async Task GetCriterionLibraries_LibraryInDb_Gets()
        {
            // Arrange
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(TestHelper.UnitOfWork, isSingleUse: false);

            // Act
            var dtos = await TestHelper.UnitOfWork.CriterionLibraryRepo.CriterionLibraries();

            // Assert
            Assert.Contains(dtos, cl => cl.Id == criterionLibrary.Id);
        }

        [Fact]
        public void DeleteAllSingleUseCriterionLibrariesWithBudgetNamesForSimulation_SimulationInDbWithBudgetWithCriterionLibrary_DeletesCriterionLibrary()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var budgetId = Guid.NewGuid();
            var budgetName = RandomStrings.WithPrefix("budget ");
            var budget = BudgetDtos.New(budgetId, budgetName);
            var criterionLibrary = CriterionLibraryDtos.Dto();
            budget.CriterionLibrary = criterionLibrary;
            var budgets = new List<BudgetDTO> { budget };
            TestHelper.UnitOfWork.BudgetRepo.UpsertOrDeleteScenarioBudgets(budgets, simulation.Id);
            var budgetNames = new List<string> { budgetName };
            var scenarioBudgetsBefore = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(simulation.Id);
            var criterionLibraryBefore = scenarioBudgetsBefore.Single().CriterionLibrary;

            TestHelper.UnitOfWork.CriterionLibraryRepo.DeleteAllSingleUseCriterionLibrariesWithBudgetNamesForSimulation(simulation.Id, budgetNames);

            var scenarioBudgetsAfter = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(simulation.Id);
            var criterionLibraryAfter = scenarioBudgetsAfter.Single().CriterionLibrary;
            Assert.Equal(Guid.Empty, criterionLibraryAfter.Id);
            Assert.NotEqual(Guid.Empty, criterionLibraryBefore.Id);
        }

        [Fact]
        public void DeleteAllSingleUseCriterionLibrariesWithBudgetNamesForBudgetLibrary_BudgetInDbWithBudgetLibraryAndCriterionLibrary_Deletes()
        {
            var budgetLibrary = BudgetLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var budgetId = Guid.NewGuid();
            var budgetName = RandomStrings.WithPrefix("budget ");
            var budget = BudgetDtos.New(budgetId, budgetName);
            var criterionLibrary = CriterionLibraryDtos.Dto();
            budget.CriterionLibrary = criterionLibrary;
            var budgets = new List<BudgetDTO> { budget };
            TestHelper.UnitOfWork.BudgetRepo.UpsertOrDeleteBudgets(budgets, budgetLibrary.Id);
            var budgetNames = new List<string> { budgetName };
            var budgetLibraryBefore = TestHelper.UnitOfWork.BudgetRepo.GetBudgetLibrary(budgetLibrary.Id);
            var criterionLibraryBefore = budgetLibraryBefore.Budgets.Single().CriterionLibrary;

            TestHelper.UnitOfWork.CriterionLibraryRepo.DeleteAllSingleUseCriterionLibrariesWithBudgetNamesForBudgetLibrary(budgetLibrary.Id, budgetNames);

            var budgetLibraryAfter = TestHelper.UnitOfWork.BudgetRepo.GetBudgetLibrary(budgetLibrary.Id);
            var criterionLibraryAfter = budgetLibraryAfter.Budgets.Single().CriterionLibrary;

            Assert.NotEqual(Guid.Empty, criterionLibraryBefore.Id);
            Assert.Equal(Guid.Empty, criterionLibraryAfter.Id);
        }

        [Fact]
        public async Task AddLibraries_Does()
        {
            var libraryId = Guid.NewGuid();
            var library = CriterionLibraryDtos.Dto(libraryId);
            var libraries = new List<CriterionLibraryDTO> { library };

            TestHelper.UnitOfWork.CriterionLibraryRepo.AddLibraries(libraries);

            var libraryAfter = await TestHelper.UnitOfWork.CriterionLibraryRepo.CriteriaLibrary(libraryId);
            ObjectAssertions.Equivalent(library, libraryAfter);
        }

        [Fact]
        public void AddLibraryBudgetJoins_CriterionLibraryAndBudgetInDb_Does()
        {
            var budgetId = Guid.NewGuid();
            var library = CriterionLibraryTestSetup.TestCriterionLibraryInDb(TestHelper.UnitOfWork);
            var libraryId = library.Id;
            var budgetLibraryJoin = new CriterionLibraryBudgetDTO
            {
                BudgetId = budgetId,
                CriterionLibraryId = libraryId,
            };
            var budgetLibraryJoins = new List<CriterionLibraryBudgetDTO> { budgetLibraryJoin };

            TestHelper.UnitOfWork.CriterionLibraryRepo.AddLibraryBudgetJoins(budgetLibraryJoins);

            var budgetLibraryJoinAfter = TestHelper.UnitOfWork.Context.CriterionLibraryBudget.Single(c => c.BudgetId == budgetId);
            Assert.Equal(libraryId, budgetLibraryJoinAfter.CriterionLibraryId);
        }

        [Fact]
        public void AddScenarioBudgetJoins_CriterionLibraryAndBudgetInDb_Does()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulationId = Guid.NewGuid();
            var simulation = SimulationTestSetup.CreateSimulation(
                TestHelper.UnitOfWork, simulationId);
            var budgetId = Guid.NewGuid();
            var library = CriterionLibraryTestSetup.TestCriterionLibraryInDb(TestHelper.UnitOfWork);
            var libraryId = library.Id;
            var budgetLibraryJoin = new CriterionLibraryScenarioBudgetDTO
            {
                ScenarioBudgetId = budgetId,
                CriterionLibraryId = libraryId,
            };
            var budgetLibraryJoins = new List<CriterionLibraryScenarioBudgetDTO> { budgetLibraryJoin };

            TestHelper.UnitOfWork.CriterionLibraryRepo.AddLibraryScenarioBudgetJoins(budgetLibraryJoins);

            var budgetLibraryJoinAfter = TestHelper.UnitOfWork.Context.CriterionLibraryScenarioBudget.Single(c => c.ScenarioBudgetId == budgetId);
            Assert.Equal(libraryId, budgetLibraryJoinAfter.CriterionLibraryId);
        }
    }
}
