using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Extensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SelectableTreatmentRepository : ISelectableTreatmentRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfWork;

        public SelectableTreatmentRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ??
                                         throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateTreatmentLibrary(string name, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having name {simulationId}.");
            }

            var treatmentLibraryEntity = new TreatmentLibraryEntity { Id = Guid.NewGuid(), Name = name };

            _unitOfWork.Context.AddEntity(treatmentLibraryEntity);
        }

        public void CreateSelectableTreatments(List<SelectableTreatment> selectableTreatments, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            var simulationEntity = _unitOfWork.Context.Simulation
                .Single(_ => _.Id == simulationId);

            var treatmentEntities = selectableTreatments
                .Select(_ => _.ToScenarioEntity(simulationId))
                .ToList();

            _unitOfWork.Context.AddAll(treatmentEntities);

            if (selectableTreatments.Any(_ => _.Budgets.Any()))
            {
                var budgetIdsPerTreatmentId = selectableTreatments
                    .Where(_ => _.Budgets.Any())
                    .ToDictionary(_ => _.Id, _ => _.Budgets.Select(__ => __.Id).ToList());

                JoinTreatmentsWithBudgets(budgetIdsPerTreatmentId);
            }

            if (selectableTreatments.Any(_ => _.Consequences.Any()))
            {
                var consequencesPerTreatmentId = selectableTreatments
                    .Where(_ => _.Consequences.Any())
                    .ToDictionary(_ => _.Id, _ => _.Consequences.ToList());

                _unitOfWork.TreatmentConsequenceRepo.CreateTreatmentConsequences(
                    consequencesPerTreatmentId, simulationEntity.Name);
            }

            if (selectableTreatments.Any(_ => _.Costs.Any()))
            {
                var costsPerTreatmentId = selectableTreatments
                    .Where(_ => _.Costs.Any())
                    .ToDictionary(_ => _.Id, _ => _.Costs.ToList());

                _unitOfWork.TreatmentCostRepo.CreateTreatmentCosts(costsPerTreatmentId,
                    simulationEntity.Name);
            }

            if (selectableTreatments.Any(_ => _.FeasibilityCriteria.Any(__ => !__.ExpressionIsBlank)))
            {
                var entityIdsPerExpression = selectableTreatments
                    .Where(_ => _.FeasibilityCriteria.Any(__ => !__.ExpressionIsBlank))
                    .ToDictionary(_ => _.FeasibilityCriteria.ToList()[0].Expression, _ => new List<Guid> { _.Id });

                _unitOfWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(entityIdsPerExpression,
                    DataPersistenceConstants.CriterionLibraryJoinEntities.SelectableTreatment, simulationEntity.Name);
            }

            if (selectableTreatments.Any(_ => _.Schedulings.Any()))
            {
                var schedulingsPerTreatmentId = selectableTreatments
                    .Where(_ => _.Schedulings.Any())
                    .ToDictionary(_ => _.Id, _ => _.Schedulings.ToList());

                _unitOfWork.TreatmentSchedulingRepo.CreateTreatmentSchedulings(schedulingsPerTreatmentId);
            }

            if (selectableTreatments.Any(_ => _.Supersessions.Any()))
            {
                var supersessionsPerTreatmentId = selectableTreatments
                    .Where(_ => _.Supersessions.Any())
                    .ToDictionary(_ => _.Id, _ => _.Supersessions.ToList());

                _unitOfWork.TreatmentSupersessionRepo.CreateTreatmentSupersessions(
                    supersessionsPerTreatmentId, simulationEntity.Name);
            }

            // Update last modified date
            _unitOfWork.SimulationRepo.UpdateLastModifiedDate(simulationEntity);
        }

        private void JoinTreatmentsWithBudgets(Dictionary<Guid, List<Guid>> budgetIdsPerTreatmentId)
        {
            var treatmentBudgetJoins = new List<SelectableTreatmentBudgetEntity>();

            budgetIdsPerTreatmentId.Keys.ForEach(treatmentId =>
            {
                var budgetIds = budgetIdsPerTreatmentId[treatmentId];
                var budgetEntities = _unitOfWork.Context.Budget
                    .Where(_ => budgetIds.Contains(_.Id))
                    .ToList();

                if (!budgetEntities.Any())
                {
                    throw new RowNotInTableException("No budgets for treatments were found.");
                }

                treatmentBudgetJoins.AddRange(budgetIds.Select(budgetId => new SelectableTreatmentBudgetEntity
                {
                    SelectableTreatmentId = treatmentId,
                    BudgetId = budgetId
                }));
            });

            _unitOfWork.Context.AddAll(treatmentBudgetJoins);
        }

        public void GetSimulationTreatments(Simulation simulation)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulation.Id))
            {
                throw new RowNotInTableException($"No simulation found having id {simulation.Id}.");
            }

            var simulationEntity = _unitOfWork.Context.Simulation
                .Include(_ => _.SelectableTreatment)
                .ThenInclude(_ => _.ScenarioTreatmentBudgetJoins)
                .ThenInclude(_ => _.Budget)
                .ThenInclude(_ => _.BudgetAmounts)
                .Include(_ => _.SelectableTreatment)
                .ThenInclude(_ => _.ScenarioTreatmentConsequences)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.SelectableTreatment)
                .ThenInclude(_ => _.ScenarioTreatmentConsequences)
                .ThenInclude(_ => _.ScenarioConditionalTreatmentConsequenceEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.SelectableTreatment)
                .ThenInclude(_ => _.ScenarioTreatmentConsequences)
                .ThenInclude(_ => _.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.SelectableTreatment)
                .ThenInclude(_ => _.ScenarioTreatmentCosts)
                .ThenInclude(_ => _.ScenarioTreatmentCostEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.SelectableTreatment)
                .ThenInclude(_ => _.ScenarioTreatmentCosts)
                .ThenInclude(_ => _.CriterionLibraryScenarioTreatmentCostJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.SelectableTreatment)
                .ThenInclude(_ => _.CriterionLibraryScenarioSelectableTreatmentJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.SelectableTreatment)
                .ThenInclude(_ => _.ScenarioTreatmentSchedulings)
                .Include(_ => _.SelectableTreatment)
                .ThenInclude(_ => _.ScenarioTreatmentSupersessions)
                .AsNoTracking()
                .Single(_ => _.Id == simulation.Id);

            simulationEntity.SelectableTreatment?.ForEach(_ =>
                _.CreateSelectableTreatment(simulation));
        }

        public List<TreatmentLibraryDTO> GetTreatmentLibrariesWithTreatments()
        {
            if (!_unitOfWork.Context.SelectableTreatment.Any())
            {
                return new List<TreatmentLibraryDTO>();
            }

            return _unitOfWork.Context.TreatmentLibrary.OrderBy(tl => tl.Name)
                .Include(_ => _.Treatments)
                .ThenInclude(_ => _.TreatmentCosts)
                .ThenInclude(_ => _.TreatmentCostEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.Treatments)
                .ThenInclude(_ => _.TreatmentCosts)
                .ThenInclude(_ => _.CriterionLibraryTreatmentCostJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Treatments)
                .ThenInclude(_ => _.TreatmentConsequences)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.Treatments)
                .ThenInclude(_ => _.TreatmentConsequences)
                .ThenInclude(_ => _.ConditionalTreatmentConsequenceEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.Treatments)
                .ThenInclude(_ => _.TreatmentConsequences)
                .ThenInclude(_ => _.CriterionLibraryConditionalTreatmentConsequenceJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Treatments)
                .ThenInclude(_ => _.TreatmentBudgetJoins)
                .ThenInclude(_ => _.Budget)
                .Include(_ => _.Treatments)
                .ThenInclude(_ => _.CriterionLibrarySelectableTreatmentJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Select(_ => _.ToDto())
                .ToList();
        }

        public void UpsertTreatmentLibrary(TreatmentLibraryDTO dto)
        {
            var treatmentLibraryEntity = dto.ToEntity();

            _unitOfWork.Context.Upsert(treatmentLibraryEntity, dto.Id, _unitOfWork.UserEntity?.Id);
        }

        public void UpsertOrDeleteTreatments(List<TreatmentDTO> treatments, Guid libraryId)
        {
            if (!_unitOfWork.Context.TreatmentLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException($"No treatment library found having id {libraryId}.");
            }

            var selectableTreatmentEntities = treatments.Select(_ => _.ToLibraryEntity(libraryId)).ToList();

            var entityIds = selectableTreatmentEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.SelectableTreatment
                .Where(_ => _.TreatmentLibraryId == libraryId && entityIds.Contains(_.Id)).Select(_ => _.Id)
                .ToList();

            _unitOfWork.Context.DeleteAll<SelectableTreatmentEntity>(_ =>
                _.TreatmentLibraryId == libraryId && !entityIds.Contains(_.Id));

            _unitOfWork.Context.UpdateAll(selectableTreatmentEntities.Where(_ => existingEntityIds.Contains(_.Id))
                .ToList());

            _unitOfWork.Context.AddAll(selectableTreatmentEntities.Where(_ => !existingEntityIds.Contains(_.Id))
                .ToList());

            _unitOfWork.Context.DeleteAll<EquationEntity>(_ =>
                _.TreatmentCostEquationJoin.TreatmentCost.SelectableTreatment.TreatmentLibraryId == libraryId ||
                _.ConditionalTreatmentConsequenceEquationJoin.ConditionalTreatmentConsequence.SelectableTreatment
                    .TreatmentLibraryId == libraryId);

            _unitOfWork.Context.DeleteAll<SelectableTreatmentBudgetEntity>(_ =>
                _.SelectableTreatment.TreatmentLibraryId == libraryId);

            _unitOfWork.Context.DeleteAll<CriterionLibrarySelectableTreatmentEntity>(_ =>
                _.SelectableTreatment.TreatmentLibraryId == libraryId);

            _unitOfWork.Context.DeleteAll<CriterionLibraryTreatmentCostEntity>(_ =>
                _.TreatmentCost.SelectableTreatment.TreatmentLibraryId == libraryId);

            _unitOfWork.Context.DeleteAll<CriterionLibraryConditionalTreatmentConsequenceEntity>(_ =>
                _.ConditionalTreatmentConsequence.SelectableTreatment.TreatmentLibraryId == libraryId);

            if (treatments.Any(_ => _.Costs.Any()))
            {
                var costsPerTreatmentId =
                    treatments.Where(_ => _.Costs.Any()).ToList().ToDictionary(_ => _.Id, _ => _.Costs);
                _unitOfWork.TreatmentCostRepo.UpsertOrDeleteTreatmentCosts(costsPerTreatmentId, libraryId);
            }

            if (treatments.Any(_ => _.Consequences.Any()))
            {
                var consequencesPerTreatmentId = treatments.Where(_ => _.Consequences.Any()).ToList()
                    .ToDictionary(_ => _.Id, _ => _.Consequences);
                _unitOfWork.TreatmentConsequenceRepo.UpsertOrDeleteTreatmentConsequences(consequencesPerTreatmentId, libraryId);
            }

            if (treatments.Any(_ => _.BudgetIds.Any()))
            {
                var treatmentBudgetJoinsToAdd = treatments.Where(_ => _.BudgetIds.Any()).SelectMany(_ =>
                    _.BudgetIds.Select(budgetId =>
                        new SelectableTreatmentBudgetEntity { SelectableTreatmentId = _.Id, BudgetId = budgetId })).ToList();

                _unitOfWork.Context.AddAll(treatmentBudgetJoinsToAdd, _unitOfWork.UserEntity?.Id);
            }

            if (treatments.Any(_ =>
                _.CriterionLibrary?.Id != null && _.CriterionLibrary.Id != Guid.Empty &&
                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
            {
                var criterionLibraryJoinsToAdd = treatments
                    .Where(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)).Select(_ =>
                        new CriterionLibrarySelectableTreatmentEntity
                        {
                            CriterionLibraryId = _.CriterionLibrary.Id,
                            SelectableTreatmentId = _.Id
                        }).ToList();

                _unitOfWork.Context.AddAll(criterionLibraryJoinsToAdd, _unitOfWork.UserEntity?.Id);
            }
        }

        public void DeleteTreatmentLibrary(Guid libraryId)
        {
            if (!_unitOfWork.Context.TreatmentLibrary.Any(_ => _.Id == libraryId))
            {
                return;
            }

            _unitOfWork.Context.DeleteAll<EquationEntity>(_ =>
                _.TreatmentCostEquationJoin.TreatmentCost.SelectableTreatment.TreatmentLibraryId == libraryId ||
                _.ConditionalTreatmentConsequenceEquationJoin.ConditionalTreatmentConsequence.SelectableTreatment
                    .TreatmentLibraryId == libraryId);

            _unitOfWork.Context.DeleteEntity<TreatmentLibraryEntity>(_ => _.Id == libraryId);
        }

        public List<TreatmentDTO> GetScenarioSelectableTreatments(Guid simulationId)
        {
            if(!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found for the given scenario");
            }
            var res = _unitOfWork.Context.ScenarioSelectableTreatment.Where(_ => _.SimulationId == simulationId)
                .Include(_ => _.ScenarioTreatmentCosts)
                .ThenInclude(_ => _.ScenarioTreatmentCostEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.ScenarioTreatmentCosts)
                .ThenInclude(_ => _.CriterionLibraryScenarioTreatmentCostJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.ScenarioTreatmentConsequences)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.ScenarioTreatmentConsequences)
                .ThenInclude(_ => _.ScenarioConditionalTreatmentConsequenceEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.ScenarioTreatmentConsequences)
                .ThenInclude(_ => _.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.ScenarioTreatmentBudgetJoins)
                .ThenInclude(_ => _.Budget)
                .Include(_ => _.CriterionLibraryScenarioSelectableTreatmentJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Select(_ => _.ToDto()).AsNoTracking()
                .ToList();

            return res;
        }

        public void UpsertOrDeleteScenarioSelectableTreatment(List<TreatmentDTO> scenarioSelectableTreatment, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found for the given scenario.");
            }
            var scenarioSelectableTreatmentEntities = scenarioSelectableTreatment.Select(_ => _.ToScenarioEntity(simulationId)).ToList();
            var entityIds = scenarioSelectableTreatmentEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.ScenarioSelectableTreatment
                .Where(_ => _.SimulationId == simulationId && entityIds.Contains(_.Id)).Select(_ => _.Id)
                .ToList();

            _unitOfWork.Context.DeleteAll<ScenarioSelectableTreatmentEntity>(_ =>
                _.SimulationId == simulationId && !entityIds.Contains(_.Id));

            _unitOfWork.Context.UpdateAll(scenarioSelectableTreatmentEntities.Where(_ => existingEntityIds.Contains(_.Id))
                .ToList());
            _unitOfWork.Context.AddAll(scenarioSelectableTreatmentEntities.Where(_ => !existingEntityIds.Contains(_.Id))
                .ToList());

            _unitOfWork.Context.DeleteAll<EquationEntity>(_ =>
                _.ScenarioTreatmentCostEquationJoin.ScenarioTreatmentCost.ScenarioSelectableTreatment.SimulationId == simulationId ||
                _.ScenarioConditionalTreatmentConsequenceEquationJoin.ScenarioConditionalTreatmentConsequence.ScenarioSelectableTreatment
                    .SimulationId == simulationId);

            _unitOfWork.Context.DeleteAll<ScenarioSelectableTreatmentBudgetEntity>(_ =>
                _.ScenarioSelectableTreatment.SimulationId == simulationId);

            _unitOfWork.Context.DeleteAll<CriterionLibraryScenarioSelectableTreatmentEntity>(_ =>
                _.ScenarioSelectableTreatment.SimulationId == simulationId);

            _unitOfWork.Context.DeleteAll<CriterionLibraryScenarioTreatmentCostEntity>(_ =>
                _.ScenarioTreatmentCost.ScenarioSelectableTreatment.SimulationId == simulationId);

            _unitOfWork.Context.DeleteAll<CriterionLibraryScenarioConditionalTreatmentConsequenceEntity>(_ =>
                _.ScenarioConditionalTreatmentConsequence.ScenarioSelectableTreatment.SimulationId == simulationId);

            if (scenarioSelectableTreatment.Any(_ => _.Costs.Any()))
            {
                var costsPerTreatmentId =
                    scenarioSelectableTreatment.Where(_ => _.Costs.Any()).ToList().ToDictionary(_ => _.Id, _ => _.Costs);
                _unitOfWork.TreatmentCostRepo.UpsertOrDeleteScenarioTreatmentCosts(costsPerTreatmentId, simulationId);
            }

            if (scenarioSelectableTreatment.Any(_ => _.Consequences.Any()))
            {
                var consequencesPerTreatmentId = scenarioSelectableTreatment.Where(_ => _.Consequences.Any()).ToList()
                    .ToDictionary(_ => _.Id, _ => _.Consequences);
                _unitOfWork.TreatmentConsequenceRepo.UpsertOrDeleteScenarioTreatmentConsequences(consequencesPerTreatmentId, simulationId);
            }

            if (scenarioSelectableTreatment.Any(_ => _.BudgetIds.Any()))
            {
                var treatmentBudgetJoinsToAdd = scenarioSelectableTreatment.Where(_ => _.BudgetIds.Any()).SelectMany(_ =>
                    _.BudgetIds.Select(budgetId =>
                        new ScenarioSelectableTreatmentBudgetEntity { ScenarioSelectableTreatmentId = _.Id, BudgetId = budgetId })).ToList();

                _unitOfWork.Context.AddAll(treatmentBudgetJoinsToAdd, _unitOfWork.UserEntity?.Id);
            }

            if (scenarioSelectableTreatment.Any(_ =>
                _.CriterionLibrary?.Id != null && _.CriterionLibrary.Id != Guid.Empty &&
                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
            {
                var criterionLibraryEntities = new List<CriterionLibraryEntity>();
                var criterionLibraryJoinEntities = new List<CriterionLibraryScenarioSelectableTreatmentEntity>();

                scenarioSelectableTreatment
                    .Where(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression))
                    .ForEach(treatment =>
                    {
                        var criterionLibraryEntity = new CriterionLibraryEntity
                        {
                            Id = Guid.NewGuid(),
                            MergedCriteriaExpression = treatment.CriterionLibrary.MergedCriteriaExpression,
                            Name = $"{treatment.Name} Criterion",
                            IsSingleUse = true
                        };
                        criterionLibraryEntities.Add(criterionLibraryEntity);
                        criterionLibraryJoinEntities.Add(new CriterionLibraryScenarioSelectableTreatmentEntity
                        {
                            CriterionLibraryId = criterionLibraryEntity.Id,
                            ScenarioSelectableTreatmentId = treatment.Id
                        });
                    });

                _unitOfWork.Context.AddAll(criterionLibraryEntities, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criterionLibraryJoinEntities, _unitOfWork.UserEntity?.Id);
            }
            // Update last modified date
            var simulationEntity = _unitOfWork.Context.Simulation.Single(_ => _.Id == simulationId);
            _unitOfWork.Context.Upsert(simulationEntity, simulationId, _unitOfWork.UserEntity?.Id);
        }
    }
}
