using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.SimulationCloning;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class SimulationCloningScenarioBudgetTests
    {
        [Fact]
        public void SimulationInDbWithBudgetWithPercentagePair_Clone_Clones()
        {
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            var budgetId = Guid.NewGuid();
            var budget = BudgetDtos.WithSingleAmount(budgetId, "budget", 2023, 4321);
            var budgets = new List<BudgetDTO> { budget };
            ScenarioBudgetTestSetup.UpsertOrDeleteScenarioBudgets(TestHelper.UnitOfWork, budgets, simulationId);
            var budgetPriority = BudgetPriorityDtos.New();
            var percentagePair = new BudgetPercentagePairDTO
            {
                BudgetId = budget.Id,
                BudgetName = budget.Name,
                Percentage = 12,
            };
            budgetPriority.BudgetPercentagePairs = new List<BudgetPercentagePairDTO> { percentagePair };
            var budgetPriorities = new List<BudgetPriorityDTO> { budgetPriority };
            TestHelper.UnitOfWork.BudgetPriorityRepo.UpsertOrDeleteScenarioBudgetPriorities(budgetPriorities, simulationId);

            var cloningResult = TestHelper.UnitOfWork.SimulationRepo.CloneSimulation(simulationEntity.Id, networkId, newSimulationName);

            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(cloningResult.Simulation.Id);
            var clonedPriorities = TestHelper.UnitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(clonedSimulation.Id);
            var clonedPriority = clonedPriorities.Single();
            ObjectAssertions.EquivalentExcluding(budgetPriority, clonedPriority,
                bp => bp.Id, bp => bp.CriterionLibrary,
                bp => bp.BudgetPercentagePairs[0].Id,
                bp => bp.BudgetPercentagePairs[0].BudgetId);
        }

        [Fact]
        public void SimulationInDbWithScenarioBudget_Clone_Clones()
        {
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            var budgetId = Guid.NewGuid();
            var budget = BudgetDtos.New(budgetId);
            var budgets = new List<BudgetDTO> { budget };
            ScenarioBudgetTestSetup.UpsertOrDeleteScenarioBudgets(TestHelper.UnitOfWork, budgets, simulationId);

            var cloningResult = TestHelper.UnitOfWork.SimulationRepo.CloneSimulation(simulationEntity.Id, networkId, newSimulationName);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(cloningResult.Simulation.Id);
            var clonedBudgets = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(cloningResult.Simulation.Id);
            var clonedBudget = clonedBudgets.Single();
            ObjectAssertions.EquivalentExcluding(budget, clonedBudget, b => b.Id, b => b.CriterionLibrary);
        }

        [Fact]
        public void SimulationInDbWithScenarioBudgetInCriterionLibrary_Clone_Clones()
        {
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            var budgetId = Guid.NewGuid();
            var budget = BudgetDtos.New(budgetId);
            var budgets = new List<BudgetDTO> { budget };
            var criterionLibrary = CriterionLibraryDtos.Dto();
            budget.CriterionLibrary = criterionLibrary;
            ScenarioBudgetTestSetup.UpsertOrDeleteScenarioBudgets(TestHelper.UnitOfWork, budgets, simulationId);

            var cloningResult = TestHelper.UnitOfWork.SimulationRepo.CloneSimulation(simulationEntity.Id, networkId, newSimulationName);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(cloningResult.Simulation.Id);
            var clonedBudgets = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(cloningResult.Simulation.Id);
            var clonedBudget = clonedBudgets.Single();
            ObjectAssertions.EquivalentExcluding(budget, clonedBudget, b => b.Id, b => b.CriterionLibrary);
            var expectedCriterionLibrary = new CriterionLibraryDTO
            {
                Name = "Budget Criterion",
                MergedCriteriaExpression = "mergedCriteriaExpression",
                IsSingleUse = true,
                Owner = TestHelper.UnitOfWork.CurrentUser?.Id??Guid.Empty,
            };
            ObjectAssertions.EquivalentExcluding(expectedCriterionLibrary, clonedBudget.CriterionLibrary, c => c.Id);
        }
    }
}
