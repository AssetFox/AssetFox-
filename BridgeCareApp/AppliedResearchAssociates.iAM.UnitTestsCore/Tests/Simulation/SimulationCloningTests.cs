using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.CashFlowRule;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class SimulationCloningTests
    {
        [Fact]
        public void SimulationInDb_Clone_Clones()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var networkId = NetworkTestSetup.NetworkId;
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);

            var cloningResult = TestHelper.UnitOfWork.SimulationRepo.CloneSimulation(simulationEntity.Id, networkId, newSimulationName);

            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(cloningResult.Simulation.Id);
            Assert.Equal(newSimulationName, clonedSimulation.Name);
            Assert.Equal(networkId, clonedSimulation.NetworkId);
            Assert.Equal("Test Network", clonedSimulation.NetworkName);
        }

        [Fact]
        public void SimulationInDbWithScenarioBudget_Clone_Clones()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var networkId = NetworkTestSetup.NetworkId;
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            var budgetId = Guid.NewGuid();
            var budget = BudgetDtos.New(budgetId);
            var budgets = new List<BudgetDTO> { budget };
            ScenarioBudgetTestSetup.UpsertOrDeleteScenarioBudgets(TestHelper.UnitOfWork, budgets, simulationId);

            var cloningResult = TestHelper.UnitOfWork.SimulationRepo.CloneSimulation(simulationEntity.Id, networkId, newSimulationName);

            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(cloningResult.Simulation.Id);
            var clonedBudgets = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(cloningResult.Simulation.Id);
            var clonedBudget = clonedBudgets.Single();
            ObjectAssertions.EquivalentExcluding(budget, clonedBudget, b => b.Id, b => b.CriterionLibrary);
        }


        [Fact]
        public void SimulationInDbWithScenarioBudgetWithAmount_Clone_Clones()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var networkId = NetworkTestSetup.NetworkId;
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            var budgetId = Guid.NewGuid();
            var budget = BudgetDtos.WithSingleAmount(budgetId, "budget", 2023, 4321);
            var budgets = new List<BudgetDTO> { budget };
            ScenarioBudgetTestSetup.UpsertOrDeleteScenarioBudgets(TestHelper.UnitOfWork, budgets, simulationId);

            var cloningResult = TestHelper.UnitOfWork.SimulationRepo.CloneSimulation(simulationEntity.Id, networkId, newSimulationName);

            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(cloningResult.Simulation.Id);
            var clonedBudgets = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(cloningResult.Simulation.Id);
            var clonedBudget = clonedBudgets.Single();
            ObjectAssertions.EquivalentExcluding(budget, clonedBudget, b => b.Id, b => b.CriterionLibrary, b => b.BudgetAmounts[0].Id);
        }

        [Fact]
        public void SimulationInDbWithBudgetPriority_Clone_Clones()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var networkId = NetworkTestSetup.NetworkId;
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            var budgetPriority = BudgetPriorityDtos.New();
            var budgetPriorities = new List<BudgetPriorityDTO> { budgetPriority };
            TestHelper.UnitOfWork.BudgetPriorityRepo.UpsertOrDeleteScenarioBudgetPriorities(budgetPriorities, simulationId);
            
            var cloningResult = TestHelper.UnitOfWork.SimulationRepo.CloneSimulation(simulationEntity.Id, networkId, newSimulationName);

            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(cloningResult.Simulation.Id);
            var clonedPriorities = TestHelper.UnitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(clonedSimulation.Id);
            var clonedPriority = clonedPriorities.Single();
            ObjectAssertions.EquivalentExcluding(budgetPriority, clonedPriority, bp => bp.Id, bp => bp.CriterionLibrary);
        }


        [Fact]
        public void SimulationInDbWithBudgetWithPercentagePair_Clone_Clones()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var networkId = NetworkTestSetup.NetworkId;
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
        public void SimulationInDbWithCashFlowRule_Clone_Clones()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var networkId = NetworkTestSetup.NetworkId;
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            var ruleId = Guid.NewGuid();
            var cashFlowRule = CashFlowRuleDtos.Rule(ruleId);
            var cashFlowRules = new List<CashFlowRuleDTO> { cashFlowRule };
            TestHelper.UnitOfWork.CashFlowRuleRepo.UpsertOrDeleteScenarioCashFlowRules(cashFlowRules, simulationId);

            var cloningResult = TestHelper.UnitOfWork.SimulationRepo.CloneSimulation(simulationEntity.Id, networkId, newSimulationName);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedCashFlowRules = TestHelper.UnitOfWork.CashFlowRuleRepo.GetScenarioCashFlowRules(clonedSimulationId);
            var clonedCashFlowRule = clonedCashFlowRules.Single();
            Assert.Equal(cashFlowRule.Name, clonedCashFlowRule.Name);
            var distributionRule = cashFlowRule.CashFlowDistributionRules.Single();
            var clonedDistributionRule = clonedCashFlowRule.CashFlowDistributionRules.Single();
            ObjectAssertions.EquivalentExcluding(distributionRule, clonedDistributionRule, x => x.Id);
        }
    }
}
