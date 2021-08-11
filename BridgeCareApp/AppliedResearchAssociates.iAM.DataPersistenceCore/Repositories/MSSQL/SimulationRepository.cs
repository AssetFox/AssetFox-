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

        public SimulationDTO CloneSimulation(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            var simulationToClone = _unitOfWork.Context.Simulation.AsNoTracking()
                .Include(_ => _.AnalysisMethod)
                .ThenInclude(_ => _.Benefit)
                .Include(_ => _.AnalysisMethod)
                .ThenInclude(_ => _.CriterionLibraryAnalysisMethodJoin)
                .Include(_ => _.InvestmentPlan)
                .Include(_ => _.Budgets)
                .ThenInclude(_ => _.ScenarioBudgetAmounts)
                .Include(_ => _.Budgets)
                .ThenInclude(_ => _.CriterionLibraryScenarioBudgetJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.BudgetPriorityLibrarySimulationJoin)
                .Include(_ => _.CashFlowRuleLibrarySimulationJoin)
                .Include(_ => _.DeficientConditionGoalLibrarySimulationJoin)
                .Include(_ => _.PerformanceCurves)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.PerformanceCurves)
                .ThenInclude(_ => _.CriterionLibraryScenarioPerformanceCurveJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.PerformanceCurves)
                .ThenInclude(_ => _.ScenarioPerformanceCurveEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.RemainingLifeLimitLibrarySimulationJoin)
                .Include(_ => _.TargetConditionGoalLibrarySimulationJoin)

                .Include(_ => _.SelectableTreatments)
                .ThenInclude(_ => _.ScenarioSelectableTreatmentScenarioBudgetJoins)
                .ThenInclude(_ => _.ScenarioBudget)
                .ThenInclude(_ => _.ScenarioBudgetAmounts)
                .Include(_ => _.SelectableTreatments)
                .ThenInclude(_ => _.ScenarioTreatmentConsequences)
                .ThenInclude(_ => _.Attribute)
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
                .ThenInclude(_ => _.CriterionLibraryScenarioSelectableTreatmentJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.SelectableTreatments)
                .ThenInclude(_ => _.ScenarioTreatmentSchedulings)
                .Include(_ => _.SelectableTreatments)
                .ThenInclude(_ => _.ScenarioTreatmentSupersessions)

                .Include(_ => _.CommittedProjects)
                .ThenInclude(_ => _.CommittedProjectConsequences)
                .Single(_ => _.Id == simulationId);

            var newSimulationId = Guid.NewGuid();

            simulationToClone.Id = newSimulationId;
            _unitOfWork.Context
                .ReInitializeAllEntityBaseProperties(simulationToClone, _unitOfWork.UserEntity?.Id);

            if (simulationToClone.AnalysisMethod != null)
            {
                var newAnalysisMethodId = Guid.NewGuid();
                simulationToClone.AnalysisMethod.Id = newAnalysisMethodId;
                simulationToClone.AnalysisMethod.SimulationId = newSimulationId;
                _unitOfWork.Context
                    .ReInitializeAllEntityBaseProperties(simulationToClone.AnalysisMethod, _unitOfWork.UserEntity?.Id);

                if (simulationToClone.AnalysisMethod.Benefit != null)
                {
                    simulationToClone.AnalysisMethod.Benefit.Id = Guid.NewGuid();
                    simulationToClone.AnalysisMethod.Benefit.AnalysisMethodId = newAnalysisMethodId;
                    _unitOfWork.Context
                        .ReInitializeAllEntityBaseProperties(simulationToClone.AnalysisMethod.Benefit,
                            _unitOfWork.UserEntity?.Id);
                }

                if (simulationToClone.AnalysisMethod.CriterionLibraryAnalysisMethodJoin != null)
                {
                    simulationToClone.AnalysisMethod.CriterionLibraryAnalysisMethodJoin.AnalysisMethodId =
                        newAnalysisMethodId;
                    _unitOfWork.Context
                        .ReInitializeAllEntityBaseProperties(
                            simulationToClone.AnalysisMethod.CriterionLibraryAnalysisMethodJoin,
                            _unitOfWork.UserEntity?.Id);
                }
            }

            if (simulationToClone.InvestmentPlan != null)
            {
                simulationToClone.InvestmentPlan.Id = Guid.NewGuid();
                simulationToClone.InvestmentPlan.SimulationId = newSimulationId;
                _unitOfWork.Context
                    .ReInitializeAllEntityBaseProperties(simulationToClone.InvestmentPlan, _unitOfWork.UserEntity?.Id);
            }

            if (simulationToClone.Budgets.Any())
            {
                simulationToClone.Budgets.ForEach(budget =>
                {
                    var newBudgetId = Guid.NewGuid();
                    budget.Id = newBudgetId;
                    budget.SimulationId = simulationId;
                    _unitOfWork.Context
                        .ReInitializeAllEntityBaseProperties(budget, _unitOfWork.UserEntity?.Id);

                    if (budget.CriterionLibraryScenarioBudgetJoin != null)
                    {
                        var criterionLibraryId = Guid.NewGuid();
                        budget.CriterionLibraryScenarioBudgetJoin.CriterionLibrary.Id = criterionLibraryId;
                        budget.CriterionLibraryScenarioBudgetJoin.CriterionLibraryId = criterionLibraryId;
                        budget.CriterionLibraryScenarioBudgetJoin.ScenarioBudgetId = newBudgetId;
                        _unitOfWork.Context
                            .ReInitializeAllEntityBaseProperties(
                                budget.CriterionLibraryScenarioBudgetJoin.CriterionLibrary,
                                _unitOfWork.UserEntity?.Id);
                        _unitOfWork.Context
                            .ReInitializeAllEntityBaseProperties(budget.CriterionLibraryScenarioBudgetJoin,
                                _unitOfWork.UserEntity?.Id);
                    }
                });
            }

            if (simulationToClone.BudgetPriorityLibrarySimulationJoin != null)
            {
                simulationToClone.BudgetPriorityLibrarySimulationJoin.SimulationId = newSimulationId;
                _unitOfWork.Context
                    .ReInitializeAllEntityBaseProperties(simulationToClone.BudgetPriorityLibrarySimulationJoin,
                        _unitOfWork.UserEntity?.Id);
            }

            if (simulationToClone.CashFlowRuleLibrarySimulationJoin != null)
            {
                simulationToClone.CashFlowRuleLibrarySimulationJoin.SimulationId = newSimulationId;
                _unitOfWork.Context
                    .ReInitializeAllEntityBaseProperties(simulationToClone.CashFlowRuleLibrarySimulationJoin,
                        _unitOfWork.UserEntity?.Id);
            }

            if (simulationToClone.DeficientConditionGoalLibrarySimulationJoin != null)
            {
                simulationToClone.DeficientConditionGoalLibrarySimulationJoin.SimulationId = newSimulationId;
                _unitOfWork.Context
                    .ReInitializeAllEntityBaseProperties(
                        simulationToClone.DeficientConditionGoalLibrarySimulationJoin, _unitOfWork.UserEntity?.Id);
            }

            if (simulationToClone.PerformanceCurves.Any())
            {
                simulationToClone.PerformanceCurves.ForEach(curve =>
                {
                    var newCurveId = Guid.NewGuid();
                    curve.Id = newCurveId;
                    curve.SimulationId = simulationId;
                    curve.Attribute = null;
                    _unitOfWork.Context
                        .ReInitializeAllEntityBaseProperties(curve, _unitOfWork.UserEntity?.Id);

                    if (curve.CriterionLibraryScenarioPerformanceCurveJoin != null)
                    {
                        var criterionLibraryId = Guid.NewGuid();
                        curve.CriterionLibraryScenarioPerformanceCurveJoin.CriterionLibrary.Id = criterionLibraryId;
                        curve.CriterionLibraryScenarioPerformanceCurveJoin.CriterionLibraryId = criterionLibraryId;
                        curve.CriterionLibraryScenarioPerformanceCurveJoin.ScenarioPerformanceCurveId = newCurveId;
                        _unitOfWork.Context
                            .ReInitializeAllEntityBaseProperties(
                                curve.CriterionLibraryScenarioPerformanceCurveJoin.CriterionLibrary,
                                _unitOfWork.UserEntity?.Id);
                        _unitOfWork.Context
                            .ReInitializeAllEntityBaseProperties(curve.CriterionLibraryScenarioPerformanceCurveJoin,
                                _unitOfWork.UserEntity?.Id);
                    }

                    if (curve.ScenarioPerformanceCurveEquationJoin != null)
                    {
                        var newEquationId = Guid.NewGuid();
                        curve.ScenarioPerformanceCurveEquationJoin.Equation.Id = newEquationId;
                        curve.ScenarioPerformanceCurveEquationJoin.EquationId = newEquationId;
                        curve.ScenarioPerformanceCurveEquationJoin.ScenarioPerformanceCurveId = newCurveId;
                        _unitOfWork.Context
                            .ReInitializeAllEntityBaseProperties(
                                curve.ScenarioPerformanceCurveEquationJoin.Equation,
                                _unitOfWork.UserEntity?.Id);
                        _unitOfWork.Context
                            .ReInitializeAllEntityBaseProperties(curve.ScenarioPerformanceCurveEquationJoin,
                                _unitOfWork.UserEntity?.Id);
                    }
                });

            }

            if (simulationToClone.RemainingLifeLimitLibrarySimulationJoin != null)
            {
                simulationToClone.RemainingLifeLimitLibrarySimulationJoin.SimulationId = newSimulationId;
                _unitOfWork.Context
                    .ReInitializeAllEntityBaseProperties(simulationToClone.RemainingLifeLimitLibrarySimulationJoin,
                        _unitOfWork.UserEntity?.Id);
            }

            if (simulationToClone.TargetConditionGoalLibrarySimulationJoin != null)
            {
                simulationToClone.TargetConditionGoalLibrarySimulationJoin.SimulationId = newSimulationId;
                _unitOfWork.Context
                    .ReInitializeAllEntityBaseProperties(simulationToClone.TargetConditionGoalLibrarySimulationJoin,
                        _unitOfWork.UserEntity?.Id);
            }

            if (simulationToClone.SelectableTreatments.Any())
            {
                simulationToClone.SelectableTreatments.ForEach(treatment =>
                {
                    var newTreatmentId = Guid.NewGuid();
                    treatment.Id = newTreatmentId;
                    treatment.SimulationId = simulationId;

                    _unitOfWork.Context
                        .ReInitializeAllEntityBaseProperties(treatment, _unitOfWork.UserEntity?.Id);

                    if (treatment.CriterionLibraryScenarioSelectableTreatmentJoin != null)
                    {
                        var criterionLibraryId = Guid.NewGuid();
                        treatment.CriterionLibraryScenarioSelectableTreatmentJoin.CriterionLibrary.Id = criterionLibraryId;
                        treatment.CriterionLibraryScenarioSelectableTreatmentJoin.CriterionLibraryId = criterionLibraryId;
                        treatment.CriterionLibraryScenarioSelectableTreatmentJoin.ScenarioSelectableTreatmentId = newTreatmentId;

                        _unitOfWork.Context
                            .ReInitializeAllEntityBaseProperties(
                                treatment.CriterionLibraryScenarioSelectableTreatmentJoin.CriterionLibrary,
                                _unitOfWork.UserEntity?.Id);
                        _unitOfWork.Context
                            .ReInitializeAllEntityBaseProperties(treatment.CriterionLibraryScenarioSelectableTreatmentJoin,
                                _unitOfWork.UserEntity?.Id);
                    }

                    if (treatment.ScenarioTreatmentConsequences.Any())
                    {
                        treatment.ScenarioTreatmentConsequences.ForEach(consequence =>
                        {
                            var newConsequenceId = Guid.NewGuid();
                            consequence.Id = newConsequenceId;
                            consequence.Attribute = null;
                            _unitOfWork.Context
                            .ReInitializeAllEntityBaseProperties(consequence,
                                _unitOfWork.UserEntity?.Id);

                            if (consequence.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin != null)
                            {
                                _unitOfWork.Context
                            .ReInitializeAllEntityBaseProperties(
                                consequence.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin,
                                _unitOfWork.UserEntity?.Id);

                                _unitOfWork.Context
                            .ReInitializeAllEntityBaseProperties(
                                consequence.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin.ScenarioConditionalTreatmentConsequence,
                                _unitOfWork.UserEntity?.Id);
                            }

                            if(consequence.ScenarioConditionalTreatmentConsequenceEquationJoin != null)
                            {
                                var newEquationId = Guid.NewGuid();
                                consequence.ScenarioConditionalTreatmentConsequenceEquationJoin.EquationId = newEquationId;
                                consequence.ScenarioConditionalTreatmentConsequenceEquationJoin.Equation.Id = newEquationId;

                                consequence.ScenarioConditionalTreatmentConsequenceEquationJoin.ScenarioConditionalTreatmentConsequenceId = newConsequenceId;

                                _unitOfWork.Context
                            .ReInitializeAllEntityBaseProperties(
                                consequence.ScenarioConditionalTreatmentConsequenceEquationJoin,
                                _unitOfWork.UserEntity?.Id);

                                _unitOfWork.Context
                            .ReInitializeAllEntityBaseProperties(
                                consequence.ScenarioConditionalTreatmentConsequenceEquationJoin.Equation,
                                _unitOfWork.UserEntity?.Id);
                            }
                        });
                    }
                    if (treatment.ScenarioTreatmentCosts.Any())
                    {
                        treatment.ScenarioTreatmentCosts.ForEach(cost =>
                        {
                            var newCostId = Guid.NewGuid();
                            cost.Id = newCostId;
                            cost.ScenarioSelectableTreatmentId = newTreatmentId;
                            _unitOfWork.Context
                            .ReInitializeAllEntityBaseProperties(cost,
                                _unitOfWork.UserEntity?.Id);

                            if(cost.CriterionLibraryScenarioTreatmentCostJoin != null)
                            {
                                cost.CriterionLibraryScenarioTreatmentCostJoin.ScenarioTreatmentCostId = newCostId;

                                cost.CriterionLibraryScenarioTreatmentCostJoin.ScenarioTreatmentCost.Id = newCostId;
                                cost.CriterionLibraryScenarioTreatmentCostJoin.ScenarioTreatmentCost.ScenarioSelectableTreatmentId = newTreatmentId;

                                _unitOfWork.Context
                            .ReInitializeAllEntityBaseProperties(
                                cost.CriterionLibraryScenarioTreatmentCostJoin,
                                _unitOfWork.UserEntity?.Id);

                                _unitOfWork.Context
                            .ReInitializeAllEntityBaseProperties(
                                cost.CriterionLibraryScenarioTreatmentCostJoin.ScenarioTreatmentCost,
                                _unitOfWork.UserEntity?.Id);
                            }
                            if (cost.ScenarioTreatmentCostEquationJoin != null)
                            {
                                var newEquationId = Guid.NewGuid();
                                cost.ScenarioTreatmentCostEquationJoin.EquationId = newEquationId;
                                cost.ScenarioTreatmentCostEquationJoin.Equation.Id = newEquationId;

                                cost.ScenarioTreatmentCostEquationJoin.ScenarioTreatmentCostId = newCostId;

                                _unitOfWork.Context
                            .ReInitializeAllEntityBaseProperties(
                                cost.ScenarioTreatmentCostEquationJoin,
                                _unitOfWork.UserEntity?.Id);

                                _unitOfWork.Context
                            .ReInitializeAllEntityBaseProperties(
                                cost.ScenarioTreatmentCostEquationJoin.Equation,
                                _unitOfWork.UserEntity?.Id);
                            }
                        });
                    }
                    if (treatment.ScenarioTreatmentSchedulings.Any())
                    {
                        treatment.ScenarioTreatmentSchedulings.ForEach(scheduling =>
                        {
                            var newScheduleId = Guid.NewGuid();
                            scheduling.Id = newScheduleId;
                            scheduling.TreatmentId = newTreatmentId;

                            _unitOfWork.Context
                            .ReInitializeAllEntityBaseProperties(
                                scheduling,
                                _unitOfWork.UserEntity?.Id);
                        });
                    }
                    if (treatment.ScenarioTreatmentSupersessions.Any())
                    {
                        treatment.ScenarioTreatmentSupersessions.ForEach(supersession =>
                        {
                            var newSupersessionId = Guid.NewGuid();
                            supersession.Id = newSupersessionId;
                            supersession.TreatmentId = newTreatmentId;

                            _unitOfWork.Context
                            .ReInitializeAllEntityBaseProperties(
                                supersession,
                                _unitOfWork.UserEntity?.Id);
                        });
                    }
                });
            }

            if (simulationToClone.CommittedProjects.Any())
            {
                simulationToClone.CommittedProjects.ForEach(committedProject =>
                {
                    committedProject.Id = Guid.NewGuid();
                    committedProject.SimulationId = newSimulationId;
                    _unitOfWork.Context
                        .ReInitializeAllEntityBaseProperties(committedProject, _unitOfWork.UserEntity?.Id);

                    if (committedProject.CommittedProjectConsequences.Any())
                    {
                        committedProject.CommittedProjectConsequences.ForEach(committedProjectConsequence =>
                        {
                            committedProjectConsequence.Id = Guid.NewGuid();
                            committedProjectConsequence.CommittedProjectId = committedProject.Id;
                            _unitOfWork.Context
                                .ReInitializeAllEntityBaseProperties(committedProjectConsequence,
                                    _unitOfWork.UserEntity?.Id);
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
                        SimulationId = newSimulationId,
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
