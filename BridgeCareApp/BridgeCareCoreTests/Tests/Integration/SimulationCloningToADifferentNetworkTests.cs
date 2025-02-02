﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes.CalculatedAttributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.BudgetPriority;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.CashFlowRule;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.User;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.SimulationCloning;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore;
using BridgeCareCore.Services;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.SelectableTreatment;
using AppliedResearchAssociates.iAM.DataUnitTests.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.TreatmentSupersedeRule;

namespace BridgeCareCoreTests.Tests.Integration
{
    public class SimulationCloningToADifferentNetworkTests
    {
        private static ICompleteSimulationCloningService CreateCompleteSimulationCloningService()
        {
            var service = new CompleteSimulationCloningService(TestHelper.UnitOfWork);
            return service;
        }

        [Fact]
        public void CloningToADifferentScenario()
        {
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var destinationNetworkId = SimulationCloningTestSetup.TestDestinationNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, destinationNetworkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);

            var cloneSimulationDto = CloneSimulationDtos.Create(simulationId, networkId, newSimulationName);
            cloneSimulationDto.DestinationNetworkId = destinationNetworkId;
            var cloningService = CreateCompleteSimulationCloningService();
            var cloningResult = cloningService.Clone(cloneSimulationDto);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(clonedSimulationId);
            Assert.Equal(newSimulationName, clonedSimulation.Name);
            Assert.Equal(destinationNetworkId, clonedSimulation.NetworkId);
            Assert.Equal(simulation.NetworkName, clonedSimulation.NetworkName);
        }

        [Fact]
        public void CloningToADifferentScenarioWithTreatmentCosts()
        {
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var destinationNetworkId = SimulationCloningTestSetup.TestDestinationNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, destinationNetworkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            var treatmentbudget = TreatmentBudgetDtos.Dto();
            var treatmentId = Guid.NewGuid();
            var treatment = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists(treatmentId);
            treatment.Budgets = new List<TreatmentBudgetDTO>() { treatmentbudget };
            treatment.BudgetIds = new List<Guid> { treatmentbudget.Id };
            var treatments = new List<TreatmentDTO> { treatment };
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(treatments, simulation.Id);
            var scenarioTreatmentCostId = Guid.NewGuid();
            var scenarioTreatmentCost = TreatmentCostDtos.WithEquationAndCriterionLibrary(scenarioTreatmentCostId);
            var scenarioTreatmentCosts = new List<TreatmentCostDTO>() { scenarioTreatmentCost };
            var scenarioTreatmentCostPerTreatmentId = new Dictionary<Guid, List<TreatmentCostDTO>>();
            scenarioTreatmentCostPerTreatmentId[treatmentId] = scenarioTreatmentCosts;

            TestHelper.UnitOfWork.TreatmentCostRepo.UpsertOrDeleteScenarioTreatmentCosts(scenarioTreatmentCostPerTreatmentId, simulationId);

            var cloneSimulationDto = CloneSimulationDtos.Create(simulationId, networkId, newSimulationName);
            cloneSimulationDto.DestinationNetworkId = destinationNetworkId;
            var cloningService = CreateCompleteSimulationCloningService();
            var cloningResult = cloningService.Clone(cloneSimulationDto);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(clonedSimulationId);
            var clonedTreatments = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(clonedSimulationId);
            var clonedTreatment = clonedTreatments.Single();

            Assert.Equal(destinationNetworkId, clonedSimulation.NetworkId);
            Assert.Equal(simulation.NetworkName, clonedSimulation.NetworkName);
            Assert.NotEqual(treatment.Id, clonedTreatment.Id);
            Assert.Empty(clonedTreatment.Budgets);
            Assert.Empty(clonedTreatment.BudgetIds);
            ObjectAssertions.EquivalentExcluding(treatment, clonedTreatment, t => t.Id, t => t.CriterionLibrary, t => t.Costs, t => t.Budgets, t => t.BudgetIds, t => t.SupersedeRules);
            Assert.NotEqual(treatment.Id, clonedTreatment.Id);
            SimulationCloningCriterionLibraryDtoAssertions.AssertValidLibraryClone(scenarioTreatmentCost.CriterionLibrary, clonedTreatment.Costs[0].CriterionLibrary, TestHelper.UnitOfWork.UserEntity?.Id);
        }

        [Fact]
        public void CloningToADifferentScenarioWithSelectableTreatments()
        {
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var destinationNetworkId = SimulationCloningTestSetup.TestDestinationNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, destinationNetworkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            var treatmentbudget = TreatmentBudgetDtos.Dto();
            var treatmentId = Guid.NewGuid();
            var treatment = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists(treatmentId);
            treatment.Budgets = new List<TreatmentBudgetDTO>() { treatmentbudget };
            treatment.BudgetIds = new List<Guid> { treatmentbudget.Id };
            var treatments = new List<TreatmentDTO> { treatment };
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(treatments, simulation.Id);

            var cloneSimulationDto = CloneSimulationDtos.Create(simulationId, networkId, newSimulationName);
            cloneSimulationDto.DestinationNetworkId = destinationNetworkId;
            var cloningService = CreateCompleteSimulationCloningService();
            var cloningResult = cloningService.Clone(cloneSimulationDto);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(clonedSimulationId);
            var clonedTreatments = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(clonedSimulationId);
            var clonedTreatment = clonedTreatments.Single();
            var expectedCriterionLibrary = new CriterionLibraryDTO();
            ObjectAssertions.EquivalentExcluding(treatment, clonedTreatment, t => t.Id, t => t.CriterionLibrary, t => t.Budgets, t => t.BudgetIds, t => t.SupersedeRules);
            Assert.Equal(newSimulationName, clonedSimulation.Name);
            Assert.Equal(destinationNetworkId, clonedSimulation.NetworkId);
            Assert.Equal(simulation.NetworkName, clonedSimulation.NetworkName);
            Assert.NotEqual(treatment.Id, clonedTreatment.Id);
            Assert.Empty(clonedTreatment.Budgets);
            Assert.Empty(clonedTreatment.BudgetIds);
            ObjectAssertions.Equivalent(expectedCriterionLibrary, clonedTreatment.CriterionLibrary);
        }

        [Fact]
        public void CloningToADifferentScenarioWithCriterionLibrary()
        {
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var destinationNetworkId = SimulationCloningTestSetup.TestDestinationNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, destinationNetworkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            var treatmentbudget = TreatmentBudgetDtos.Dto();
            var treatmentId = Guid.NewGuid();
            var treatment = TreatmentDtos.DtoWithEmptyCostsAndConsequencesListsWithCriterionLibrary(treatmentId);
            treatment.Budgets = new List<TreatmentBudgetDTO>() { treatmentbudget };
            treatment.BudgetIds = new List<Guid> { treatmentbudget.Id };
            var treatments = new List<TreatmentDTO> { treatment };
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(treatments, simulation.Id);
            var treatmentsBefore = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulationId);
            var treatmentBefore = treatmentsBefore.Single();

            var cloneSimulationDto = CloneSimulationDtos.Create(simulationId, networkId, newSimulationName);
            cloneSimulationDto.DestinationNetworkId = destinationNetworkId;
            var cloningService = CreateCompleteSimulationCloningService();
            var cloningResult = cloningService.Clone(cloneSimulationDto);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(clonedSimulationId);
            var clonedTreatments = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(clonedSimulationId);
            var clonedTreatment = clonedTreatments.Single();
            var expectedCriterionLibrary = new CriterionLibraryDTO();
            ObjectAssertions.EquivalentExcluding(treatment, clonedTreatment, t => t.Id, t => t.CriterionLibrary, t => t.Budgets, t => t.BudgetIds, t => t.SupersedeRules);
            Assert.Equal(newSimulationName, clonedSimulation.Name);
            Assert.Equal(destinationNetworkId, clonedSimulation.NetworkId);
            Assert.Equal(simulation.NetworkName, clonedSimulation.NetworkName);
            Assert.NotEqual(treatment.Id, clonedTreatment.Id);
            Assert.Empty(clonedTreatment.Budgets);
            Assert.Empty(clonedTreatment.BudgetIds);
            Assert.NotEqual(treatmentBefore.Id, clonedTreatment.Id);
            SimulationCloningCriterionLibraryDtoAssertions.AssertValidLibraryClone(treatmentBefore.CriterionLibrary, clonedTreatment.CriterionLibrary, TestHelper.UnitOfWork.UserEntity?.Id);
        }

        [Fact]
        public void CloningToADifferentScenarioWithSelectableTreatmentWithConsequences()
        {
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var destinationNetworkId = SimulationCloningTestSetup.TestDestinationNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, destinationNetworkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            var treatmentbudget = TreatmentBudgetDtos.Dto();
            var treatmentId = Guid.NewGuid();
            var treatment = TreatmentDtos.DtoWithEmptyCostsAndConsequencesListsWithCriterionLibrary(treatmentId);
            treatment.Budgets = new List<TreatmentBudgetDTO>() { treatmentbudget };
            treatment.BudgetIds = new List<Guid> { treatmentbudget.Id };
            var treatments = new List<TreatmentDTO> { treatment };
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(treatments, simulation.Id);

            var treatmentConsequenceId = Guid.NewGuid();
            var treatmentConsequence = TreatmentConsequenceDtos.WithEquationAndCriterionLibrary(treatmentConsequenceId, TestAttributeNames.CulvDurationN);
            var treatmentConsequences = new List<TreatmentConsequenceDTO>() { treatmentConsequence };
            var treatmentConsequencesPerTreatmentId = new Dictionary<Guid, List<TreatmentConsequenceDTO>>();
            treatmentConsequencesPerTreatmentId[treatmentId] = treatmentConsequences;

            TestHelper.UnitOfWork.TreatmentConsequenceRepo.UpsertOrDeleteScenarioTreatmentConsequences(treatmentConsequencesPerTreatmentId, simulationId);

            var cloneSimulationDto = CloneSimulationDtos.Create(simulationId, networkId, newSimulationName);
            cloneSimulationDto.DestinationNetworkId = destinationNetworkId;
            var cloningService = CreateCompleteSimulationCloningService();
            var cloningResult = cloningService.Clone(cloneSimulationDto);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(clonedSimulationId);
            var clonedTreatments = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(clonedSimulationId);
            var clonedTreatment = clonedTreatments.Single();

            Assert.Equal(newSimulationName, clonedSimulation.Name);
            Assert.Equal(destinationNetworkId, clonedSimulation.NetworkId);
            Assert.Equal(simulation.NetworkName, clonedSimulation.NetworkName);
            Assert.NotEqual(treatment.Id, clonedTreatment.Id);
            Assert.Empty(clonedTreatment.Budgets);
            Assert.Empty(clonedTreatment.BudgetIds);
            ObjectAssertions.EquivalentExcluding(treatment, clonedTreatment, t => t.Id, t => t.Consequences, t => t.CriterionLibrary, t => t.BudgetIds, t => t.Budgets, t => t.SupersedeRules);
            SimulationCloningCriterionLibraryDtoAssertions.AssertValidLibraryClone(treatment.CriterionLibrary, clonedTreatment.CriterionLibrary, TestHelper.UnitOfWork.UserEntity?.Id);
            var clonedConsequence = clonedTreatment.Consequences.Single();
            ObjectAssertions.EquivalentExcluding(treatmentConsequence, clonedConsequence, c => c.Id, c => c.CriterionLibrary, c => c.Equation.Id);
            SimulationCloningCriterionLibraryDtoAssertions.AssertValidLibraryClone(treatmentConsequence.CriterionLibrary, clonedConsequence.CriterionLibrary, TestHelper.UnitOfWork.UserEntity?.Id);
            Assert.NotEqual(treatmentConsequence.Id, clonedConsequence.Id);
        }

        [Fact]
        public void CloningToADifferentScenarioWithSelectableTreatmentWithSupersedeRules()
        {
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var destinationNetworkId = SimulationCloningTestSetup.TestDestinationNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, destinationNetworkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);

            var treatmentbudget = TreatmentBudgetDtos.Dto();
            var treatmentId = Guid.NewGuid();
            var treatment = TreatmentDtos.DtoWithEmptyCostsAndConsequencesListsWithCriterionLibrary(treatmentId);
            treatment.Budgets = new List<TreatmentBudgetDTO>() { treatmentbudget };
            treatment.BudgetIds = new List<Guid> { treatmentbudget.Id };

            var preventTreatmentId = Guid.NewGuid();
            var preventTreatment = TreatmentDtos.DtoWithEmptyCostsAndConsequencesListsWithCriterionLibrary(preventTreatmentId);
            var treatmentbudget1 = TreatmentBudgetDtos.Dto();
            preventTreatment.Name = "Prevent Treatment Name";
            preventTreatment.Budgets = new List<TreatmentBudgetDTO>() { treatmentbudget1 };
            preventTreatment.BudgetIds = new List<Guid> { treatmentbudget1.Id };

            var treatments = new List<TreatmentDTO> { treatment, preventTreatment };
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(treatments, simulation.Id);

            var supersedeRuleId = Guid.NewGuid();
            var treatmentSuperdedeRule = TreatmentSupersedeRuleTestSetup.TreatmentSuperdedeRuleDto;
            treatmentSuperdedeRule.Id = supersedeRuleId;
            treatmentSuperdedeRule.treatment = preventTreatment;
            var treatmentWithSupersedeRules = new List<TreatmentSupersedeRuleDTO>() { treatmentSuperdedeRule };
            var treatmentWithSupersedeRulesPerTreatmentId = new Dictionary<Guid, List<TreatmentSupersedeRuleDTO>>
            {
                [treatmentId] = treatmentWithSupersedeRules
            };

            TestHelper.UnitOfWork.TreatmentSupersedeRuleRepo.UpsertOrDeleteScenarioTreatmentSupersedeRules(treatmentWithSupersedeRulesPerTreatmentId, simulationId);

            var cloneSimulationDto = CloneSimulationDtos.Create(simulationId, networkId, newSimulationName);
            cloneSimulationDto.DestinationNetworkId = destinationNetworkId;
            var cloningService = CreateCompleteSimulationCloningService();
            var cloningResult = cloningService.Clone(cloneSimulationDto);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(clonedSimulationId);
            var clonedTreatments = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(clonedSimulationId);
            var clonedTreatment1 = clonedTreatments.FirstOrDefault(_ => _.Name.Equals("Treatment name"));

            Assert.Equal(newSimulationName, clonedSimulation.Name);
            Assert.Equal(destinationNetworkId, clonedSimulation.NetworkId);
            Assert.Equal(simulation.NetworkName, clonedSimulation.NetworkName);
            Assert.Equal(2, clonedTreatments.Count);
            Assert.True(clonedTreatment1.SupersedeRules.Count == 1);
            Assert.NotEqual(treatment.Id, clonedTreatment1.Id);
            Assert.Empty(clonedTreatment1.Budgets);
            Assert.Empty(clonedTreatment1.BudgetIds);
            Assert.Empty(clonedTreatment1.Consequences);
            Assert.Empty(clonedTreatment1.PerformanceFactors);
            ObjectAssertions.EquivalentExcluding(treatment, clonedTreatment1, t => t.Id, t => t.Consequences, t => t.CriterionLibrary, t => t.BudgetIds, t => t.Budgets, t => t.SupersedeRules);
            var clonedSupersedeRule = clonedTreatment1.SupersedeRules.Single();
            ObjectAssertions.EquivalentExcluding(treatmentSuperdedeRule, clonedSupersedeRule, _ => _.Id, _ => _.CriterionLibrary, _ => _.treatment.CriterionLibrary, _ => _.treatment.BudgetIds, t => t.treatment.Budgets, t => t.treatment.SupersedeRules, t => t.treatment.Id);
            SimulationCloningCriterionLibraryDtoAssertions.AssertValidLibraryClone(treatmentSuperdedeRule.CriterionLibrary, clonedSupersedeRule.CriterionLibrary, TestHelper.UnitOfWork.UserEntity?.Id);
            Assert.NotEqual(treatmentSuperdedeRule.Id, clonedSupersedeRule.Id);
        }

        [Fact]
        public void CloningToADifferentScenarioWithRemainingLifeLimit()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var limitId = Guid.NewGuid();
            var limit = RemainingLifeLimitDtos.Dto(TestAttributeNames.CulvDurationN, limitId, 1);
            var limits = new List<RemainingLifeLimitDTO> { limit };

            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var destinationNetworkId = SimulationCloningTestSetup.TestDestinationNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, destinationNetworkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);

            TestHelper.UnitOfWork.RemainingLifeLimitRepo.UpsertOrDeleteScenarioRemainingLifeLimits(limits, simulationId);

            var cloneSimulationDto = CloneSimulationDtos.Create(simulationId, networkId, newSimulationName);
            cloneSimulationDto.DestinationNetworkId = destinationNetworkId;
            var cloningService = CreateCompleteSimulationCloningService();
            var cloningResult = cloningService.Clone(cloneSimulationDto);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(clonedSimulationId);
            var clonedLifeLimits = TestHelper.UnitOfWork.RemainingLifeLimitRepo.GetScenarioRemainingLifeLimits(clonedSimulationId);
            var clonedLifeLimit = clonedLifeLimits.Single();
            ObjectAssertions.EquivalentExcluding(limit, clonedLifeLimit, c => c.CriterionLibrary, c => c.Id);
            Assert.Equal(newSimulationName, clonedSimulation.Name);
            Assert.Equal(destinationNetworkId, clonedSimulation.NetworkId);
            Assert.Equal(simulation.NetworkName, clonedSimulation.NetworkName);
            var expectedCriterionLibrary = new CriterionLibraryDTO();
            ObjectAssertions.Equivalent(expectedCriterionLibrary, clonedLifeLimit.CriterionLibrary);
            Assert.NotEqual(limit.Id, clonedLifeLimit.Id);
        }

        [Fact]
        public void CloningToADifferentScenarioWithRemainingLifeLimitWithCriterionLibrary()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var limitId = Guid.NewGuid();
            var limit = RemainingLifeLimitDtos.DtoWithCriterionLibrary(TestAttributeNames.CulvDurationN, limitId, 1);
            var limits = new List<RemainingLifeLimitDTO> { limit };
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var destinationNetworkId = SimulationCloningTestSetup.TestDestinationNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, destinationNetworkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            TestHelper.UnitOfWork.RemainingLifeLimitRepo.UpsertOrDeleteScenarioRemainingLifeLimits(limits, simulationId);
            var lifeLimitsBefore = TestHelper.UnitOfWork.RemainingLifeLimitRepo.GetScenarioRemainingLifeLimits(simulationId);
            var lifeLimitBefore = lifeLimitsBefore.Single();

            var cloneSimulationDto = CloneSimulationDtos.Create(simulationId, networkId, newSimulationName);
            cloneSimulationDto.DestinationNetworkId = destinationNetworkId;
            var cloningService = CreateCompleteSimulationCloningService();
            var cloningResult = cloningService.Clone(cloneSimulationDto);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(clonedSimulationId);
            var clonedLifeLimits = TestHelper.UnitOfWork.RemainingLifeLimitRepo.GetScenarioRemainingLifeLimits(clonedSimulationId);
            var clonedLifeLimit = clonedLifeLimits.Single();
            ObjectAssertions.EquivalentExcluding(lifeLimitBefore, clonedLifeLimit, c => c.CriterionLibrary, c => c.Id);
            Assert.Equal(newSimulationName, clonedSimulation.Name);
            Assert.Equal(destinationNetworkId, clonedSimulation.NetworkId);
            Assert.Equal(simulation.NetworkName, clonedSimulation.NetworkName);
            ObjectAssertions.EquivalentExcluding(lifeLimitBefore.CriterionLibrary, clonedLifeLimit.CriterionLibrary, c => c.Id, c => c.MergedCriteriaExpression, c => c.IsSingleUse, c => c.Name);
            Assert.NotEqual(Guid.Empty, clonedLifeLimit.CriterionLibrary.Id);
            Assert.NotEqual(lifeLimitBefore.Id, clonedLifeLimit.Id);
            Assert.NotEqual(lifeLimitBefore.CriterionLibrary.Id, clonedLifeLimit.CriterionLibrary.Id);
        }

        [Fact]
        public void CloningToADifferentScenarioWithScenarioDeficientConditionGoals()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var deficientconditiongoalId = Guid.NewGuid();
            var deficientconditiongoal = DeficientConditionGoalDtos.DtoWithIdOnlyCriterionLibrary(deficientconditiongoalId, TestAttributeNames.CulvDurationN);
            var deficientconditiongoals = new List<DeficientConditionGoalDTO> { deficientconditiongoal };

            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var destinationNetworkId = SimulationCloningTestSetup.TestDestinationNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, destinationNetworkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);

            TestHelper.UnitOfWork.DeficientConditionGoalRepo.UpsertOrDeleteScenarioDeficientConditionGoals(deficientconditiongoals, simulationId);

            var cloneSimulationDto = CloneSimulationDtos.Create(simulationId, networkId, newSimulationName);
            cloneSimulationDto.DestinationNetworkId = destinationNetworkId;
            var cloningService = CreateCompleteSimulationCloningService();
            var cloningResult = cloningService.Clone(cloneSimulationDto);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(clonedSimulationId);
            var clonedDeficientConditionGoals = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(clonedSimulationId);
            var clonedDeficientConditionGoal = clonedDeficientConditionGoals.Single();
            ObjectAssertions.EquivalentExcluding(deficientconditiongoal, clonedDeficientConditionGoal, c => c.CriterionLibrary, c => c.Id);
            Assert.Equal(newSimulationName, clonedSimulation.Name);
            Assert.Equal(destinationNetworkId, clonedSimulation.NetworkId);
            Assert.Equal(simulation.NetworkName, clonedSimulation.NetworkName);
            Assert.Equal(deficientconditiongoal.DeficientLimit, clonedDeficientConditionGoal.DeficientLimit);
            Assert.Equal(deficientconditiongoal.Name, clonedDeficientConditionGoal.Name);
            var expectedCriterionLibrary = new CriterionLibraryDTO();
            ObjectAssertions.Equivalent(expectedCriterionLibrary, clonedDeficientConditionGoal.CriterionLibrary);
            Assert.NotEqual(deficientconditiongoal.Id, clonedDeficientConditionGoal.Id);
        }

        [Fact]
        public async Task CloningToADifferentScenarioWithScenarioDeficientConditionGoalsWithCriterionLibrary()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, false);
            TestHelper.UnitOfWork.SetUser(user.Username);

            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var deficientconditiongoalId = Guid.NewGuid();
            var deficientconditiongoal = DeficientConditionGoalDtos.DtoWithCriterionLibrary(deficientconditiongoalId, TestAttributeNames.CulvDurationN);
            var deficientconditiongoals = new List<DeficientConditionGoalDTO> { deficientconditiongoal };
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var destinationNetworkId = SimulationCloningTestSetup.TestDestinationNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, destinationNetworkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            TestHelper.UnitOfWork.DeficientConditionGoalRepo.UpsertOrDeleteScenarioDeficientConditionGoals(deficientconditiongoals, simulationId);
            var deficientConditionGoalsBefore = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(simulationId);
            var deficientConditionGoalBefore = deficientConditionGoalsBefore.Single();

            var cloneSimulationDto = CloneSimulationDtos.Create(simulationId, networkId, newSimulationName);
            cloneSimulationDto.DestinationNetworkId = destinationNetworkId;
            var cloningService = CreateCompleteSimulationCloningService();
            var cloningResult = cloningService.Clone(cloneSimulationDto);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(clonedSimulationId);
            var clonedDeficientConditionGoals = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(clonedSimulationId);
            var clonedDeficientConditionGoal = clonedDeficientConditionGoals.Single();
            ObjectAssertions.EquivalentExcluding(deficientConditionGoalBefore, clonedDeficientConditionGoal, c => c.CriterionLibrary, c => c.Id);
            Assert.Equal(newSimulationName, clonedSimulation.Name);
            Assert.Equal(destinationNetworkId, clonedSimulation.NetworkId);
            Assert.Equal(simulation.NetworkName, clonedSimulation.NetworkName);
            ObjectAssertions.EquivalentExcluding(deficientConditionGoalBefore.CriterionLibrary, clonedDeficientConditionGoal.CriterionLibrary, c => c.Id, c => c.MergedCriteriaExpression, c => c.IsSingleUse, c => c.Name);
            Assert.NotEqual(deficientConditionGoalBefore.Id, clonedDeficientConditionGoal.Id);
            Assert.NotEqual(deficientConditionGoalBefore.CriterionLibrary.Id, clonedDeficientConditionGoal.CriterionLibrary.Id);
        }

        [Fact]
        public void CloningToADifferentScenarioWithAnalysisMethodInCriterionLibrary()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var networkEntity = NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var networkId = networkEntity.Id;
            var destinationNetworkId = SimulationCloningTestSetup.TestDestinationNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, destinationNetworkId);
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
                Limit = 1.0,
                Attribute = TestAttributeNames.CulvDurationN,
            };
            analysisMethodDto.CriterionLibrary = CriterionLibraryDtos.Dto();
            var budgetPriority = BudgetPriorityTestSetup.SetupSingleBudgetPriorityForSimulationInDb(simulationId);
            TestHelper.UnitOfWork.AnalysisMethodRepo.UpsertAnalysisMethod(simulationId, analysisMethodDto);
            TestHelper.UnitOfWork.AnalysisMethodRepo.GetSimulationAnalysisMethod(simulation, "");
            var newSimulationName = RandomStrings.WithPrefix("cloned");

            var cloneSimulationDto = CloneSimulationDtos.Create(simulationId, networkId, newSimulationName);
            cloneSimulationDto.DestinationNetworkId = destinationNetworkId;
            var cloningService = CreateCompleteSimulationCloningService();
            var cloningResult = cloningService.Clone(cloneSimulationDto);

            var clonedSimulation = cloningResult.Simulation;
            var clonedAnalysisMethod = TestHelper.UnitOfWork.AnalysisMethodRepo.GetAnalysisMethod(clonedSimulation.Id);
            var clonedBenefit = clonedAnalysisMethod.Benefit;
            ObjectAssertions.EquivalentExcluding(analysisMethodDto.Benefit, clonedBenefit, b => b.Id);
            Assert.NotEqual(analysisMethodDto.Benefit.Id, clonedBenefit.Id);
            Assert.NotEqual(analysisMethodDto.Id, clonedAnalysisMethod.Id);
            Assert.NotEqual(analysisMethodDto.CriterionLibrary.Id, clonedAnalysisMethod.CriterionLibrary.Id);
            Assert.Equal("mergedCriteriaExpression", clonedAnalysisMethod.CriterionLibrary.MergedCriteriaExpression);
        }

        [Fact]
        public void CloningToADifferentScenarioWithSelectableTreatmentWithBudget()
        {
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var destinationNetworkId = SimulationCloningTestSetup.TestDestinationNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, destinationNetworkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            var treatmentId = Guid.NewGuid();
            var treatment = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists(treatmentId);
            var budgetId = Guid.NewGuid();
            var budget = BudgetDtos.WithSingleAmount(budgetId, "budget", 2023, 4321);
            var budgets = new List<BudgetDTO> { budget };
            var amount = budget.BudgetAmounts.Single();
            ScenarioBudgetTestSetup.UpsertOrDeleteScenarioBudgets(TestHelper.UnitOfWork, budgets, simulationId);
            var scenarioBudgets = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(simulationId);
            var scenarioBudgetId = scenarioBudgets[0].Id;
            treatment.BudgetIds = new List<Guid> { scenarioBudgetId };
            var treatments = new List<TreatmentDTO> { treatment };
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(treatments, simulation.Id);
            var treatmentsBefore = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulationId);
            var treatmentBefore = treatmentsBefore.Single();

            var cloneSimulationDto = CloneSimulationDtos.Create(simulationId, networkId, newSimulationName);
            cloneSimulationDto.DestinationNetworkId = destinationNetworkId;
            var cloningService = CreateCompleteSimulationCloningService();
            var cloningResult = cloningService.Clone(cloneSimulationDto);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(clonedSimulationId);
            var clonedTreatments = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(clonedSimulationId);
            var clonedTreatment = clonedTreatments.Single();
            var expectedCriterionLibrary = new CriterionLibraryDTO();
            ObjectAssertions.EquivalentExcluding(treatment, clonedTreatment, t => t.Id, t => t.CriterionLibrary, t => t.Budgets, t => t.BudgetIds, t => t.SupersedeRules);
            Assert.Equal(newSimulationName, clonedSimulation.Name);
            Assert.Equal(destinationNetworkId, clonedSimulation.NetworkId);
            Assert.Equal(simulation.NetworkName, clonedSimulation.NetworkName);
            Assert.NotEqual(treatment.Id, clonedTreatment.Id);
            var budgetBefore = treatmentBefore.Budgets.Single();
            var clonedBudget = clonedTreatment.Budgets.Single();
            ObjectAssertions.EquivalentExcluding(budgetBefore, clonedBudget, b => b.Id);
            Assert.NotEqual(budgetBefore.Id, clonedBudget.Id);
            var clonedBudgetId = clonedTreatment.BudgetIds.Single();
            Assert.Equal(clonedBudgetId, clonedBudget.Id);
            ObjectAssertions.Equivalent(expectedCriterionLibrary, clonedTreatment.CriterionLibrary);
        }

        [Fact]
        public async Task ScenarioTargetConditionalGoals_Clone_Clones()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, false);
            TestHelper.UnitOfWork.SetUser(user.Username);

            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var targetconditionalgoalId = Guid.NewGuid();
            var targetconditionalgoal = TargetConditionGoalDtos.DtoWithCriterionLibrary(TestAttributeNames.CulvDurationN, targetconditionalgoalId);
            var targetconditionalgoals = new List<TargetConditionGoalDTO> { targetconditionalgoal };
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var destinationNetworkId = SimulationCloningTestSetup.TestDestinationNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, destinationNetworkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            TestHelper.UnitOfWork.TargetConditionGoalRepo.UpsertOrDeleteScenarioTargetConditionGoals(targetconditionalgoals, simulationId);
            var targetconditionalgoalsBefore = TestHelper.UnitOfWork.TargetConditionGoalRepo.GetScenarioTargetConditionGoals(simulationId);
            var targetconditionalgoalBefore = targetconditionalgoalsBefore.Single();

            var cloneSimulationDto = CloneSimulationDtos.Create(simulationId, networkId, newSimulationName);
            cloneSimulationDto.DestinationNetworkId = destinationNetworkId;
            var cloningService = CreateCompleteSimulationCloningService();
            var cloningResult = cloningService.Clone(cloneSimulationDto);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(clonedSimulationId);
            var clonedTargetConditionalGoals = TestHelper.UnitOfWork.TargetConditionGoalRepo.GetScenarioTargetConditionGoals(clonedSimulationId);
            var clonedTargetConditionalGoal = clonedTargetConditionalGoals.Single();
            ObjectAssertions.EquivalentExcluding(targetconditionalgoal, clonedTargetConditionalGoal, c => c.CriterionLibrary, c => c.Id);
            Assert.Equal(newSimulationName, clonedSimulation.Name);
            Assert.Equal(destinationNetworkId, clonedSimulation.NetworkId);
            Assert.Equal(simulation.NetworkName, clonedSimulation.NetworkName);
            ObjectAssertions.EquivalentExcluding(targetconditionalgoalBefore.CriterionLibrary, clonedTargetConditionalGoal.CriterionLibrary, c => c.Id, c => c.MergedCriteriaExpression, c => c.IsSingleUse, c => c.Name);
            Assert.NotEqual(targetconditionalgoalBefore.Id, clonedTargetConditionalGoal.Id);
            Assert.NotEqual(targetconditionalgoalBefore.CriterionLibrary.Id, clonedTargetConditionalGoal.CriterionLibrary.Id);
        }

        [Fact]
        public async Task SimulationInDb_Clone_JoinsCurrentUserToClone()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, false);
            TestHelper.UnitOfWork.SetUser(user.Username);
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var destinationNetworkId = SimulationCloningTestSetup.TestDestinationNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, destinationNetworkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);

            var cloneSimulationDto = CloneSimulationDtos.Create(simulationId, networkId, newSimulationName);
            cloneSimulationDto.DestinationNetworkId = destinationNetworkId;
            var cloningService = CreateCompleteSimulationCloningService();
            var cloningResult = cloningService.Clone(cloneSimulationDto);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(clonedSimulationId);
            Assert.Equal(newSimulationName, clonedSimulation.Name);
            Assert.Equal(destinationNetworkId, clonedSimulation.NetworkId);
            Assert.Equal(simulation.NetworkName, clonedSimulation.NetworkName);
            var clonedSimulationUser = clonedSimulation.Users.Single();
            Assert.Equal(user.Username, clonedSimulationUser.Username);
        }

        [Fact]
        public void SimulationInDbWithCalculatedAttribute_Clone_Clones()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);

            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var destinationNetworkId = SimulationCloningTestSetup.TestDestinationNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, destinationNetworkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            var calculatedAttributeId = Guid.NewGuid();
            var calculatedAttribute = CalculatedAttributeTestSetup.TestCalculatedAttributeDto(calculatedAttributeId, TestAttributeNames.Age);
            var calculatedAttributes = new List<CalculatedAttributeDTO> { calculatedAttribute };
            TestHelper.UnitOfWork.CalculatedAttributeRepo.UpsertScenarioCalculatedAttributes(calculatedAttributes, simulationId);

            var cloneSimulationDto = CloneSimulationDtos.Create(simulationId, networkId, newSimulationName);
            cloneSimulationDto.DestinationNetworkId = destinationNetworkId;
            var cloningService = CreateCompleteSimulationCloningService();
            var cloningResult = cloningService.Clone(cloneSimulationDto);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(clonedSimulationId);
            Assert.Equal(newSimulationName, clonedSimulation.Name);
            Assert.Equal(destinationNetworkId, clonedSimulation.NetworkId);
            Assert.Equal(simulation.NetworkName, clonedSimulation.NetworkName);
            var allAttributes = TestHelper.UnitOfWork.AttributeRepo.GetAttributes();
            var ageAttribute = allAttributes.Single(a => a.Name == TestAttributeNames.Age);
            var clonedAttribute = TestHelper.UnitOfWork.CalculatedAttributeRepo.GetScenarioCalulatedAttributesByScenarioAndAttributeId(clonedSimulationId, ageAttribute.Id);
            ObjectAssertions.EquivalentExcluding(calculatedAttribute, clonedAttribute, c => c.Id, c => c.Equations[0].Id, c => c.Equations[0].Equation.Id);
            Assert.NotEqual(calculatedAttribute.Id, clonedAttribute.Id);
            Assert.NotEqual(calculatedAttribute.Equations[0].Id, clonedAttribute.Equations[0].Id);
            Assert.NotEqual(calculatedAttribute.Equations[0].Equation.Id, clonedAttribute.Equations[0].Equation.Id);
        }

        [Fact]
        public async Task SimulationInDbWithCalculatedAttributeWithCriterionLibrary_Clone_Clones()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, false);
            TestHelper.UnitOfWork.SetUser(user.Username);
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var destinationNetworkId = SimulationCloningTestSetup.TestDestinationNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, destinationNetworkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            var calculatedAttributeId = Guid.NewGuid();
            var calculatedAttribute = CalculatedAttributeTestSetup.TestCalculatedAttributeDtoWithEquationCriterionLibrary(calculatedAttributeId, TestAttributeNames.Age, "");
            var calculatedAttributes = new List<CalculatedAttributeDTO> { calculatedAttribute };
            TestHelper.UnitOfWork.CalculatedAttributeRepo.UpsertScenarioCalculatedAttributes(calculatedAttributes, simulationId);

            var calculatedAttributeCriterionLibrary = await TestHelper.UnitOfWork.CriterionLibraryRepo.CriteriaLibrary(calculatedAttribute.Equations[0].CriteriaLibrary.Id);
            calculatedAttributeCriterionLibrary.MergedCriteriaExpression = "MergedCriteriaExpression";
            TestHelper.UnitOfWork.CriterionLibraryRepo.UpsertCriterionLibrary(calculatedAttributeCriterionLibrary);

            var cloneSimulationDto = CloneSimulationDtos.Create(simulationId, networkId, newSimulationName);
            cloneSimulationDto.DestinationNetworkId = destinationNetworkId;
            var cloningService = CreateCompleteSimulationCloningService();
            var cloningResult = cloningService.Clone(cloneSimulationDto);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(clonedSimulationId);
            Assert.Equal(newSimulationName, clonedSimulation.Name);
            Assert.Equal(destinationNetworkId, clonedSimulation.NetworkId);
            Assert.Equal(simulation.NetworkName, clonedSimulation.NetworkName);
            var allAttributes = TestHelper.UnitOfWork.AttributeRepo.GetAttributes();
            var ageAttribute = allAttributes.Single(a => a.Name == TestAttributeNames.Age);
            var clonedAttribute = TestHelper.UnitOfWork.CalculatedAttributeRepo.GetScenarioCalulatedAttributesByScenarioAndAttributeId(clonedSimulationId, ageAttribute.Id);
            calculatedAttribute.Equations[0].CriteriaLibrary = calculatedAttributeCriterionLibrary;
            ObjectAssertions.EquivalentExcluding(calculatedAttribute, clonedAttribute, c => c.Id, c => c.Equations[0].Id, c => c.Equations[0].Equation.Id, c => c.Equations[0].CriteriaLibrary.Id, c => c.Equations[0].CriteriaLibrary.Owner);
            Assert.NotEqual(calculatedAttribute.Id, clonedAttribute.Id);
            Assert.NotEqual(calculatedAttribute.Equations[0].Id, clonedAttribute.Equations[0].Id);
            Assert.NotEqual(calculatedAttribute.Equations[0].Equation.Id, clonedAttribute.Equations[0].Equation.Id);
            Assert.NotEqual(calculatedAttribute.Equations[0].CriteriaLibrary.Id, clonedAttribute.Equations[0].CriteriaLibrary.Id);
            Assert.Equal(user.Id, clonedAttribute.Equations[0].CriteriaLibrary.Owner);
        }

        [Fact]
        public void SimulationInDbWithBudgetWithAmountAndCommittedProject_Clone_Clones()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var networkId = Guid.NewGuid();
            var destinationNetworkId = SimulationCloningTestSetup.TestDestinationNetworkIdInDatabase();
            var keyAttributeId = TestAttributeIds.BrKeyId;

            var maintainableAssets = new List<MaintainableAsset>();
            var assetId = Guid.NewGuid();
            var locationIdentifier = RandomStrings.WithPrefix("Location");
            var location = Locations.Section(locationIdentifier);
            var maintainableAsset = new MaintainableAsset(assetId, destinationNetworkId, location, "[Deck_Area]");
            var maintainableAssetLocationId = Guid.NewGuid();
            var maintainableAssetEntity = maintainableAsset.ToEntity(destinationNetworkId);
            var maintainableAssetLocation = new MaintainableAssetLocationEntity()
            {
                Id = maintainableAssetLocationId,
                LocationIdentifier = "2",
                Discriminator = DataPersistenceConstants.SectionLocation,
            };
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            maintainableAssetEntity.MaintainableAssetLocation = maintainableAssetLocation;
            var testMaintainableAsset = maintainableAssetEntity.ToDomain(locationIdentifier);
            maintainableAssets.Add(testMaintainableAsset);

            var network = NetworkTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, maintainableAssets, networkId, keyAttributeId);
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, destinationNetworkId);
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

            var cloneSimulationDto = CloneSimulationDtos.Create(simulationId, networkId, newSimulationName);
            cloneSimulationDto.DestinationNetworkId = destinationNetworkId;
            var cloningService = CreateCompleteSimulationCloningService();
            var cloningResult = cloningService.Clone(cloneSimulationDto);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(clonedSimulationId);
            Assert.Equal(newSimulationName, clonedSimulation.Name);
            Assert.Equal(destinationNetworkId, clonedSimulation.NetworkId);
            Assert.Equal(simulation.NetworkName, clonedSimulation.NetworkName);

            var clonedProjects = TestHelper.UnitOfWork.CommittedProjectRepo.GetSectionCommittedProjectDTOs(clonedSimulationId);
            var clonedProject = clonedProjects.Single();
            ObjectAssertions.EquivalentExcluding(sectionCommittedProject, clonedProject, x => x.SimulationId, cp => cp.ScenarioBudgetId, cp => cp.Id, cp => cp.LocationKeys);
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
            var destinationNetworkId = SimulationCloningTestSetup.TestDestinationNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, destinationNetworkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            var budgetId = Guid.NewGuid();
            var budget = BudgetDtos.WithSingleAmount(budgetId, "budget", 2023, 4321);
            var budgets = new List<BudgetDTO> { budget };
            ScenarioBudgetTestSetup.UpsertOrDeleteScenarioBudgets(TestHelper.UnitOfWork, budgets, simulationId);

            var cloneSimulationDto = CloneSimulationDtos.Create(simulationId, networkId, newSimulationName);
            cloneSimulationDto.DestinationNetworkId = destinationNetworkId;
            var cloningService = CreateCompleteSimulationCloningService();
            var cloningResult = cloningService.Clone(cloneSimulationDto);

            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(cloningResult.Simulation.Id);
            var clonedBudgets = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(cloningResult.Simulation.Id);
            var clonedBudget = clonedBudgets.Single();
            Assert.NotEqual(budgetId, clonedBudget.Id);
            Assert.NotEqual(budget.BudgetAmounts[0].Id, clonedBudget.BudgetAmounts[0].Id);
            ObjectAssertions.EquivalentExcluding(budget, clonedBudget, b => b.Id, b => b.CriterionLibrary, b => b.BudgetAmounts[0].Id);
        }

        [Fact]
        public void SimulationInDbWithScenarioBudget_Clone_Clones()
        {
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var destinationNetworkId = SimulationCloningTestSetup.TestDestinationNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            var budgetId = Guid.NewGuid();
            var budget = BudgetDtos.New(budgetId);
            var budgets = new List<BudgetDTO> { budget };
            ScenarioBudgetTestSetup.UpsertOrDeleteScenarioBudgets(TestHelper.UnitOfWork, budgets, simulationId);

            var cloneSimulationDto = CloneSimulationDtos.Create(simulationId, networkId, newSimulationName);
            cloneSimulationDto.DestinationNetworkId = destinationNetworkId;
            var cloningService = CreateCompleteSimulationCloningService();
            var cloningResult = cloningService.Clone(cloneSimulationDto);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(cloningResult.Simulation.Id);
            var clonedBudgets = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(cloningResult.Simulation.Id);
            var clonedBudget = clonedBudgets.Single();
            ObjectAssertions.EquivalentExcluding(budget, clonedBudget, b => b.Id, b => b.CriterionLibrary);
        }

        [Fact]
        public void SimulationInDbWithBudgetWithPercentagePair_Clone_Clones()
        {
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var destinationNetworkId = SimulationCloningTestSetup.TestDestinationNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            var budgetId = Guid.NewGuid();
            var budget = BudgetDtos.New(budgetId);
            var budgets = new List<BudgetDTO> { budget };
            ScenarioBudgetTestSetup.UpsertOrDeleteScenarioBudgets(TestHelper.UnitOfWork, budgets, simulationId);
            var budgetPriority = BudgetPriorityDtos.WithPercentagePair(budget.Name, budgetId);
            var budgetPriorities = new List<BudgetPriorityDTO> { budgetPriority };
            TestHelper.UnitOfWork.BudgetPriorityRepo.UpsertOrDeleteScenarioBudgetPriorities(budgetPriorities, simulationId);

            var cloneSimulationDto = CloneSimulationDtos.Create(simulationId, networkId, newSimulationName);
            cloneSimulationDto.DestinationNetworkId = destinationNetworkId;
            var cloningService = CreateCompleteSimulationCloningService();
            var cloningResult = cloningService.Clone(cloneSimulationDto);

            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(cloningResult.Simulation.Id);
            var clonedPriorities = TestHelper.UnitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(clonedSimulation.Id);
            var clonedPriority = clonedPriorities.Single();
            var originalPair = budgetPriority.BudgetPercentagePairs[0];
            var clonedPair = clonedPriority.BudgetPercentagePairs[0];
            var clonedbudgets = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(clonedSimulation.Id);
            var clonedbudget = clonedbudgets.Single();
            Assert.NotEqual(originalPair.Id, clonedPair.Id);
            Assert.Equal(clonedbudget.Id, clonedPair.BudgetId);
            Assert.Equal(originalPair.Percentage, clonedPair.Percentage);
            Assert.Equal(budget.Name, clonedPair.BudgetName);
            ObjectAssertions.EquivalentExcluding(budgetPriority, clonedPriority, bp => bp.Id, bp => bp.CriterionLibrary, bp => bp.BudgetPercentagePairs);
        }

        [Fact]
        public void SimulationInDbWithScenarioBudgetWithCriterionLibrary_Clone_Clones()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var networkId = NetworkTestSetup.NetworkId;
            var destinationNetworkId = SimulationCloningTestSetup.TestDestinationNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            var budgetId = Guid.NewGuid();
            var budget = BudgetDtos.New(budgetId);
            budget.CriterionLibrary = CriterionLibraryDtos.Dto();
            var budgets = new List<BudgetDTO> { budget };
            ScenarioBudgetTestSetup.UpsertOrDeleteScenarioBudgets(TestHelper.UnitOfWork, budgets, simulationId);

            var cloneSimulationDto = CloneSimulationDtos.Create(simulationId, networkId, newSimulationName);
            cloneSimulationDto.DestinationNetworkId = destinationNetworkId;
            var cloningService = CreateCompleteSimulationCloningService();
            var cloningResult = cloningService.Clone(cloneSimulationDto);

            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(cloningResult.Simulation.Id);
            var clonedBudgets = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(cloningResult.Simulation.Id);
            var clonedBudget = clonedBudgets.Single();
            var originalLibrary = budget.CriterionLibrary;
            var clonedLibrary = clonedBudget.CriterionLibrary;
            SimulationCloningCriterionLibraryDtoAssertions.AssertValidLibraryClone(originalLibrary, clonedLibrary, TestHelper.UnitOfWork.UserEntity?.Id);
            ObjectAssertions.EquivalentExcluding(budget, clonedBudget, b => b.Id, b => b.CriterionLibrary, b => b.BudgetAmounts[0].Id);
        }

        [Fact]
        public void SimulationInDbWithBudgetPriority_Clone_Clones()
        {
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var destinationNetworkId = SimulationCloningTestSetup.TestDestinationNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            var budgetPriority = BudgetPriorityTestSetup.SetupSingleBudgetPriorityForSimulationInDb(simulationId);

            var cloneSimulationDto = CloneSimulationDtos.Create(simulationId, networkId, newSimulationName);
            cloneSimulationDto.DestinationNetworkId = destinationNetworkId;
            var cloningService = CreateCompleteSimulationCloningService();
            var cloningResult = cloningService.Clone(cloneSimulationDto);

            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(cloningResult.Simulation.Id);
            var clonedPriorities = TestHelper.UnitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(clonedSimulation.Id);
            var clonedPriority = clonedPriorities.Single();
            ObjectAssertions.EquivalentExcluding(budgetPriority, clonedPriority, bp => bp.Id, bp => bp.CriterionLibrary);
        }

        [Fact]
        public void SimulationInDbWithBudgetPriorityWithCriterionLibrary_Clone_Clones()
        {
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var destinationNetworkId = SimulationCloningTestSetup.TestDestinationNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            var budgetPriority = BudgetPriorityTestSetup.SetupSingleBudgetPriorityWithCriterionLibraryForSimulationInDb(simulationId);

            var cloneSimulationDto = CloneSimulationDtos.Create(simulationId, networkId, newSimulationName);
            cloneSimulationDto.DestinationNetworkId = destinationNetworkId;
            var cloningService = CreateCompleteSimulationCloningService();
            var cloningResult = cloningService.Clone(cloneSimulationDto);

            var clonedSimulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(cloningResult.Simulation.Id);
            var clonedPriorities = TestHelper.UnitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(clonedSimulation.Id);
            var clonedPriority = clonedPriorities.Single();
            ObjectAssertions.EquivalentExcluding(budgetPriority, clonedPriority, bp => bp.Id, bp => bp.CriterionLibrary);

        }

        [Fact]
        public void SimulationInDbWithCashFlowRule_Clone_Clones()
        {

            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var destinationNetworkId = SimulationCloningTestSetup.TestDestinationNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);
            var ruleId = Guid.NewGuid();
            var cashFlowRule = CashFlowRuleDtos.Rule(ruleId);
            var cashFlowRules = new List<CashFlowRuleDTO> { cashFlowRule };
            TestHelper.UnitOfWork.CashFlowRuleRepo.UpsertOrDeleteScenarioCashFlowRules(cashFlowRules, simulationId);

            var cloneSimulationDto = CloneSimulationDtos.Create(simulationId, networkId, newSimulationName);
            cloneSimulationDto.DestinationNetworkId = destinationNetworkId;
            var cloningService = CreateCompleteSimulationCloningService();
            var cloningResult = cloningService.Clone(cloneSimulationDto);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedCashFlowRules = TestHelper.UnitOfWork.CashFlowRuleRepo.GetScenarioCashFlowRules(clonedSimulationId);
            var clonedCashFlowRule = clonedCashFlowRules.Single();
            Assert.Equal(cashFlowRule.Name, clonedCashFlowRule.Name);
            var distributionRule = cashFlowRule.CashFlowDistributionRules.Single();
            var clonedDistributionRule = clonedCashFlowRule.CashFlowDistributionRules.Single();
            ObjectAssertions.EquivalentExcluding(distributionRule, clonedDistributionRule, x => x.Id);
            var originalLibrary = cashFlowRule.CriterionLibrary;
            var clonedLibrary = clonedCashFlowRule.CriterionLibrary;
            SimulationCloningCriterionLibraryDtoAssertions.AssertValidLibraryClone(originalLibrary, clonedLibrary, TestHelper.UnitOfWork.UserEntity?.Id);
        }


        [Fact]
        public void SimulationInDbWithInvestmentPlan_Clone_Clones()
        {
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var destinationNetworkId = SimulationCloningTestSetup.TestDestinationNetworkIdInDatabase();
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            var newSimulationName = RandomStrings.WithPrefix("cloned");
            var investmentPlanId = Guid.NewGuid();
            var investmentPlan = InvestmentPlanDtos.Dto(investmentPlanId);
            TestHelper.UnitOfWork.InvestmentPlanRepo.UpsertInvestmentPlan(investmentPlan, simulationId);
            var simulation = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulationId);

            var cloneSimulationDto = CloneSimulationDtos.Create(simulationId, networkId, newSimulationName);
            cloneSimulationDto.DestinationNetworkId = destinationNetworkId;
            var cloningService = CreateCompleteSimulationCloningService();
            var cloningResult = cloningService.Clone(cloneSimulationDto);

            var clonedSimulationId = cloningResult.Simulation.Id;
            var clonedInvestmentPlan = TestHelper.UnitOfWork.InvestmentPlanRepo.GetInvestmentPlan(clonedSimulationId);
            ObjectAssertions.EquivalentExcluding(investmentPlan, clonedInvestmentPlan,
                i => i.Id);
        }

        [Fact]
        public void SimulationInDbWithPerformanceCurve_Clone_Clones()
        {
            var networkId = SimulationCloningTestSetup.TestNetworkIdInDatabase();
            var destinationNetworkId = SimulationCloningTestSetup.TestDestinationNetworkIdInDatabase();
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

            var cloneSimulationDto = CloneSimulationDtos.Create(simulationId, networkId, newSimulationName);
            cloneSimulationDto.DestinationNetworkId = destinationNetworkId;
            var cloningService = CreateCompleteSimulationCloningService();
            var cloningResult = cloningService.Clone(cloneSimulationDto);

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
            var destinationNetworkId = SimulationCloningTestSetup.TestDestinationNetworkIdInDatabase();
            var keyAttributeId = TestAttributeIds.BrKeyId;

            var network = NetworkTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, new List<MaintainableAsset>(), networkId, keyAttributeId);
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

            var cloneSimulationDto = CloneSimulationDtos.Create(simulationId, networkId, newSimulationName);
            cloneSimulationDto.DestinationNetworkId = destinationNetworkId;
            var cloningService = CreateCompleteSimulationCloningService();
            var cloningResult = cloningService.Clone(cloneSimulationDto);

            var warningMessage = cloningResult.WarningMessage;
            Assert.Contains("Budget in the wrong simulation", warningMessage);
        }

    }
}
