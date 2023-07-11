﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes.CalculatedAttributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.BudgetPriority;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.CashFlowRule;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.SimulationCloning;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.User;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class SimulationCloningTests
    {

        [Fact]
        public void SimulationInDbWithAnalysisMethodInCriterionLibrary_Clone_Clones()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var networkEntity = NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var networkId = networkEntity.Id;
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            var explorer = TestHelper.UnitOfWork.AttributeRepo.GetExplorer();
            var network = NetworkMapper.ToDomain(networkEntity, explorer);
            var date = new DateTime(2023, 5, 3);
            SimulationMapper.CreateSimulation(simulationEntity, network, date, date);
            var simulation = network.Simulations.Single(s => s.Id == simulationId);
            var analysisMethodId = Guid.NewGuid();
            var analysisMethodDto = TestHelper.UnitOfWork.AnalysisMethodRepo.GetAnalysisMethod(simulationId);
            analysisMethodDto.Benefit = new BenefitDTO
            {
                Id = Guid.NewGuid(),
                Limit = 0.0,
                Attribute = TestAttributeNames.CulvDurationN,
            };
            analysisMethodDto.CriterionLibrary = CriterionLibraryDtos.Dto();
            var budgetPriority = BudgetPriorityTestSetup.SetupSingleBudgetPriorityForSimulationInDb(simulationId);
            TestHelper.UnitOfWork.AnalysisMethodRepo.UpsertAnalysisMethod(simulationId, analysisMethodDto);
            TestHelper.UnitOfWork.AnalysisMethodRepo.GetSimulationAnalysisMethod(simulation, "");
            var newSimulationName = RandomStrings.WithPrefix("cloned");

            var cloningResult = TestHelper.UnitOfWork.SimulationRepo.CloneSimulation(simulationEntity.Id, networkId, newSimulationName);

            var clonedSimulation = cloningResult.Simulation;
            var clonedAnalysisMethod = TestHelper.UnitOfWork.AnalysisMethodRepo.GetAnalysisMethod(clonedSimulation.Id);
            Assert.NotEqual(analysisMethodDto.Id, clonedAnalysisMethod.Id);
            Assert.NotEqual(analysisMethodDto.CriterionLibrary.Id, clonedAnalysisMethod.CriterionLibrary.Id);
            Assert.Equal("mergedCriteriaExpression", clonedAnalysisMethod.CriterionLibrary.MergedCriteriaExpression);
        }

        [Fact]
        public async Task SimulationInDbWithUserJoin_Clone_Clones()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, false);
            TestHelper.UnitOfWork.SetUser(user.Username);
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);

            var cloningResult = TestHelper.UnitOfWork.SimulationRepo.CloneSimulation(simulationEntity.Id, networkId, newSimulationName);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(clonedSimulationId);
            Assert.Equal(newSimulationName, clonedSimulation.Name);
            Assert.Equal(networkId, clonedSimulation.NetworkId);
            Assert.Equal("Test Network", clonedSimulation.NetworkName);
            var clonedSimulationUser = clonedSimulation.Users.Single();
            Assert.Equal(user.Username, clonedSimulationUser.Username);
        }

        [Fact]
        public void SimulationInDb_Clone_Clones()
        {
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);

            var cloningResult = TestHelper.UnitOfWork.SimulationRepo.CloneSimulation(simulationEntity.Id, networkId, newSimulationName);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(clonedSimulationId);
            Assert.Equal(newSimulationName, clonedSimulation.Name);
            Assert.Equal(networkId, clonedSimulation.NetworkId);
            Assert.Equal("Test Network", clonedSimulation.NetworkName);
        }

        [Fact]
        public void SimulationInDbWithCalculatedAttribute_Clone_Clones()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
           
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            var calculatedAttributeId = Guid.NewGuid();
            var calculatedAttribute = CalculatedAttributeTestSetup.TestCalculatedAttributeDto(calculatedAttributeId, TestAttributeNames.Age);
            var calculatedAttributes = new List<CalculatedAttributeDTO> { calculatedAttribute };
            TestHelper.UnitOfWork.CalculatedAttributeRepo.UpsertScenarioCalculatedAttributes(calculatedAttributes, simulationId);

            var cloningResult = TestHelper.UnitOfWork.SimulationRepo.CloneSimulation(simulationEntity.Id, networkId, newSimulationName);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(clonedSimulationId);
            Assert.Equal(newSimulationName, clonedSimulation.Name);
            Assert.Equal(networkId, clonedSimulation.NetworkId);
            Assert.Equal("Test Network", clonedSimulation.NetworkName);
            var allAttributes = TestHelper.UnitOfWork.AttributeRepo.GetAttributes();
            var ageAttribute = allAttributes.Single(a => a.Name == TestAttributeNames.Age);
            var clonedAttribute = TestHelper.UnitOfWork.CalculatedAttributeRepo.GetScenarioCalulatedAttributesByScenarioAndAttributeId(clonedSimulationId, ageAttribute.Id);
            ObjectAssertions.EquivalentExcluding(calculatedAttribute, clonedAttribute, c => c.Id, c => c.Equations[0].Id, c => c.Equations[0].Equation.Id);
            Assert.NotEqual(calculatedAttribute.Id, clonedAttribute.Id);
            Assert.NotEqual(calculatedAttribute.Equations[0].Id, clonedAttribute.Equations[0].Id);
            Assert.NotEqual(calculatedAttribute.Equations[0].Equation.Id, clonedAttribute.Equations[0].Equation.Id);
        }

        [Fact]
        public void SimulationInDbWithCalculatedAttributeWithCriterionLibrary_Clone_Clones()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);

            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            var calculatedAttributeId = Guid.NewGuid();
            var calculatedAttribute = CalculatedAttributeTestSetup.TestCalculatedAttributeDtoWithEquationCriterionLibrary(calculatedAttributeId, TestAttributeNames.Age, "");
            var calculatedAttributes = new List<CalculatedAttributeDTO> { calculatedAttribute };
            TestHelper.UnitOfWork.CalculatedAttributeRepo.UpsertScenarioCalculatedAttributes(calculatedAttributes, simulationId);

            var cloningResult = TestHelper.UnitOfWork.SimulationRepo.CloneSimulation(simulationEntity.Id, networkId, newSimulationName);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(clonedSimulationId);
            Assert.Equal(newSimulationName, clonedSimulation.Name);
            Assert.Equal(networkId, clonedSimulation.NetworkId);
            Assert.Equal("Test Network", clonedSimulation.NetworkName);
            var allAttributes = TestHelper.UnitOfWork.AttributeRepo.GetAttributes();
            var ageAttribute = allAttributes.Single(a => a.Name == TestAttributeNames.Age);
            var clonedAttribute = TestHelper.UnitOfWork.CalculatedAttributeRepo.GetScenarioCalulatedAttributesByScenarioAndAttributeId(clonedSimulationId, ageAttribute.Id);
            ObjectAssertions.EquivalentExcluding(calculatedAttribute, clonedAttribute, c => c.Id, c => c.Equations[0].Id, c => c.Equations[0].Equation.Id, c => c.Equations[0].CriteriaLibrary.Id);
            Assert.NotEqual(calculatedAttribute.Id, clonedAttribute.Id);
            Assert.NotEqual(calculatedAttribute.Equations[0].Id, clonedAttribute.Equations[0].Id);
            Assert.NotEqual(calculatedAttribute.Equations[0].Equation.Id, clonedAttribute.Equations[0].Equation.Id);
            Assert.NotEqual(calculatedAttribute.Equations[0].CriteriaLibrary.Id, clonedAttribute.Equations[0].CriteriaLibrary.Id);
        }

        [Fact]
        public void SimulationInDbWithBudgetWithAmount_Clone_Clones()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var networkId = Guid.NewGuid();
            var keyAttributeId = TestAttributeIds.BrKeyId;

            var network = NetworkTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, new List<Data.Networking.MaintainableAsset>(), networkId, keyAttributeId);
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            var budgetId = Guid.NewGuid();
            var budget = BudgetDtos.WithSingleAmount(budgetId, "budget", 2023, 4321);
            var budgets = new List<BudgetDTO> { budget };
            var amount = budget.BudgetAmounts.Single();
            ScenarioBudgetTestSetup.UpsertOrDeleteScenarioBudgets(TestHelper.UnitOfWork, budgets, simulationId);
            var scenarioBudgets = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(simulationId);
            var scenarioBudgetsId = scenarioBudgets[0].Id;
            var sectionCommittedProjectId = Guid.NewGuid();
            var sectionCommittedProject = TestDataForCommittedProjects.SimpleSectionCommittedProjectDTO(sectionCommittedProjectId, simulationId, 2023, scenarioBudgetsId);
            var sectionCommittedProjects = new List<SectionCommittedProjectDTO>
            {
                sectionCommittedProject
            };
            TestHelper.UnitOfWork.CommittedProjectRepo.UpsertCommittedProjects(sectionCommittedProjects);

            var cloningResult = TestHelper.UnitOfWork.SimulationRepo.CloneSimulation(simulationEntity.Id, networkId, newSimulationName);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(clonedSimulationId);
            Assert.Equal(newSimulationName, clonedSimulation.Name);
            Assert.Equal(networkId, clonedSimulation.NetworkId);
            Assert.Equal(network.Name, clonedSimulation.NetworkName);

            var clonedProjects = TestHelper.UnitOfWork.CommittedProjectRepo.GetSectionCommittedProjectDTOs(clonedSimulationId);
            var clonedProject = clonedProjects.Single();
            ObjectAssertions.EquivalentExcluding(sectionCommittedProject, clonedProject, x => x.SimulationId, cp => cp.ScenarioBudgetId, cp => cp.Consequences, cp => cp.Id, cp => cp.LocationKeys);
            Assert.NotEqual(sectionCommittedProjectId, clonedProject.Id);
            var clonedBudgets = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(cloningResult.Simulation.Id);
            var clonedBudget = clonedBudgets.Single();
            var clonedAmount = clonedBudget.BudgetAmounts.Single();
            Assert.Equal(clonedBudget.Id, clonedProject.ScenarioBudgetId);
            ObjectAssertions.EquivalentExcluding(amount, clonedAmount, x => x.Id);
            Assert.NotEqual(amount.Id, clonedAmount.Id);
            ObjectAssertions.EquivalentExcluding(budget, clonedBudget, b => b.Id, b => b.BudgetAmounts, b => b.CriterionLibrary);
            var expectedCriterionLibrary = new CriterionLibraryDTO();
            ObjectAssertions.Equivalent(expectedCriterionLibrary, clonedBudget.CriterionLibrary);
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
        public void SimulationInDbWithScenarioBudgetWithCriterionLibrary_Clone_Clones()
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
            budget.CriterionLibrary = CriterionLibraryDtos.Dto();
            var budgets = new List<BudgetDTO> { budget };
            ScenarioBudgetTestSetup.UpsertOrDeleteScenarioBudgets(TestHelper.UnitOfWork, budgets, simulationId);

            var cloningResult = TestHelper.UnitOfWork.SimulationRepo.CloneSimulation(simulationEntity.Id, networkId, newSimulationName);

            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(cloningResult.Simulation.Id);
            var clonedBudgets = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(cloningResult.Simulation.Id);
            var clonedBudget = clonedBudgets.Single();
            var originalLibrary = budget.CriterionLibrary;
            var clonedLibrary = clonedBudget.CriterionLibrary;
            Assert.NotEqual(originalLibrary.Id, clonedLibrary.Id);
            Assert.True(clonedLibrary.IsSingleUse);

            ObjectAssertions.EquivalentExcluding(budget, clonedBudget, b => b.Id, b => b.CriterionLibrary, b => b.BudgetAmounts[0].Id);
        }


        [Fact]
        public void SimulationInDbWithBudgetPriority_Clone_Clones()
        {
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            var budgetPriority = BudgetPriorityTestSetup.SetupSingleBudgetPriorityForSimulationInDb(simulationId);

            var cloningResult = TestHelper.UnitOfWork.SimulationRepo.CloneSimulation(simulationEntity.Id, networkId, newSimulationName);

            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(cloningResult.Simulation.Id);
            var clonedPriorities = TestHelper.UnitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(clonedSimulation.Id);
            var clonedPriority = clonedPriorities.Single();
            ObjectAssertions.EquivalentExcluding(budgetPriority, clonedPriority, bp => bp.Id, bp => bp.CriterionLibrary);
        }


        [Fact]
        public void SimulationInDbWithBudgetPriorityWithCriterionLibrary_Clone_Clones()
        {
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            var budgetPriority = BudgetPriorityTestSetup.SetupSingleBudgetPriorityWithCriterionLibraryForSimulationInDb(simulationId);

            var cloningResult = TestHelper.UnitOfWork.SimulationRepo.CloneSimulation(simulationEntity.Id, networkId, newSimulationName);

            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(cloningResult.Simulation.Id);
            var clonedPriorities = TestHelper.UnitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(clonedSimulation.Id);
            var clonedPriority = clonedPriorities.Single();
            ObjectAssertions.EquivalentExcluding(budgetPriority, clonedPriority, bp => bp.Id, bp => bp.CriterionLibrary);
            var clonedLibrary = clonedPriority.CriterionLibrary;
            Assert.True(clonedLibrary.IsSingleUse);
            Assert.Equal("mergedCriteriaExpression", clonedLibrary.MergedCriteriaExpression);
            Assert.Empty(clonedLibrary.AppliedScenarioIds);
        }


        [Fact]
        public void SimulationInDbWithCashFlowRule_Clone_Clones()
        {
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
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

        [Fact]
        public void SimulationInDbWithInvestmentPlan_Clone_Clones()
        {
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var investmentPlanId = Guid.NewGuid();
            var investmentPlan = InvestmentPlanDtos.Dto(investmentPlanId);
            TestHelper.UnitOfWork.InvestmentPlanRepo.UpsertInvestmentPlan(investmentPlan, simulationId);
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);

            var cloningResult = TestHelper.UnitOfWork.SimulationRepo.CloneSimulation(simulationEntity.Id, networkId, newSimulationName);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedInvestmentPlan = TestHelper.UnitOfWork.InvestmentPlanRepo.GetInvestmentPlan(clonedSimulationId);
            ObjectAssertions.EquivalentExcluding(investmentPlan, clonedInvestmentPlan,
                i => i.Id);
        }

        [Fact]
        public void SimulationInDbWithPerformanceCurve_Clone_Clones()
        {
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            var performanceCurveId = Guid.NewGuid();
            var attributeName = TestAttributeNames.DeckDurationN;
            var equationId = Guid.NewGuid();
            var performanceCurve = PerformanceCurveDtos.Dto(performanceCurveId, null, attributeName);
            performanceCurve.Equation.Id = equationId;
            var performanceCurves = new List<PerformanceCurveDTO> { performanceCurve };
            var criterionLibrary = CriterionLibraryDtos.Dto();
            performanceCurve.CriterionLibrary = criterionLibrary;
            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeleteScenarioPerformanceCurves(performanceCurves, simulationId);
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);

            var cloningResult = TestHelper.UnitOfWork.SimulationRepo.CloneSimulation(simulationEntity.Id, networkId, newSimulationName);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedPerformanceCurves = TestHelper.UnitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(clonedSimulationId);
            var clonedPerformanceCurve = clonedPerformanceCurves.Single();
            ObjectAssertions.EquivalentExcluding(performanceCurve, clonedPerformanceCurve,
                x => x.Id, x => x.Equation.Id, x => x.CriterionLibrary.Id, x => x.CriterionLibrary.Name,
                x => x.CriterionLibrary.Owner);
        }

        [Fact]
        public void SimulationInDbWithBadData_Clone_HitsErrorCatchingCode()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var networkId = Guid.NewGuid();
            var keyAttributeId = TestAttributeIds.BrKeyId;

            var network = NetworkTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, new List<Data.Networking.MaintainableAsset>(), networkId, keyAttributeId);
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationEntity2 = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            var budgetId = Guid.NewGuid();
            var budget = BudgetDtos.WithSingleAmount(budgetId, "budget", 2023, 4321);
            var budgets = new List<BudgetDTO> { budget };
            var amount = budget.BudgetAmounts.Single();
            ScenarioBudgetTestSetup.UpsertOrDeleteScenarioBudgets(TestHelper.UnitOfWork, budgets, simulationId);
            var scenarioBudgets = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(simulationId);
            var scenarioBudgetsId = scenarioBudgets[0].Id;
            var sectionCommittedProjectId = Guid.NewGuid();
            var sectionCommittedProject = TestDataForCommittedProjects.SimpleSectionCommittedProjectDTO(sectionCommittedProjectId, simulationId, 2023, scenarioBudgetsId);
            var sectionCommittedProjects = new List<SectionCommittedProjectDTO>
            {
                sectionCommittedProject
            };
            TestHelper.UnitOfWork.CommittedProjectRepo.UpsertCommittedProjects(sectionCommittedProjects);
            var rogueBudgetId = Guid.NewGuid();
            var rogueBudget = new ScenarioBudgetEntity
            {
                Id = rogueBudgetId,
                SimulationId = simulationEntity2.Id,
                Name = "Budget in the wrong simulation",
            };
            TestHelper.UnitOfWork.Context.Add(rogueBudget);
            var committedProject = TestHelper.UnitOfWork.Context.CommittedProject.Single(cp => cp.SimulationId == simulation.Id);
            committedProject.ScenarioBudgetId = rogueBudgetId;
            TestHelper.UnitOfWork.Context.Update(committedProject);
            TestHelper.UnitOfWork.Context.SaveChanges();

            var cloningResult = TestHelper.UnitOfWork.SimulationRepo.CloneSimulation(simulationEntity.Id, networkId, newSimulationName);

            var warningMessage = cloningResult.WarningMessage;
            Assert.Contains("Budget in the wrong simulation", warningMessage);
        }
    }
}
