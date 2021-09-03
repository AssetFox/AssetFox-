using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SimulationRepository : ISimulationRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public SimulationRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ??
                                         throw new ArgumentNullException(nameof(unitOfWork));

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
            var simulationAnalysisDetail = _unitOfWork.Context.SimulationAnalysisDetail.AsNoTracking().SingleOrDefault(_ => _.SimulationId == simulationId);

            simulationEntity.CreateSimulation(network, simulationAnalysisDetail.LastRun, simulationAnalysisDetail.LastModifiedDate);
        }

        public void CreateSimulation(Guid networkId, SimulationDTO dto)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            var simulationEntity = dto.ToEntity(networkId);

            _unitOfWork.Context.AddEntity(simulationEntity, _unitOfWork.UserEntity?.Id);

            if (dto.Users.Any())
            {
                _unitOfWork.Context.AddAll(dto.Users.Select(_ => _.ToEntity(dto.Id)).ToList(),
                    _unitOfWork.UserEntity?.Id);
            }
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

        public SimulationDTO CloneSimulation(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            var simulationToClone = _unitOfWork.Context.Simulation.AsNoTracking()
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
                _unitOfWork.Context.ReInitializeAllEntityBaseProperties(simulationToClone.AnalysisMethod, _unitOfWork.UserEntity?.Id);

                if (simulationToClone.AnalysisMethod.Benefit != null)
                {
                    simulationToClone.AnalysisMethod.Benefit.Id = Guid.NewGuid();
                    simulationToClone.AnalysisMethod.Benefit.AnalysisMethodId = simulationToClone.AnalysisMethod.Id;
                    _unitOfWork.Context.ReInitializeAllEntityBaseProperties(simulationToClone.AnalysisMethod.Benefit, _unitOfWork.UserEntity?.Id);
                }

                if (simulationToClone.AnalysisMethod.CriterionLibraryAnalysisMethodJoin != null)
                {
                    var criterionId = Guid.NewGuid();
                    simulationToClone.AnalysisMethod.CriterionLibraryAnalysisMethodJoin.CriterionLibrary.Id = criterionId;
                    simulationToClone.AnalysisMethod.CriterionLibraryAnalysisMethodJoin.CriterionLibrary.IsSingleUse = true;
                    simulationToClone.AnalysisMethod.CriterionLibraryAnalysisMethodJoin.CriterionLibraryId = criterionId;
                    simulationToClone.AnalysisMethod.CriterionLibraryAnalysisMethodJoin.AnalysisMethodId = simulationToClone.AnalysisMethod.Id;
                    _unitOfWork.Context.ReInitializeAllEntityBaseProperties(simulationToClone.AnalysisMethod.CriterionLibraryAnalysisMethodJoin.CriterionLibrary, _unitOfWork.UserEntity?.Id);
                    _unitOfWork.Context.ReInitializeAllEntityBaseProperties(simulationToClone.AnalysisMethod.CriterionLibraryAnalysisMethodJoin, _unitOfWork.UserEntity?.Id);
                }
            }

            if (simulationToClone.Budgets.Any())
            {
                simulationToClone.Budgets.ToList().ForEach(budget =>
                {
                    budget.Id = Guid.NewGuid();
                    _unitOfWork.Context.ReInitializeAllEntityBaseProperties(budget, _unitOfWork.UserEntity?.Id);

                    if (budget.ScenarioBudgetAmounts.Any())
                    {
                        budget.ScenarioBudgetAmounts.ForEach(budgetAmount =>
                        {
                            budgetAmount.Id = Guid.NewGuid();
                            budgetAmount.ScenarioBudgetId = budget.Id;
                            _unitOfWork.Context.ReInitializeAllEntityBaseProperties(budgetAmount, _unitOfWork.UserEntity?.Id);
                        });
                    }

                    if (budget.CriterionLibraryScenarioBudgetJoin != null)
                    {
                        var criterionId = Guid.NewGuid();
                        budget.CriterionLibraryScenarioBudgetJoin.CriterionLibrary.Id = criterionId;
                        budget.CriterionLibraryScenarioBudgetJoin.CriterionLibrary.IsSingleUse = true;
                        budget.CriterionLibraryScenarioBudgetJoin.CriterionLibraryId = criterionId;
                        budget.CriterionLibraryScenarioBudgetJoin.ScenarioBudgetId = budget.Id;
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(budget.CriterionLibraryScenarioBudgetJoin.CriterionLibrary, _unitOfWork.UserEntity?.Id);
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(budget.CriterionLibraryScenarioBudgetJoin, _unitOfWork.UserEntity?.Id);
                    }
                });
            }

            if (simulationToClone.BudgetPriorities.Any())
            {
                simulationToClone.BudgetPriorities.ForEach(budgetPriority =>
                {
                    budgetPriority.Id = Guid.NewGuid();
                    _unitOfWork.Context.ReInitializeAllEntityBaseProperties(budgetPriority, _unitOfWork.UserEntity?.Id);

                    if (budgetPriority.BudgetPercentagePairs.Any())
                    {
                        budgetPriority.BudgetPercentagePairs.ForEach(percentagePair =>
                        {
                            percentagePair.Id = Guid.NewGuid();
                            percentagePair.ScenarioBudgetId = simulationToClone.Budgets.Single(_ => _.Name == percentagePair.ScenarioBudget.Name).Id;
                            percentagePair.ScenarioBudget = null;
                            _unitOfWork.Context.ReInitializeAllEntityBaseProperties(percentagePair, _unitOfWork.UserEntity?.Id);
                        });
                    }

                    if (budgetPriority.CriterionLibraryScenarioBudgetPriorityJoin != null)
                    {
                        var criterionId = Guid.NewGuid();
                        budgetPriority.CriterionLibraryScenarioBudgetPriorityJoin.CriterionLibrary.Id = criterionId;
                        budgetPriority.CriterionLibraryScenarioBudgetPriorityJoin.CriterionLibrary.IsSingleUse = true;
                        budgetPriority.CriterionLibraryScenarioBudgetPriorityJoin.CriterionLibraryId = criterionId;
                        budgetPriority.CriterionLibraryScenarioBudgetPriorityJoin.ScenarioBudgetPriorityId = budgetPriority.Id;
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(budgetPriority.CriterionLibraryScenarioBudgetPriorityJoin.CriterionLibrary, _unitOfWork.UserEntity?.Id);
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(budgetPriority.CriterionLibraryScenarioBudgetPriorityJoin, _unitOfWork.UserEntity?.Id);
                    }

                });
            }

            if (simulationToClone.CashFlowRules.Any())
            {
                simulationToClone.CashFlowRules.ForEach(cashFlowRule =>
                {
                    cashFlowRule.Id = Guid.NewGuid();
                    _unitOfWork.Context.ReInitializeAllEntityBaseProperties(cashFlowRule, _unitOfWork.UserEntity?.Id);

                    if (cashFlowRule.ScenarioCashFlowDistributionRules.Any())
                    {
                        cashFlowRule.ScenarioCashFlowDistributionRules.ForEach(distributionRule =>
                        {
                            distributionRule.Id = Guid.NewGuid();
                            distributionRule.ScenarioCashFlowRuleId = cashFlowRule.Id;
                            _unitOfWork.Context.ReInitializeAllEntityBaseProperties(distributionRule, _unitOfWork.UserEntity?.Id);
                        });
                    }

                    if (cashFlowRule.CriterionLibraryScenarioCashFlowRuleJoin != null)
                    {
                        var criterionId = Guid.NewGuid();
                        cashFlowRule.CriterionLibraryScenarioCashFlowRuleJoin.CriterionLibrary.Id = criterionId;
                        cashFlowRule.CriterionLibraryScenarioCashFlowRuleJoin.CriterionLibrary.IsSingleUse = true;
                        cashFlowRule.CriterionLibraryScenarioCashFlowRuleJoin.CriterionLibraryId = criterionId;
                        cashFlowRule.CriterionLibraryScenarioCashFlowRuleJoin.ScenarioCashFlowRuleId = cashFlowRule.Id;
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(cashFlowRule.CriterionLibraryScenarioCashFlowRuleJoin.CriterionLibrary, _unitOfWork.UserEntity?.Id);
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(cashFlowRule.CriterionLibraryScenarioCashFlowRuleJoin, _unitOfWork.UserEntity?.Id);
                    }

                });
            }

            if (simulationToClone.InvestmentPlan != null)
            {
                simulationToClone.InvestmentPlan.Id = Guid.NewGuid();
                _unitOfWork.Context.ReInitializeAllEntityBaseProperties(simulationToClone.InvestmentPlan, _unitOfWork.UserEntity?.Id);
            }

            if (simulationToClone.CommittedProjects.Any())
            {
                simulationToClone.CommittedProjects.ForEach(committedProject =>
                {
                    committedProject.Id = Guid.NewGuid();
                    committedProject.ScenarioBudgetId = simulationToClone.Budgets.Single(_ => _.Name == committedProject.ScenarioBudget.Name).Id;
                    committedProject.ScenarioBudget = null;
                    _unitOfWork.Context.ReInitializeAllEntityBaseProperties(committedProject, _unitOfWork.UserEntity?.Id);

                    if (committedProject.CommittedProjectConsequences.Any())
                    {
                        committedProject.CommittedProjectConsequences.ForEach(consequence =>
                        {
                            consequence.Id = Guid.NewGuid();
                            consequence.CommittedProjectId = committedProject.Id;
                            _unitOfWork.Context.ReInitializeAllEntityBaseProperties(consequence, _unitOfWork.UserEntity?.Id);
                        });
                    }
                });
            }

            if (simulationToClone.PerformanceCurves.Any())
            {
                simulationToClone.PerformanceCurves.ForEach(performanceCurve =>
                {
                    performanceCurve.Id = Guid.NewGuid();
                    _unitOfWork.Context.ReInitializeAllEntityBaseProperties(performanceCurve, _unitOfWork.UserEntity?.Id);

                    if (performanceCurve.ScenarioPerformanceCurveEquationJoin != null)
                    {
                        var equationId = Guid.NewGuid();
                        performanceCurve.ScenarioPerformanceCurveEquationJoin.Equation.Id = equationId;
                        performanceCurve.ScenarioPerformanceCurveEquationJoin.EquationId = equationId;
                        performanceCurve.ScenarioPerformanceCurveEquationJoin.ScenarioPerformanceCurveId = performanceCurve.Id;
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(performanceCurve.ScenarioPerformanceCurveEquationJoin.Equation, _unitOfWork.UserEntity?.Id);
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(performanceCurve.ScenarioPerformanceCurveEquationJoin, _unitOfWork.UserEntity?.Id);
                    }

                    if (performanceCurve.CriterionLibraryScenarioPerformanceCurveJoin != null)
                    {
                        var criterionId = Guid.NewGuid();
                        performanceCurve.CriterionLibraryScenarioPerformanceCurveJoin.CriterionLibrary.Id = criterionId;
                        performanceCurve.CriterionLibraryScenarioPerformanceCurveJoin.CriterionLibrary.IsSingleUse = true;
                        performanceCurve.CriterionLibraryScenarioPerformanceCurveJoin.CriterionLibraryId = criterionId;
                        performanceCurve.CriterionLibraryScenarioPerformanceCurveJoin.ScenarioPerformanceCurveId = performanceCurve.Id;
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(performanceCurve.CriterionLibraryScenarioPerformanceCurveJoin.CriterionLibrary, _unitOfWork.UserEntity?.Id);
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(performanceCurve.CriterionLibraryScenarioPerformanceCurveJoin, _unitOfWork.UserEntity?.Id);
                    }
                });
            }

            if (simulationToClone.RemainingLifeLimits.Any())
            {
                simulationToClone.RemainingLifeLimits.ForEach(remainingLifeLimit =>
                {
                    remainingLifeLimit.Id = Guid.NewGuid();
                    _unitOfWork.Context.ReInitializeAllEntityBaseProperties(remainingLifeLimit, _unitOfWork.UserEntity?.Id);

                    if (remainingLifeLimit.CriterionLibraryScenarioRemainingLifeLimitJoin != null)
                    {
                        var criterionId = Guid.NewGuid();
                        remainingLifeLimit.CriterionLibraryScenarioRemainingLifeLimitJoin.CriterionLibrary.Id = criterionId;
                        remainingLifeLimit.CriterionLibraryScenarioRemainingLifeLimitJoin.CriterionLibrary.IsSingleUse = true;
                        remainingLifeLimit.CriterionLibraryScenarioRemainingLifeLimitJoin.CriterionLibraryId = criterionId;
                        remainingLifeLimit.CriterionLibraryScenarioRemainingLifeLimitJoin.ScenarioRemainingLifeLimitId = remainingLifeLimit.Id;
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(remainingLifeLimit.CriterionLibraryScenarioRemainingLifeLimitJoin.CriterionLibrary, _unitOfWork.UserEntity?.Id);
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(remainingLifeLimit.CriterionLibraryScenarioRemainingLifeLimitJoin, _unitOfWork.UserEntity?.Id);
                    }
                });
            }

            if (simulationToClone.ScenarioDeficientConditionGoals.Any())
            {
                simulationToClone.ScenarioDeficientConditionGoals.ForEach(goal =>
                {
                    goal.Id = Guid.NewGuid();
                    _unitOfWork.Context.ReInitializeAllEntityBaseProperties(goal, _unitOfWork.UserEntity?.Id);

                    if (goal.CriterionLibraryScenarioDeficientConditionGoalJoin != null)
                    {
                        var criterionId = Guid.NewGuid();
                        goal.CriterionLibraryScenarioDeficientConditionGoalJoin.CriterionLibrary.Id = criterionId;
                        goal.CriterionLibraryScenarioDeficientConditionGoalJoin.CriterionLibrary.IsSingleUse = true;
                        goal.CriterionLibraryScenarioDeficientConditionGoalJoin.CriterionLibraryId = criterionId;
                        goal.CriterionLibraryScenarioDeficientConditionGoalJoin.ScenarioDeficientConditionGoalId = goal.Id;
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(goal.CriterionLibraryScenarioDeficientConditionGoalJoin.CriterionLibrary, _unitOfWork.UserEntity?.Id);
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(goal.CriterionLibraryScenarioDeficientConditionGoalJoin, _unitOfWork.UserEntity?.Id);
                    }
                });
            }

            if (simulationToClone.ScenarioTargetConditionalGoals.Any())
            {
                simulationToClone.ScenarioTargetConditionalGoals.ForEach(goal =>
                {
                    goal.Id = Guid.NewGuid();
                    _unitOfWork.Context.ReInitializeAllEntityBaseProperties(goal, _unitOfWork.UserEntity?.Id);

                    if (goal.CriterionLibraryScenarioTargetConditionGoalJoin != null)
                    {
                        var criterionId = Guid.NewGuid();
                        goal.CriterionLibraryScenarioTargetConditionGoalJoin.CriterionLibrary.Id = criterionId;
                        goal.CriterionLibraryScenarioTargetConditionGoalJoin.CriterionLibrary.IsSingleUse = true;
                        goal.CriterionLibraryScenarioTargetConditionGoalJoin.CriterionLibraryId = criterionId;
                        goal.CriterionLibraryScenarioTargetConditionGoalJoin.ScenarioTargetConditionGoalId = goal.Id;
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(goal.CriterionLibraryScenarioTargetConditionGoalJoin.CriterionLibrary, _unitOfWork.UserEntity?.Id);
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(goal.CriterionLibraryScenarioTargetConditionGoalJoin, _unitOfWork.UserEntity?.Id);
                    }
                });
            }

            if (simulationToClone.SelectableTreatments.Any())
            {
                simulationToClone.SelectableTreatments.ForEach(treatment =>
                {
                    treatment.Id = Guid.NewGuid();
                    _unitOfWork.Context.ReInitializeAllEntityBaseProperties(treatment, _unitOfWork.UserEntity?.Id);

                    if (treatment.CriterionLibraryScenarioSelectableTreatmentJoin != null)
                    {
                        var criterionId = Guid.NewGuid();
                        treatment.CriterionLibraryScenarioSelectableTreatmentJoin.CriterionLibrary.Id = criterionId;
                        treatment.CriterionLibraryScenarioSelectableTreatmentJoin.CriterionLibrary.IsSingleUse = true;
                        treatment.CriterionLibraryScenarioSelectableTreatmentJoin.CriterionLibraryId = criterionId;
                        treatment.CriterionLibraryScenarioSelectableTreatmentJoin.ScenarioSelectableTreatmentId = treatment.Id;
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(treatment.CriterionLibraryScenarioSelectableTreatmentJoin.CriterionLibrary, _unitOfWork.UserEntity?.Id);
                        _unitOfWork.Context.ReInitializeAllEntityBaseProperties(treatment.CriterionLibraryScenarioSelectableTreatmentJoin, _unitOfWork.UserEntity?.Id);
                    }

                    if (treatment.ScenarioTreatmentConsequences != null)
                    {
                        treatment.ScenarioTreatmentConsequences.ForEach(consequence =>
                        {
                            consequence.Id = Guid.NewGuid();
                            consequence.ScenarioSelectableTreatmentId = treatment.Id;
                            consequence.ScenarioSelectableTreatment = null;
                            _unitOfWork.Context.ReInitializeAllEntityBaseProperties(consequence, _unitOfWork.UserEntity?.Id);

                            if (consequence.ScenarioConditionalTreatmentConsequenceEquationJoin != null)
                            {
                                var equationId = Guid.NewGuid();
                                consequence.ScenarioConditionalTreatmentConsequenceEquationJoin.Equation.Id = equationId;
                                consequence.ScenarioConditionalTreatmentConsequenceEquationJoin.EquationId = equationId;
                                consequence.ScenarioConditionalTreatmentConsequenceEquationJoin.ScenarioConditionalTreatmentConsequenceId = consequence.Id;
                                _unitOfWork.Context.ReInitializeAllEntityBaseProperties(consequence.ScenarioConditionalTreatmentConsequenceEquationJoin.Equation, _unitOfWork.UserEntity?.Id);
                                _unitOfWork.Context.ReInitializeAllEntityBaseProperties(consequence.ScenarioConditionalTreatmentConsequenceEquationJoin, _unitOfWork.UserEntity?.Id);
                            }

                            if (consequence.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin != null)
                            {
                                var criterionId = Guid.NewGuid();
                                consequence.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin.CriterionLibrary.Id = criterionId;
                                consequence.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin.CriterionLibrary.IsSingleUse = true;
                                consequence.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin.CriterionLibraryId = criterionId;
                                consequence.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin.ScenarioConditionalTreatmentConsequenceId = consequence.Id;
                                _unitOfWork.Context.ReInitializeAllEntityBaseProperties(consequence.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin.CriterionLibrary, _unitOfWork.UserEntity?.Id);
                                _unitOfWork.Context.ReInitializeAllEntityBaseProperties(consequence.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin, _unitOfWork.UserEntity?.Id);
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
                                _unitOfWork.Context.ReInitializeAllEntityBaseProperties(cost.ScenarioTreatmentCostEquationJoin.Equation, _unitOfWork.UserEntity?.Id);
                                _unitOfWork.Context.ReInitializeAllEntityBaseProperties(cost.ScenarioTreatmentCostEquationJoin, _unitOfWork.UserEntity?.Id);
                            }

                            if (cost.CriterionLibraryScenarioTreatmentCostJoin != null)
                            {
                                var criterionId = Guid.NewGuid();
                                cost.CriterionLibraryScenarioTreatmentCostJoin.CriterionLibrary.Id = criterionId;
                                cost.CriterionLibraryScenarioTreatmentCostJoin.CriterionLibrary.IsSingleUse = true;
                                cost.CriterionLibraryScenarioTreatmentCostJoin.CriterionLibraryId = criterionId;
                                cost.CriterionLibraryScenarioTreatmentCostJoin.ScenarioTreatmentCostId = cost.Id;
                                _unitOfWork.Context.ReInitializeAllEntityBaseProperties(cost.CriterionLibraryScenarioTreatmentCostJoin.CriterionLibrary, _unitOfWork.UserEntity?.Id);
                                _unitOfWork.Context.ReInitializeAllEntityBaseProperties(cost.CriterionLibraryScenarioTreatmentCostJoin, _unitOfWork.UserEntity?.Id);
                            }
                        });
                    }

                    if (treatment.ScenarioTreatmentSchedulings.Any())
                    {
                        treatment.ScenarioTreatmentSchedulings.ForEach(scheduling =>
                        {
                            scheduling.Id = Guid.NewGuid();
                            scheduling.TreatmentId = treatment.Id;
                            _unitOfWork.Context.ReInitializeAllEntityBaseProperties(scheduling, _unitOfWork.UserEntity?.Id);
                        });
                    }

                    if (treatment.ScenarioTreatmentSupersessions.Any())
                    {
                        treatment.ScenarioTreatmentSupersessions.ForEach(supersession =>
                        {
                            supersession.Id = Guid.NewGuid();
                            supersession.TreatmentId = treatment.Id;
                            _unitOfWork.Context.ReInitializeAllEntityBaseProperties(supersession, _unitOfWork.UserEntity?.Id);

                            if (supersession.CriterionLibraryScenarioTreatmentSupersessionJoin != null)
                            {
                                var criterionId = Guid.NewGuid();
                                supersession.CriterionLibraryScenarioTreatmentSupersessionJoin.CriterionLibrary.Id = criterionId;
                                supersession.CriterionLibraryScenarioTreatmentSupersessionJoin.CriterionLibrary.IsSingleUse = true;
                                supersession.CriterionLibraryScenarioTreatmentSupersessionJoin.CriterionLibraryId = criterionId;
                                supersession.CriterionLibraryScenarioTreatmentSupersessionJoin.TreatmentSupersessionId = supersession.Id;
                                _unitOfWork.Context.ReInitializeAllEntityBaseProperties(supersession.CriterionLibraryScenarioTreatmentSupersessionJoin.CriterionLibrary, _unitOfWork.UserEntity?.Id);
                                _unitOfWork.Context.ReInitializeAllEntityBaseProperties(supersession.CriterionLibraryScenarioTreatmentSupersessionJoin, _unitOfWork.UserEntity?.Id);
                            }
                        });
                    }

                    if (treatment.ScenarioSelectableTreatmentScenarioBudgetJoins.Any())
                    {
                        treatment.ScenarioSelectableTreatmentScenarioBudgetJoins.ForEach(join =>
                        {
                            join.ScenarioSelectableTreatmentId = treatment.Id;
                            join.ScenarioBudgetId = simulationToClone.Budgets.Single(_ => _.Name == join.ScenarioBudget.Name).Id;
                            join.ScenarioBudget = null;
                            _unitOfWork.Context.ReInitializeAllEntityBaseProperties(join, _unitOfWork.UserEntity?.Id);
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

            _unitOfWork.Context.AddEntity(simulationToClone);

            return simulationToClone.ToDto(_unitOfWork.UserEntity);
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
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId) &&
                !_unitOfWork.Context.CommittedProject.Any(_ => _.SimulationId == null))
            {
                return;
            }

            _unitOfWork.Context.DeleteAll<CommittedProjectEntity>(_ => _.SimulationId == simulationId);

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
