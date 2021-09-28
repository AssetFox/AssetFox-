﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SimulationRepository : ISimulationRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public SimulationRepository(UnitOfDataPersistenceWork unitOfWork)
        {
            _unitOfWork = unitOfWork ??
                          throw new ArgumentNullException(nameof(unitOfWork));
        }

        public void CreateSimulation(Simulation simulation)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == simulation.Network.Id))
            {
                throw new RowNotInTableException("The specified network was not found.");
            }

            _unitOfWork.Context.AddEntity(simulation.ToEntity(), _unitOfWork.UserEntity?.Id);
        }

        public void GetAllInNetwork(Network network)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == network.Id))
            {
                throw new RowNotInTableException("The specified network was not found.");
            }

            var entities = _unitOfWork.Context.Simulation.Where(_ => _.NetworkId == network.Id).ToList();

            // "GetAllInNetwork" is only getting used in testing. Therefore, passing DateTime.Now,
            // instead of actual date
            entities.ForEach(_ => _.CreateSimulation(network, DateTime.Now, DateTime.Now));
        }

        public List<SimulationDTO> GetAllInNetwork(Guid networkId)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            if (!_unitOfWork.Context.Simulation.Any())
            {
                return new List<SimulationDTO>();
            }

            var users = _unitOfWork.Context.User.ToList();

            var simulationEntities = _unitOfWork.Context.Simulation
                .Include(_ => _.SimulationAnalysisDetail)
                .Include(_ => _.SimulationReportDetail)
                .Include(_ => _.SimulationUserJoins)
                .ThenInclude(_ => _.User)
                .Where(_ => _.NetworkId == networkId)
                .ToList();

            return simulationEntities.Select(_ => _.ToDto(users.FirstOrDefault(__ => __.Id == _.CreatedBy)))
                .ToList();
        }

        public void GetSimulationInNetwork(Guid simulationId, Network network)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == network.Id))
            {
                throw new RowNotInTableException("The specified network was not found.");
            }

            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation found for given scenario.");
            }

            var simulationEntity = _unitOfWork.Context.Simulation.AsNoTracking().Single(_ => _.Id == simulationId);
            var simulationAnalysisDetail = _unitOfWork.Context.SimulationAnalysisDetail.AsNoTracking()
                .SingleOrDefault(_ => _.SimulationId == simulationId);

            simulationEntity.CreateSimulation(network, simulationAnalysisDetail.LastRun,
                simulationAnalysisDetail.LastModifiedDate);
        }

        public void CreateSimulation(Guid networkId, SimulationDTO dto)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            var defaultLibrary = _unitOfWork.Context.CalculatedAttributeLibrary.Where(_ => _.IsDefault == true)
                .Include(_ => _.CalculatedAttributes)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.CalculatedAttributes)
                .ThenInclude(_ => _.Equations)
                .ThenInclude(_ => _.CriterionLibraryCalculatedAttributeJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.CalculatedAttributes)
                .ThenInclude(_ => _.Equations)
                .ThenInclude(_ => _.EquationCalculatedAttributeJoin)
                .ThenInclude(_ => _.Equation)
                .Select(_ => _.ToDto())
                .ToList();

            if (defaultLibrary.Count == 0)
            {
                throw new RowNotInTableException($"No default library for Calculated Attributes has been found. Please contact admin");
            }

            var simulationEntity = dto.ToEntity(networkId);
            // if there are multiple default libraries (This should not happen). Take the first one
             //var libraryDto = defaultLibrary[0].ToDto();

            _unitOfWork.Context.AddEntity(simulationEntity, _unitOfWork.UserEntity?.Id);
            if (dto.Users.Any())
            {
                _unitOfWork.Context.AddAll(dto.Users.Select(_ => _.ToEntity(dto.Id)).ToList(),
                    _unitOfWork.UserEntity?.Id);
            }
            ICalculatedAttributesRepository _calculatedAttributesRepo = _unitOfWork.CalculatedAttributeRepo;
            // Assiging new Ids because this object will be assiged to a simulation
            defaultLibrary[0].CalculatedAttributes.ForEach(_ => {
                _.Id = Guid.NewGuid();
                _.Equations.ForEach(e =>
                {
                    e.Id = Guid.NewGuid();
                    if (e.CriteriaLibrary != null)
                    {
                        e.CriteriaLibrary.Id = Guid.NewGuid();
                        e.CriteriaLibrary.IsSingleUse = true;
                    }
                    e.Equation.Id = Guid.NewGuid();
                });
            });
            _calculatedAttributesRepo.UpsertScenarioCalculatedAttributes(defaultLibrary[0].CalculatedAttributes, simulationEntity.Id);
        }

        public SimulationDTO GetSimulation(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            var users = _unitOfWork.Context.User.ToList();

            var simulationEntity = _unitOfWork.Context.Simulation
                .Include(_ => _.SimulationAnalysisDetail)
                .Include(_ => _.SimulationUserJoins)
                .ThenInclude(_ => _.User)
                .Single(_ => _.Id == simulationId);

            return simulationEntity.ToDto(users.FirstOrDefault(_ => _.Id == simulationEntity.CreatedBy));
        }

        public string GetSimulationName(Guid simulationId)
        {
            var selectedSimulation = _unitOfWork.Context.Simulation.FirstOrDefault(_ => _.Id == simulationId);
            // We either need to return null here or an error.  An empty string is possible for an existing simulation.
            return (selectedSimulation == null) ? null : selectedSimulation.Name;
        }

        public SimulationCloningResultDTO CloneSimulation(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            var budgetsPreventingCloning = new List<string>();
            var numberOfCommittedProjectsAffected = 0;

            var simulationToClone = _unitOfWork.Context.Simulation.AsNoTracking().AsSplitQuery()
                // analysis method
                .Include(_ => _.AnalysisMethod)
                .ThenInclude(_ => _.Benefit)
                .Include(_ => _.AnalysisMethod)
                .ThenInclude(_ => _.CriterionLibraryAnalysisMethodJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                // budgets
                .Include(_ => _.Budgets)
                .ThenInclude(_ => _.ScenarioBudgetAmounts)
                .Include(_ => _.Budgets)
                .ThenInclude(_ => _.CriterionLibraryScenarioBudgetJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                // budget priorities
                .Include(_ => _.BudgetPriorities)
                .ThenInclude(_ => _.BudgetPercentagePairs)
                .ThenInclude(_ => _.ScenarioBudget)
                .Include(_ => _.BudgetPriorities)
                .ThenInclude(_ => _.CriterionLibraryScenarioBudgetPriorityJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                // cash flow rules
                .Include(_ => _.CashFlowRules)
                .ThenInclude(_ => _.ScenarioCashFlowDistributionRules)
                .Include(_ => _.CashFlowRules)
                .ThenInclude(_ => _.CriterionLibraryScenarioCashFlowRuleJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                // investment plan
                .Include(_ => _.InvestmentPlan)
                // committed projects
                .Include(_ => _.CommittedProjects)
                .ThenInclude(_ => _.CommittedProjectConsequences)
                .Include(_ => _.CommittedProjects)
                .ThenInclude(_ => _.ScenarioBudget)
                // performance curves
                .Include(_ => _.PerformanceCurves)
                .ThenInclude(_ => _.ScenarioPerformanceCurveEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.PerformanceCurves)
                .ThenInclude(_ => _.CriterionLibraryScenarioPerformanceCurveJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                // remaining life limits
                .Include(_ => _.RemainingLifeLimits)
                .ThenInclude(_ => _.CriterionLibraryScenarioRemainingLifeLimitJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                // selectable treatments
                .Include(_ => _.SelectableTreatments)
                .ThenInclude(_ => _.ScenarioTreatmentConsequences)
                .ThenInclude(_ => _.ScenarioConditionalTreatmentConsequenceEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.SelectableTreatments)
                .ThenInclude(_ => _.ScenarioTreatmentConsequences)
                .ThenInclude(_ => _.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.SelectableTreatments)
                .ThenInclude(_ => _.ScenarioTreatmentCosts)
                .ThenInclude(_ => _.ScenarioTreatmentCostEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.SelectableTreatments)
                .ThenInclude(_ => _.ScenarioTreatmentCosts)
                .ThenInclude(_ => _.CriterionLibraryScenarioTreatmentCostJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.SelectableTreatments)
                .ThenInclude(_ => _.ScenarioTreatmentSchedulings)
                .Include(_ => _.SelectableTreatments)
                .ThenInclude(_ => _.ScenarioTreatmentSupersessions)
                .ThenInclude(_ => _.CriterionLibraryScenarioTreatmentSupersessionJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.SelectableTreatments)
                .ThenInclude(_ => _.ScenarioSelectableTreatmentScenarioBudgetJoins)
                .ThenInclude(_ => _.ScenarioBudget)
                .Include(_ => _.SelectableTreatments)
                .ThenInclude(_ => _.CriterionLibraryScenarioSelectableTreatmentJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                // deficient condition goals
                .Include(_ => _.ScenarioDeficientConditionGoals)
                .ThenInclude(_ => _.CriterionLibraryScenarioDeficientConditionGoalJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                // target condition goals
                .Include(_ => _.ScenarioTargetConditionalGoals)
                .ThenInclude(_ => _.CriterionLibraryScenarioTargetConditionGoalJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Single(_ => _.Id == simulationId);

            simulationToClone.Id = Guid.NewGuid();
            _unitOfWork.Context.ReInitializeAllEntityBaseProperties(simulationToClone, _unitOfWork.UserEntity?.Id);

            if (simulationToClone.AnalysisMethod != null)
            {
                simulationToClone.AnalysisMethod.Id = Guid.NewGuid();
                simulationToClone.AnalysisMethod.SimulationId = simulationToClone.Id;
                simulationToClone.AnalysisMethod.Simulation = null;
                _unitOfWork.Context.ReInitializeAllEntityBaseProperties(simulationToClone.AnalysisMethod,
                    _unitOfWork.UserEntity?.Id);

                if (simulationToClone.AnalysisMethod.Benefit != null)
                {
                    simulationToClone.AnalysisMethod.Benefit.Id = Guid.NewGuid();
                    simulationToClone.AnalysisMethod.Benefit.AnalysisMethodId = simulationToClone.AnalysisMethod.Id;
                    _unitOfWork.Context.ReInitializeAllEntityBaseProperties(simulationToClone.AnalysisMethod.Benefit,
                        _unitOfWork.UserEntity?.Id);
                }

                if (simulationToClone.AnalysisMethod.CriterionLibraryAnalysisMethodJoin != null)
                {
                    var criterionId = Guid.NewGuid();
                    simulationToClone.AnalysisMethod.CriterionLibraryAnalysisMethodJoin.CriterionLibrary.Id =
                        criterionId;
                    simulationToClone.AnalysisMethod.CriterionLibraryAnalysisMethodJoin.CriterionLibrary.IsSingleUse =
                        true;
                    simulationToClone.AnalysisMethod.CriterionLibraryAnalysisMethodJoin.CriterionLibraryId =
                        criterionId;
                    simulationToClone.AnalysisMethod.CriterionLibraryAnalysisMethodJoin.AnalysisMethodId =
                        simulationToClone.AnalysisMethod.Id;
                    _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                        simulationToClone.AnalysisMethod.CriterionLibraryAnalysisMethodJoin.CriterionLibrary,
                        _unitOfWork.UserEntity?.Id);
                    _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                        simulationToClone.AnalysisMethod.CriterionLibraryAnalysisMethodJoin,
                        _unitOfWork.UserEntity?.Id);
                }
            }

            if (simulationToClone.Budgets.Any())
            {
                simulationToClone.Budgets.ToList().ForEach(budget =>
                {
                    budget.Id = Guid.NewGuid();
                    budget.SimulationId = simulationToClone.Id;
                    budget.Simulation = null;
                    _unitOfWork.Context.ReInitializeAllEntityBaseProperties(budget, _unitOfWork.UserEntity?.Id);

                    if (budget.ScenarioBudgetAmounts.Any())
                    {
                        budget.ScenarioBudgetAmounts.ForEach(budgetAmount =>
                        {
                            budgetAmount.Id = Guid.NewGuid();
                            budgetAmount.ScenarioBudgetId = budget.Id;
                            _unitOfWork.Context.ReInitializeAllEntityBaseProperties(budgetAmount,
                                _unitOfWork.UserEntity?.Id);
                        });
                    }

                    if (budget.CriterionLibraryScenarioBudgetJoin != null)
                    {
                        var criterionId = Guid.NewGuid();
                        budget.CriterionLibraryScenarioBudgetJoin.CriterionLibrary.Id = criterionId;
                        budget.CriterionLibraryScenarioBudgetJoin.CriterionLibrary.IsSingleUse = true;
                        budget.CriterionLibraryScenarioBudgetJoin.CriterionLibraryId = criterionId;
                        budget.CriterionLibraryScenarioBudgetJoin.ScenarioBudgetId = budget.Id;
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                            budget.CriterionLibraryScenarioBudgetJoin.CriterionLibrary, _unitOfWork.UserEntity?.Id);
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                            budget.CriterionLibraryScenarioBudgetJoin, _unitOfWork.UserEntity?.Id);
                    }
                });
            }

            if (simulationToClone.BudgetPriorities.Any())
            {
                simulationToClone.BudgetPriorities.ForEach(budgetPriority =>
                {
                    budgetPriority.Id = Guid.NewGuid();
                    budgetPriority.SimulationId = simulationToClone.Id;
                    budgetPriority.Simulation = null;
                    _unitOfWork.Context.ReInitializeAllEntityBaseProperties(budgetPriority, _unitOfWork.UserEntity?.Id);

                    if (budgetPriority.BudgetPercentagePairs.Any())
                    {
                        budgetPriority.BudgetPercentagePairs = budgetPriority.BudgetPercentagePairs.Where(_ =>
                            simulationToClone.Budgets.Any(budget => budget.Name == _.ScenarioBudget.Name)).ToList();
                        budgetPriority.BudgetPercentagePairs.ForEach(percentagePair =>
                        {
                            if (simulationToClone.Budgets.Any(_ => _.Name == percentagePair.ScenarioBudget.Name))
                            {
                                percentagePair.Id = Guid.NewGuid();
                                percentagePair.ScenarioBudgetId = simulationToClone.Budgets
                                    .Single(_ => _.Name == percentagePair.ScenarioBudget.Name).Id;
                                percentagePair.ScenarioBudget = null;
                                _unitOfWork.Context.ReInitializeAllEntityBaseProperties(percentagePair,
                                    _unitOfWork.UserEntity?.Id);
                            }
                        });
                    }

                    if (budgetPriority.CriterionLibraryScenarioBudgetPriorityJoin != null)
                    {
                        var criterionId = Guid.NewGuid();
                        budgetPriority.CriterionLibraryScenarioBudgetPriorityJoin.CriterionLibrary.Id = criterionId;
                        budgetPriority.CriterionLibraryScenarioBudgetPriorityJoin.CriterionLibrary.IsSingleUse = true;
                        budgetPriority.CriterionLibraryScenarioBudgetPriorityJoin.CriterionLibraryId = criterionId;
                        budgetPriority.CriterionLibraryScenarioBudgetPriorityJoin.ScenarioBudgetPriorityId =
                            budgetPriority.Id;
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                            budgetPriority.CriterionLibraryScenarioBudgetPriorityJoin.CriterionLibrary,
                            _unitOfWork.UserEntity?.Id);
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                            budgetPriority.CriterionLibraryScenarioBudgetPriorityJoin, _unitOfWork.UserEntity?.Id);
                    }
                });
            }

            if (simulationToClone.CashFlowRules.Any())
            {
                simulationToClone.CashFlowRules.ForEach(cashFlowRule =>
                {
                    cashFlowRule.Id = Guid.NewGuid();
                    cashFlowRule.SimulationId = simulationToClone.Id;
                    cashFlowRule.Simulation = null;
                    _unitOfWork.Context.ReInitializeAllEntityBaseProperties(cashFlowRule, _unitOfWork.UserEntity?.Id);

                    if (cashFlowRule.ScenarioCashFlowDistributionRules.Any())
                    {
                        cashFlowRule.ScenarioCashFlowDistributionRules.ForEach(distributionRule =>
                        {
                            distributionRule.Id = Guid.NewGuid();
                            distributionRule.ScenarioCashFlowRuleId = cashFlowRule.Id;
                            _unitOfWork.Context.ReInitializeAllEntityBaseProperties(distributionRule,
                                _unitOfWork.UserEntity?.Id);
                        });
                    }

                    if (cashFlowRule.CriterionLibraryScenarioCashFlowRuleJoin != null)
                    {
                        var criterionId = Guid.NewGuid();
                        cashFlowRule.CriterionLibraryScenarioCashFlowRuleJoin.CriterionLibrary.Id = criterionId;
                        cashFlowRule.CriterionLibraryScenarioCashFlowRuleJoin.CriterionLibrary.IsSingleUse = true;
                        cashFlowRule.CriterionLibraryScenarioCashFlowRuleJoin.CriterionLibraryId = criterionId;
                        cashFlowRule.CriterionLibraryScenarioCashFlowRuleJoin.ScenarioCashFlowRuleId = cashFlowRule.Id;
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                            cashFlowRule.CriterionLibraryScenarioCashFlowRuleJoin.CriterionLibrary,
                            _unitOfWork.UserEntity?.Id);
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                            cashFlowRule.CriterionLibraryScenarioCashFlowRuleJoin, _unitOfWork.UserEntity?.Id);
                    }
                });
            }

            if (simulationToClone.InvestmentPlan != null)
            {
                simulationToClone.InvestmentPlan.Id = Guid.NewGuid();
                simulationToClone.InvestmentPlan.SimulationId = simulationToClone.Id;
                simulationToClone.InvestmentPlan.Simulation = null;
                _unitOfWork.Context.ReInitializeAllEntityBaseProperties(simulationToClone.InvestmentPlan,
                    _unitOfWork.UserEntity?.Id);
            }

            if (simulationToClone.CommittedProjects.Any())
            {
                var committedProjectsAffected = simulationToClone.CommittedProjects.Where(_ =>
                    simulationToClone.Budgets.Any(budget => budget.Name != _.ScenarioBudget.Name)).ToList();
                if (committedProjectsAffected.Any())
                {
                    numberOfCommittedProjectsAffected = committedProjectsAffected.Count();
                    budgetsPreventingCloning = committedProjectsAffected
                        .Select(_ => _.ScenarioBudget.Name).Distinct().ToList();
                }

                simulationToClone.CommittedProjects = simulationToClone.CommittedProjects.Where(_ =>
                    simulationToClone.Budgets.Any(budget => budget.Name == _.ScenarioBudget.Name)).ToList();
                simulationToClone.CommittedProjects.ForEach(committedProject =>
                {
                    if (simulationToClone.Budgets.Any(_ => _.Name == committedProject.ScenarioBudget.Name))
                    {
                        committedProject.Id = Guid.NewGuid();
                        committedProject.SimulationId = simulationToClone.Id;
                        committedProject.Simulation = null;
                        committedProject.ScenarioBudgetId = simulationToClone.Budgets
                            .Single(_ => _.Name == committedProject.ScenarioBudget.Name).Id;
                        committedProject.ScenarioBudget = null;
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(committedProject,
                            _unitOfWork.UserEntity?.Id);

                        if (committedProject.CommittedProjectConsequences.Any())
                        {
                            committedProject.CommittedProjectConsequences.ForEach(consequence =>
                            {
                                consequence.Id = Guid.NewGuid();
                                consequence.CommittedProjectId = committedProject.Id;
                                _unitOfWork.Context.ReInitializeAllEntityBaseProperties(consequence,
                                    _unitOfWork.UserEntity?.Id);
                            });
                        }
                    }
                });
            }

            if (simulationToClone.PerformanceCurves.Any())
            {
                simulationToClone.PerformanceCurves.ForEach(performanceCurve =>
                {
                    performanceCurve.Id = Guid.NewGuid();
                    performanceCurve.SimulationId = simulationToClone.Id;
                    performanceCurve.Simulation = null;
                    _unitOfWork.Context.ReInitializeAllEntityBaseProperties(performanceCurve,
                        _unitOfWork.UserEntity?.Id);

                    if (performanceCurve.ScenarioPerformanceCurveEquationJoin != null)
                    {
                        var equationId = Guid.NewGuid();
                        performanceCurve.ScenarioPerformanceCurveEquationJoin.Equation.Id = equationId;
                        performanceCurve.ScenarioPerformanceCurveEquationJoin.EquationId = equationId;
                        performanceCurve.ScenarioPerformanceCurveEquationJoin.ScenarioPerformanceCurveId =
                            performanceCurve.Id;
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                            performanceCurve.ScenarioPerformanceCurveEquationJoin.Equation, _unitOfWork.UserEntity?.Id);
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                            performanceCurve.ScenarioPerformanceCurveEquationJoin, _unitOfWork.UserEntity?.Id);
                    }

                    if (performanceCurve.CriterionLibraryScenarioPerformanceCurveJoin != null)
                    {
                        var criterionId = Guid.NewGuid();
                        performanceCurve.CriterionLibraryScenarioPerformanceCurveJoin.CriterionLibrary.Id = criterionId;
                        performanceCurve.CriterionLibraryScenarioPerformanceCurveJoin.CriterionLibrary.IsSingleUse =
                            true;
                        performanceCurve.CriterionLibraryScenarioPerformanceCurveJoin.CriterionLibraryId = criterionId;
                        performanceCurve.CriterionLibraryScenarioPerformanceCurveJoin.ScenarioPerformanceCurveId =
                            performanceCurve.Id;
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                            performanceCurve.CriterionLibraryScenarioPerformanceCurveJoin.CriterionLibrary,
                            _unitOfWork.UserEntity?.Id);
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                            performanceCurve.CriterionLibraryScenarioPerformanceCurveJoin, _unitOfWork.UserEntity?.Id);
                    }
                });
            }

            if (simulationToClone.RemainingLifeLimits.Any())
            {
                simulationToClone.RemainingLifeLimits.ForEach(remainingLifeLimit =>
                {
                    remainingLifeLimit.Id = Guid.NewGuid();
                    remainingLifeLimit.SimulationId = simulationToClone.Id;
                    remainingLifeLimit.Simulation = null;
                    _unitOfWork.Context.ReInitializeAllEntityBaseProperties(remainingLifeLimit,
                        _unitOfWork.UserEntity?.Id);

                    if (remainingLifeLimit.CriterionLibraryScenarioRemainingLifeLimitJoin != null)
                    {
                        var criterionId = Guid.NewGuid();
                        remainingLifeLimit.CriterionLibraryScenarioRemainingLifeLimitJoin.CriterionLibrary.Id =
                            criterionId;
                        remainingLifeLimit.CriterionLibraryScenarioRemainingLifeLimitJoin.CriterionLibrary.IsSingleUse =
                            true;
                        remainingLifeLimit.CriterionLibraryScenarioRemainingLifeLimitJoin.CriterionLibraryId =
                            criterionId;
                        remainingLifeLimit.CriterionLibraryScenarioRemainingLifeLimitJoin.ScenarioRemainingLifeLimitId =
                            remainingLifeLimit.Id;
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                            remainingLifeLimit.CriterionLibraryScenarioRemainingLifeLimitJoin.CriterionLibrary,
                            _unitOfWork.UserEntity?.Id);
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                            remainingLifeLimit.CriterionLibraryScenarioRemainingLifeLimitJoin,
                            _unitOfWork.UserEntity?.Id);
                    }
                });
            }

            if (simulationToClone.ScenarioDeficientConditionGoals.Any())
            {
                simulationToClone.ScenarioDeficientConditionGoals.ForEach(goal =>
                {
                    goal.Id = Guid.NewGuid();
                    goal.SimulationId = simulationToClone.Id;
                    goal.Simulation = null;
                    _unitOfWork.Context.ReInitializeAllEntityBaseProperties(goal, _unitOfWork.UserEntity?.Id);

                    if (goal.CriterionLibraryScenarioDeficientConditionGoalJoin != null)
                    {
                        var criterionId = Guid.NewGuid();
                        goal.CriterionLibraryScenarioDeficientConditionGoalJoin.CriterionLibrary.Id = criterionId;
                        goal.CriterionLibraryScenarioDeficientConditionGoalJoin.CriterionLibrary.IsSingleUse = true;
                        goal.CriterionLibraryScenarioDeficientConditionGoalJoin.CriterionLibraryId = criterionId;
                        goal.CriterionLibraryScenarioDeficientConditionGoalJoin.ScenarioDeficientConditionGoalId =
                            goal.Id;
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                            goal.CriterionLibraryScenarioDeficientConditionGoalJoin.CriterionLibrary,
                            _unitOfWork.UserEntity?.Id);
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                            goal.CriterionLibraryScenarioDeficientConditionGoalJoin, _unitOfWork.UserEntity?.Id);
                    }
                });
            }

            if (simulationToClone.ScenarioTargetConditionalGoals.Any())
            {
                simulationToClone.ScenarioTargetConditionalGoals.ForEach(goal =>
                {
                    goal.Id = Guid.NewGuid();
                    goal.SimulationId = simulationToClone.Id;
                    goal.Simulation = null;
                    _unitOfWork.Context.ReInitializeAllEntityBaseProperties(goal, _unitOfWork.UserEntity?.Id);

                    if (goal.CriterionLibraryScenarioTargetConditionGoalJoin != null)
                    {
                        var criterionId = Guid.NewGuid();
                        goal.CriterionLibraryScenarioTargetConditionGoalJoin.CriterionLibrary.Id = criterionId;
                        goal.CriterionLibraryScenarioTargetConditionGoalJoin.CriterionLibrary.IsSingleUse = true;
                        goal.CriterionLibraryScenarioTargetConditionGoalJoin.CriterionLibraryId = criterionId;
                        goal.CriterionLibraryScenarioTargetConditionGoalJoin.ScenarioTargetConditionGoalId = goal.Id;
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                            goal.CriterionLibraryScenarioTargetConditionGoalJoin.CriterionLibrary,
                            _unitOfWork.UserEntity?.Id);
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                            goal.CriterionLibraryScenarioTargetConditionGoalJoin, _unitOfWork.UserEntity?.Id);
                    }
                });
            }

            if (simulationToClone.SelectableTreatments.Any())
            {
                simulationToClone.SelectableTreatments.ForEach(treatment =>
                {
                    treatment.Id = Guid.NewGuid();
                    treatment.SimulationId = simulationToClone.Id;
                    treatment.Simulation = null;
                    _unitOfWork.Context.ReInitializeAllEntityBaseProperties(treatment, _unitOfWork.UserEntity?.Id);

                    if (treatment.CriterionLibraryScenarioSelectableTreatmentJoin != null)
                    {
                        var criterionId = Guid.NewGuid();
                        treatment.CriterionLibraryScenarioSelectableTreatmentJoin.CriterionLibrary.Id = criterionId;
                        treatment.CriterionLibraryScenarioSelectableTreatmentJoin.CriterionLibrary.IsSingleUse = true;
                        treatment.CriterionLibraryScenarioSelectableTreatmentJoin.CriterionLibraryId = criterionId;
                        treatment.CriterionLibraryScenarioSelectableTreatmentJoin.ScenarioSelectableTreatmentId =
                            treatment.Id;
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                            treatment.CriterionLibraryScenarioSelectableTreatmentJoin.CriterionLibrary,
                            _unitOfWork.UserEntity?.Id);
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                            treatment.CriterionLibraryScenarioSelectableTreatmentJoin, _unitOfWork.UserEntity?.Id);
                    }

                    if (treatment.ScenarioTreatmentConsequences != null)
                    {
                        treatment.ScenarioTreatmentConsequences.ForEach(consequence =>
                        {
                            consequence.Id = Guid.NewGuid();
                            consequence.ScenarioSelectableTreatmentId = treatment.Id;
                            consequence.ScenarioSelectableTreatment = null;
                            _unitOfWork.Context.ReInitializeAllEntityBaseProperties(consequence,
                                _unitOfWork.UserEntity?.Id);

                            if (consequence.ScenarioConditionalTreatmentConsequenceEquationJoin != null)
                            {
                                var equationId = Guid.NewGuid();
                                consequence.ScenarioConditionalTreatmentConsequenceEquationJoin.Equation.Id =
                                    equationId;
                                consequence.ScenarioConditionalTreatmentConsequenceEquationJoin.EquationId = equationId;
                                consequence.ScenarioConditionalTreatmentConsequenceEquationJoin
                                    .ScenarioConditionalTreatmentConsequenceId = consequence.Id;
                                _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                                    consequence.ScenarioConditionalTreatmentConsequenceEquationJoin.Equation,
                                    _unitOfWork.UserEntity?.Id);
                                _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                                    consequence.ScenarioConditionalTreatmentConsequenceEquationJoin,
                                    _unitOfWork.UserEntity?.Id);
                            }

                            if (consequence.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin != null)
                            {
                                var criterionId = Guid.NewGuid();
                                consequence.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin.CriterionLibrary
                                    .Id = criterionId;
                                consequence.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin.CriterionLibrary
                                    .IsSingleUse = true;
                                consequence.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin
                                    .CriterionLibraryId = criterionId;
                                consequence.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin
                                    .ScenarioConditionalTreatmentConsequenceId = consequence.Id;
                                _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                                    consequence.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin
                                        .CriterionLibrary, _unitOfWork.UserEntity?.Id);
                                _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                                    consequence.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin,
                                    _unitOfWork.UserEntity?.Id);
                            }
                        });
                    }

                    if (treatment.ScenarioTreatmentCosts.Any())
                    {
                        treatment.ScenarioTreatmentCosts.ForEach(cost =>
                        {
                            cost.Id = Guid.NewGuid();
                            cost.ScenarioSelectableTreatmentId = treatment.Id;
                            cost.ScenarioSelectableTreatment = null;
                            _unitOfWork.Context.ReInitializeAllEntityBaseProperties(cost, _unitOfWork.UserEntity?.Id);

                            if (cost.ScenarioTreatmentCostEquationJoin != null)
                            {
                                var equationId = Guid.NewGuid();
                                cost.ScenarioTreatmentCostEquationJoin.Equation.Id = equationId;
                                cost.ScenarioTreatmentCostEquationJoin.EquationId = equationId;
                                cost.ScenarioTreatmentCostEquationJoin.ScenarioTreatmentCostId = cost.Id;
                                _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                                    cost.ScenarioTreatmentCostEquationJoin.Equation, _unitOfWork.UserEntity?.Id);
                                _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                                    cost.ScenarioTreatmentCostEquationJoin, _unitOfWork.UserEntity?.Id);
                            }

                            if (cost.CriterionLibraryScenarioTreatmentCostJoin != null)
                            {
                                var criterionId = Guid.NewGuid();
                                cost.CriterionLibraryScenarioTreatmentCostJoin.CriterionLibrary.Id = criterionId;
                                cost.CriterionLibraryScenarioTreatmentCostJoin.CriterionLibrary.IsSingleUse = true;
                                cost.CriterionLibraryScenarioTreatmentCostJoin.CriterionLibraryId = criterionId;
                                cost.CriterionLibraryScenarioTreatmentCostJoin.ScenarioTreatmentCostId = cost.Id;
                                _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                                    cost.CriterionLibraryScenarioTreatmentCostJoin.CriterionLibrary,
                                    _unitOfWork.UserEntity?.Id);
                                _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                                    cost.CriterionLibraryScenarioTreatmentCostJoin, _unitOfWork.UserEntity?.Id);
                            }
                        });
                    }

                    if (treatment.ScenarioTreatmentSchedulings.Any())
                    {
                        treatment.ScenarioTreatmentSchedulings.ForEach(scheduling =>
                        {
                            scheduling.Id = Guid.NewGuid();
                            scheduling.TreatmentId = treatment.Id;
                            _unitOfWork.Context.ReInitializeAllEntityBaseProperties(scheduling,
                                _unitOfWork.UserEntity?.Id);
                        });
                    }

                    if (treatment.ScenarioTreatmentSupersessions.Any())
                    {
                        treatment.ScenarioTreatmentSupersessions.ForEach(supersession =>
                        {
                            supersession.Id = Guid.NewGuid();
                            supersession.TreatmentId = treatment.Id;
                            _unitOfWork.Context.ReInitializeAllEntityBaseProperties(supersession,
                                _unitOfWork.UserEntity?.Id);

                            if (supersession.CriterionLibraryScenarioTreatmentSupersessionJoin != null)
                            {
                                var criterionId = Guid.NewGuid();
                                supersession.CriterionLibraryScenarioTreatmentSupersessionJoin.CriterionLibrary.Id =
                                    criterionId;
                                supersession.CriterionLibraryScenarioTreatmentSupersessionJoin.CriterionLibrary
                                    .IsSingleUse = true;
                                supersession.CriterionLibraryScenarioTreatmentSupersessionJoin.CriterionLibraryId =
                                    criterionId;
                                supersession.CriterionLibraryScenarioTreatmentSupersessionJoin.TreatmentSupersessionId =
                                    supersession.Id;
                                _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                                    supersession.CriterionLibraryScenarioTreatmentSupersessionJoin.CriterionLibrary,
                                    _unitOfWork.UserEntity?.Id);
                                _unitOfWork.Context.ReInitializeAllEntityBaseProperties(
                                    supersession.CriterionLibraryScenarioTreatmentSupersessionJoin,
                                    _unitOfWork.UserEntity?.Id);
                            }
                        });
                    }

                    if (treatment.ScenarioSelectableTreatmentScenarioBudgetJoins.Any())
                    {
                        treatment.ScenarioSelectableTreatmentScenarioBudgetJoins = treatment.ScenarioSelectableTreatmentScenarioBudgetJoins.Where(_ =>
                            simulationToClone.Budgets.Any(budget => budget.Name == _.ScenarioBudget.Name)).ToList();
                        treatment.ScenarioSelectableTreatmentScenarioBudgetJoins.ForEach(join =>
                        {
                            if (simulationToClone.Budgets.Any(_ => _.Name == join.ScenarioBudget.Name))
                            {
                                join.ScenarioSelectableTreatmentId = treatment.Id;
                                join.ScenarioBudgetId = simulationToClone.Budgets
                                    .Single(_ => _.Name == join.ScenarioBudget.Name).Id;
                                join.ScenarioBudget = null;
                                _unitOfWork.Context.ReInitializeAllEntityBaseProperties(join,
                                    _unitOfWork.UserEntity?.Id);
                            }
                        });
                    }
                });
            }

            if (_unitOfWork.UserEntity != null)
            {
                simulationToClone.SimulationUserJoins = new List<SimulationUserEntity>
                {
                    new SimulationUserEntity
                    {
                        SimulationId = simulationToClone.Id,
                        UserId = _unitOfWork.UserEntity.Id,
                        CanModify = true,
                        IsOwner = true,
                        CreatedBy = _unitOfWork.UserEntity.Id,
                        LastModifiedBy = _unitOfWork.UserEntity.Id
                    }
                };
            }

            /*_unitOfWork.Context.AddEntity(simulationToClone);*/
            // add simulation
            _unitOfWork.Context.AddAll(new List<SimulationEntity> {simulationToClone});
            _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
            // add analysis method
            _unitOfWork.Context.AddEntity(simulationToClone.AnalysisMethod);
            _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
            // add budgets
            if (simulationToClone.Budgets.Any())
            {
                _unitOfWork.Context.AddAll(simulationToClone.Budgets.ToList());
                _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                // add budget amounts
                if (simulationToClone.Budgets.Any(_ => _.ScenarioBudgetAmounts.Any()))
                {
                    _unitOfWork.Context.AddAll(simulationToClone.Budgets.Where(_ => _.ScenarioBudgetAmounts.Any())
                        .SelectMany(_ => _.ScenarioBudgetAmounts.Select(amount => amount)).ToList());
                    _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                }

                // add budget criteria
                if (simulationToClone.Budgets.Any(_ => _.CriterionLibraryScenarioBudgetJoin?.CriterionLibrary != null))
                {
                    var budgetCriteriaJoins = simulationToClone.Budgets
                        .Where(_ => _.CriterionLibraryScenarioBudgetJoin?.CriterionLibrary != null)
                        .Select(_ => _.CriterionLibraryScenarioBudgetJoin)
                        .ToList();
                    _unitOfWork.Context.AddAll(budgetCriteriaJoins.Select(_ => _.CriterionLibrary).ToList());
                    _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                    _unitOfWork.Context.AddAll(budgetCriteriaJoins);
                    _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                }
            }

            // add budget priorities
            if (simulationToClone.BudgetPriorities.Any())
            {
                _unitOfWork.Context.AddAll(simulationToClone.BudgetPriorities.ToList());
                _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                // add percentage pairs
                if (simulationToClone.BudgetPriorities.Any(_ => _.BudgetPercentagePairs.Any()))
                {
                    _unitOfWork.Context.AddAll(simulationToClone.BudgetPriorities
                        .Where(_ => _.BudgetPercentagePairs.Any())
                        .SelectMany(_ => _.BudgetPercentagePairs.Select(pair => pair))
                        .ToList());
                    _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                }

                // add budget priority criteria
                if (simulationToClone.BudgetPriorities.Any(_ =>
                    _.CriterionLibraryScenarioBudgetPriorityJoin?.CriterionLibrary != null))
                {
                    var budgetPriorityCriteriaJoins = simulationToClone.BudgetPriorities
                        .Where(_ => _.CriterionLibraryScenarioBudgetPriorityJoin?.CriterionLibrary != null)
                        .Select(_ => _.CriterionLibraryScenarioBudgetPriorityJoin)
                        .ToList();
                    _unitOfWork.Context.AddAll(budgetPriorityCriteriaJoins.Select(_ => _.CriterionLibrary).ToList());
                    _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                    _unitOfWork.Context.AddAll(budgetPriorityCriteriaJoins);
                    _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                }
            }

            // add cash flow rules
            if (simulationToClone.CashFlowRules.Any())
            {
                _unitOfWork.Context.AddAll(simulationToClone.CashFlowRules.ToList());
                _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                // add distribution rules
                if (simulationToClone.CashFlowRules.Any(_ => _.ScenarioCashFlowDistributionRules.Any()))
                {
                    _unitOfWork.Context.AddAll(simulationToClone.CashFlowRules
                        .Where(_ => _.ScenarioCashFlowDistributionRules.Any())
                        .SelectMany(_ => _.ScenarioCashFlowDistributionRules
                            .Select(distributionRule => distributionRule))
                        .ToList());
                    _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                }

                // add cash flow rule criteria
                if (simulationToClone.CashFlowRules.Any(_ =>
                    _.CriterionLibraryScenarioCashFlowRuleJoin?.CriterionLibrary != null))
                {
                    var cashFlowRuleCriteriaJoins = simulationToClone.CashFlowRules
                        .Where(_ => _.CriterionLibraryScenarioCashFlowRuleJoin?.CriterionLibrary != null)
                        .Select(_ => _.CriterionLibraryScenarioCashFlowRuleJoin).ToList();
                    _unitOfWork.Context.AddAll(cashFlowRuleCriteriaJoins.Select(_ => _.CriterionLibrary).ToList());
                    _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                    _unitOfWork.Context.AddAll(cashFlowRuleCriteriaJoins);
                    _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                }
            }

            // add investment plan
            _unitOfWork.Context.AddEntity(simulationToClone.InvestmentPlan);
            // add committed projects
            if (simulationToClone.CommittedProjects.Any())
            {
                _unitOfWork.Context.AddAll(simulationToClone.CommittedProjects.ToList());
                _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                // add committed project consequences
                if (simulationToClone.CommittedProjects.Any(_ => _.CommittedProjectConsequences.Any()))
                {
                    _unitOfWork.Context.AddAll(simulationToClone.CommittedProjects
                        .Where(_ => _.CommittedProjectConsequences.Any())
                        .SelectMany(_ => _.CommittedProjectConsequences
                            .Select(consequence => consequence))
                        .ToList());
                    _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                }
            }

            // add performance curves
            if (simulationToClone.PerformanceCurves.Any())
            {
                _unitOfWork.Context.AddAll(simulationToClone.PerformanceCurves.ToList());
                _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                // add performance curve equations
                if (simulationToClone.PerformanceCurves.Any(_ =>
                    _.ScenarioPerformanceCurveEquationJoin?.Equation != null))
                {
                    var performanceCurveEquationJoins = simulationToClone.PerformanceCurves
                        .Where(_ => _.ScenarioPerformanceCurveEquationJoin?.Equation != null)
                        .Select(_ => _.ScenarioPerformanceCurveEquationJoin).ToList();
                    _unitOfWork.Context.AddAll(performanceCurveEquationJoins.Select(_ => _.Equation).ToList());
                    _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                    _unitOfWork.Context.AddAll(performanceCurveEquationJoins);
                    _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                }
                // add performance curve criteria
                if (simulationToClone.PerformanceCurves.Any(_ => _.CriterionLibraryScenarioPerformanceCurveJoin?.CriterionLibrary != null))
                {
                    var performanceCurveCriteriaJoins = simulationToClone.PerformanceCurves
                        .Where(_ => _.CriterionLibraryScenarioPerformanceCurveJoin?.CriterionLibrary != null)
                        .Select(_ => _.CriterionLibraryScenarioPerformanceCurveJoin)
                            .ToList();
                    _unitOfWork.Context.AddAll(performanceCurveCriteriaJoins.Select(_ => _.CriterionLibrary).ToList());
                    _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                    _unitOfWork.Context.AddAll(performanceCurveCriteriaJoins);
                    _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                }
            }

            // add remaining life limits
            if (simulationToClone.RemainingLifeLimits.Any())
            {
                _unitOfWork.Context.AddAll(simulationToClone.RemainingLifeLimits.ToList());
                _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                // add remaining life limit criteria
                if (simulationToClone.RemainingLifeLimits.Any(_ =>
                    _.CriterionLibraryScenarioRemainingLifeLimitJoin?.CriterionLibrary != null))
                {
                    var remainingLifeLimitCriteriaJoins = simulationToClone.RemainingLifeLimits
                        .Where(_ => _.CriterionLibraryScenarioRemainingLifeLimitJoin?.CriterionLibrary != null)
                        .Select(_ => _.CriterionLibraryScenarioRemainingLifeLimitJoin)
                        .ToList();
                    _unitOfWork.Context.AddAll(remainingLifeLimitCriteriaJoins.Select(_ => _.CriterionLibrary)
                        .ToList());
                    _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                    _unitOfWork.Context.AddAll(remainingLifeLimitCriteriaJoins);
                    _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                }
            }

            // add deficient condition goals
            if (simulationToClone.ScenarioDeficientConditionGoals.Any())
            {
                _unitOfWork.Context.AddAll(simulationToClone.ScenarioDeficientConditionGoals.ToList());
                _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                // add deficient condition goal criteria
                if (simulationToClone.ScenarioDeficientConditionGoals.Any(_ =>
                    _.CriterionLibraryScenarioDeficientConditionGoalJoin?.CriterionLibrary != null))
                {
                    var deficientConditionGoalCriteriaJoins = simulationToClone.ScenarioDeficientConditionGoals
                        .Where(_ => _.CriterionLibraryScenarioDeficientConditionGoalJoin?.CriterionLibrary != null)
                        .Select(_ => _.CriterionLibraryScenarioDeficientConditionGoalJoin)
                        .ToList();
                    _unitOfWork.Context.AddAll(deficientConditionGoalCriteriaJoins.Select(_ => _.CriterionLibrary)
                        .ToList());
                    _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                    _unitOfWork.Context.AddAll(deficientConditionGoalCriteriaJoins);
                    _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                }
            }

            // add target condition goals
            if (simulationToClone.ScenarioTargetConditionalGoals.Any())
            {
                _unitOfWork.Context.AddAll(simulationToClone.ScenarioTargetConditionalGoals.ToList());
                _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                // add target condition goal criteria
                if (simulationToClone.ScenarioTargetConditionalGoals.Any(_ =>
                    _.CriterionLibraryScenarioTargetConditionGoalJoin?.CriterionLibrary != null))
                {
                    var targetConditionGoalCriteriaJoins = simulationToClone.ScenarioTargetConditionalGoals
                        .Where(_ => _.CriterionLibraryScenarioTargetConditionGoalJoin?.CriterionLibrary != null)
                        .Select(_ => _.CriterionLibraryScenarioTargetConditionGoalJoin)
                        .ToList();
                    _unitOfWork.Context.AddAll(
                        targetConditionGoalCriteriaJoins.Select(_ => _.CriterionLibrary).ToList());
                    _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                    _unitOfWork.Context.AddAll(targetConditionGoalCriteriaJoins);
                    _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                }
            }

            // add selectable treatments
            if (simulationToClone.SelectableTreatments.Any())
            {
                _unitOfWork.Context.AddAll(simulationToClone.SelectableTreatments.ToList());
                _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                // add selectable treatment criteria
                if (simulationToClone.SelectableTreatments.Any(_ =>
                    _.CriterionLibraryScenarioSelectableTreatmentJoin?.CriterionLibrary != null))
                {
                    var treatmentCriteriaJoins = simulationToClone.SelectableTreatments
                        .Where(_ => _.CriterionLibraryScenarioSelectableTreatmentJoin?.CriterionLibrary != null)
                        .Select(_ => _.CriterionLibraryScenarioSelectableTreatmentJoin)
                        .ToList();
                    _unitOfWork.Context.AddAll(treatmentCriteriaJoins.Select(_ => _.CriterionLibrary).ToList());
                    _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                    _unitOfWork.Context.AddAll(treatmentCriteriaJoins);
                    _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                }

                // add treatment consequences
                if (simulationToClone.SelectableTreatments.Any(_ => _.ScenarioTreatmentConsequences.Any()))
                {
                    var treatmentConsequences = simulationToClone.SelectableTreatments
                        .Where(_ => _.ScenarioTreatmentConsequences.Any())
                        .SelectMany(_ => _.ScenarioTreatmentConsequences
                            .Select(consequence => consequence))
                        .ToList();
                    _unitOfWork.Context.AddAll(treatmentConsequences);
                    _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                    // add treatment consequence equations
                    if (treatmentConsequences.Any(_ =>
                        _.ScenarioConditionalTreatmentConsequenceEquationJoin?.Equation != null))
                    {
                        var treatmentConsequenceEquationJoins = treatmentConsequences
                            .Where(_ => _.ScenarioConditionalTreatmentConsequenceEquationJoin?.Equation != null)
                            .Select(_ => _.ScenarioConditionalTreatmentConsequenceEquationJoin)
                            .ToList();
                        _unitOfWork.Context.AddAll(treatmentConsequenceEquationJoins.Select(_ => _.Equation).ToList());
                        _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                        _unitOfWork.Context.AddAll(treatmentConsequenceEquationJoins);
                        _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                    }

                    // add treatment consequence criteria
                    if (treatmentConsequences.Any(_ =>
                        _.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin?.CriterionLibrary != null))
                    {
                        var treatmentConsequenceCriteriaJoins = treatmentConsequences
                            .Where(_ =>
                                _.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin?.CriterionLibrary != null)
                            .Select(_ => _.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin)
                            .ToList();
                        _unitOfWork.Context.AddAll(treatmentConsequenceCriteriaJoins.Select(_ => _.CriterionLibrary)
                            .ToList());
                        _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                        _unitOfWork.Context.AddAll(treatmentConsequenceCriteriaJoins);
                        _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                    }
                }

                // add treatment costs
                if (simulationToClone.SelectableTreatments.Any(_ => _.ScenarioTreatmentCosts.Any()))
                {
                    var treatmentCosts = simulationToClone.SelectableTreatments
                        .Where(_ => _.ScenarioTreatmentCosts.Any())
                        .SelectMany(_ => _.ScenarioTreatmentCosts
                            .Select(consequence => consequence))
                        .ToList();
                    _unitOfWork.Context.AddAll(treatmentCosts);
                    _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                    // add treatment cost equations
                    if (treatmentCosts.Any(_ => _.ScenarioTreatmentCostEquationJoin?.Equation != null))
                    {
                        var treatmentCostEquationJoins = treatmentCosts
                            .Where(_ => _.ScenarioTreatmentCostEquationJoin?.Equation != null)
                            .Select(_ => _.ScenarioTreatmentCostEquationJoin)
                            .ToList();
                        _unitOfWork.Context.AddAll(treatmentCostEquationJoins.Select(_ => _.Equation).ToList());
                        _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                        _unitOfWork.Context.AddAll(treatmentCostEquationJoins);
                        _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                    }

                    // add treatment cost criteria
                    if (treatmentCosts.Any(_ => _.CriterionLibraryScenarioTreatmentCostJoin?.CriterionLibrary != null))
                    {
                        var treatmentCostCriteriaJoins = treatmentCosts
                            .Where(_ => _.CriterionLibraryScenarioTreatmentCostJoin?.CriterionLibrary != null)
                            .Select(_ => _.CriterionLibraryScenarioTreatmentCostJoin)
                            .ToList();
                        _unitOfWork.Context.AddAll(treatmentCostCriteriaJoins.Select(_ => _.CriterionLibrary).ToList());
                        _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                        _unitOfWork.Context.AddAll(treatmentCostCriteriaJoins);
                        _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                    }
                }

                // add treatment schedulings
                if (simulationToClone.SelectableTreatments.Any(_ => _.ScenarioTreatmentSchedulings.Any()))
                {
                    _unitOfWork.Context.AddAll(simulationToClone.SelectableTreatments
                        .Where(_ => _.ScenarioTreatmentSchedulings.Any())
                        .SelectMany(_ => _.ScenarioTreatmentSchedulings
                            .Select(scheduling => scheduling))
                        .ToList());
                    _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                }

                // add treatment supersessions
                if (simulationToClone.SelectableTreatments.Any(_ => _.ScenarioTreatmentSupersessions.Any()))
                {
                    var treatmentSupersessions = simulationToClone.SelectableTreatments
                        .Where(_ => _.ScenarioTreatmentSupersessions.Any())
                        .SelectMany(_ => _.ScenarioTreatmentSupersessions
                            .Select(scheduling => scheduling))
                        .ToList();
                    _unitOfWork.Context.AddAll(treatmentSupersessions);
                    _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                    // add treatment supersession criteria
                    if (treatmentSupersessions.Any(_ =>
                        _.CriterionLibraryScenarioTreatmentSupersessionJoin?.CriterionLibrary != null))
                    {
                        var treatmentSupersessionCriteriaJoins = treatmentSupersessions
                            .Where(_ => _.CriterionLibraryScenarioTreatmentSupersessionJoin?.CriterionLibrary != null)
                            .Select(_ => _.CriterionLibraryScenarioTreatmentSupersessionJoin)
                            .ToList();
                        _unitOfWork.Context.AddAll(treatmentSupersessionCriteriaJoins.Select(_ => _.CriterionLibrary)
                            .ToList());
                        _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                        _unitOfWork.Context.AddAll(treatmentSupersessionCriteriaJoins);
                        _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                    }
                }
            }

            // add simulation user
            if (simulationToClone.SimulationUserJoins.Any())
            {
                _unitOfWork.Context.AddAll(simulationToClone.SimulationUserJoins.ToList());
                _unitOfWork.Context.Database.ExecuteSqlRaw("CHECKPOINT");
                simulationToClone.SimulationUserJoins.ToList()
                    .ForEach(join => join.User = _unitOfWork.UserEntity ?? new UserEntity
                    {
                        Id = Guid.Empty, Username = "NA"
                    });
            }

            return new SimulationCloningResultDTO
            {
                Simulation = simulationToClone.ToDto(_unitOfWork.UserEntity),
                WarningMessage = budgetsPreventingCloning.Any() && numberOfCommittedProjectsAffected > 0
                    ? $"The following committed project budgets were not found which has prevented {numberOfCommittedProjectsAffected} committed project(s) from being cloned: {string.Join(", ", budgetsPreventingCloning)}"
                    : null
            };
        }

        public void UpdateSimulation(SimulationDTO dto)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == dto.Id))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            var simulationEntity = _unitOfWork.Context.Simulation.Single(_ => _.Id == dto.Id);
            if (simulationEntity.Name != dto.Name)
            {
                simulationEntity.Name = dto.Name;

                _unitOfWork.Context.UpdateEntity(simulationEntity, dto.Id, _unitOfWork.UserEntity?.Id);
            }

            if (dto.Users.Any())
            {
                _unitOfWork.Context.DeleteAll<SimulationUserEntity>(_ => _.SimulationId == dto.Id);
                _unitOfWork.Context.AddAll(dto.Users.Select(_ => _.ToEntity(dto.Id)).ToList(),
                    _unitOfWork.UserEntity?.Id);
            }
        }

        public void DeleteSimulation(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                return;
            }

            _unitOfWork.Context.DeleteAll<BudgetPercentagePairEntity>(_ =>
                _.ScenarioBudgetPriority.SimulationId == simulationId || _.ScenarioBudget.SimulationId == simulationId);

            _unitOfWork.Context.DeleteAll<ScenarioSelectableTreatmentScenarioBudgetEntity>(_ =>
                _.ScenarioSelectableTreatment.SimulationId == simulationId || _.ScenarioBudget.SimulationId == simulationId);

            _unitOfWork.Context.DeleteAll<CommittedProjectEntity>(_ =>
                _.SimulationId == simulationId || _.ScenarioBudget.SimulationId == simulationId);

            _unitOfWork.Context.DeleteEntity<SimulationEntity>(_ => _.Id == simulationId);
        }

        // the method is used only by other repositories.
        public void UpdateLastModifiedDate(SimulationEntity entity)
        {
            entity.LastModifiedDate = DateTime.Now; // updating the last modified date
            _unitOfWork.Context.Upsert(entity, entity.Id, _unitOfWork.UserEntity?.Id);
        }
    }
}
