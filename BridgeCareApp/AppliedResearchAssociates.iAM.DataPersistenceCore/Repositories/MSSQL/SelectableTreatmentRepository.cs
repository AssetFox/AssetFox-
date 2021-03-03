using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Extensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SelectableTreatmentRepository : ISelectableTreatmentRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public SelectableTreatmentRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfDataPersistenceWork)
        {
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
        }

        public void CreateTreatmentLibrary(string name, Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having name {simulationId}.");
            }

            var treatmentLibraryEntity = new TreatmentLibraryEntity { Id = Guid.NewGuid(), Name = name };

            _unitOfDataPersistenceWork.Context.TreatmentLibrary.Add(treatmentLibraryEntity);

            _unitOfDataPersistenceWork.Context.TreatmentLibrarySimulation.Add(new TreatmentLibrarySimulationEntity
            {
                TreatmentLibraryId = treatmentLibraryEntity.Id,
                SimulationId = simulationId
            });

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        public void CreateSelectableTreatments(List<SelectableTreatment> selectableTreatments, Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            var simulationEntity = _unitOfDataPersistenceWork.Context.Simulation
                .Include(_ => _.TreatmentLibrarySimulationJoin)
                .Single(_ => _.Id == simulationId);

            if (simulationEntity.TreatmentLibrarySimulationJoin == null)
            {
                throw new RowNotInTableException($"No treatment library found for simulation having id {simulationId}.");
            }

            var treatmentEntities = selectableTreatments
                .Select(_ => _.ToEntity(simulationEntity.TreatmentLibrarySimulationJoin.TreatmentLibraryId))
                .ToList();

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.SelectableTreatment.AddRange(treatmentEntities);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkInsert(treatmentEntities);
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();

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

                _unitOfDataPersistenceWork.TreatmentConsequenceRepo.CreateTreatmentConsequences(consequencesPerTreatmentId, simulationEntity.Name);
            }

            if (selectableTreatments.Any(_ => _.Costs.Any()))
            {
                var costsPerTreatmentId = selectableTreatments
                    .Where(_ => _.Costs.Any())
                    .ToDictionary(_ => _.Id, _ => _.Costs.ToList());

                _unitOfDataPersistenceWork.TreatmentCostRepo.CreateTreatmentCosts(costsPerTreatmentId, simulationEntity.Name);
            }

            if (selectableTreatments.Any(_ => _.FeasibilityCriteria.Any(__ => !__.ExpressionIsBlank)))
            {
                var entityIdsPerExpression = selectableTreatments
                    .Where(_ => _.FeasibilityCriteria.Any(__ => !__.ExpressionIsBlank))
                    .ToDictionary(_ => _.FeasibilityCriteria.ToList()[0].Expression, _ => new List<Guid> { _.Id });

                _unitOfDataPersistenceWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(entityIdsPerExpression,
                    DataPersistenceConstants.CriterionLibraryJoinEntities.SelectableTreatment, simulationEntity.Name);
            }

            if (selectableTreatments.Any(_ => _.Schedulings.Any()))
            {
                var schedulingsPerTreatmentId = selectableTreatments
                    .Where(_ => _.Schedulings.Any())
                    .ToDictionary(_ => _.Id, _ => _.Schedulings.ToList());

                _unitOfDataPersistenceWork.TreatmentSchedulingRepo.CreateTreatmentSchedulings(schedulingsPerTreatmentId);
            }

            if (selectableTreatments.Any(_ => _.Supersessions.Any()))
            {
                var supersessionsPerTreatmentId = selectableTreatments
                    .Where(_ => _.Supersessions.Any())
                    .ToDictionary(_ => _.Id, _ => _.Supersessions.ToList());

                _unitOfDataPersistenceWork.TreatmentSupersessionRepo.CreateTreatmentSupersessions(supersessionsPerTreatmentId, simulationEntity.Name);
            }
        }

        private void JoinTreatmentsWithBudgets(Dictionary<Guid, List<Guid>> budgetIdsPerTreatmentId)
        {
            var treatmentBudgetJoins = new List<SelectableTreatmentBudgetEntity>();

            budgetIdsPerTreatmentId.Keys.ForEach(treatmentId =>
            {
                var budgetIds = budgetIdsPerTreatmentId[treatmentId];
                var budgetEntities = _unitOfDataPersistenceWork.Context.Budget
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

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.TreatmentBudget.AddRange(treatmentBudgetJoins);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkInsert(treatmentBudgetJoins);
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        public void GetSimulationTreatments(Simulation simulation)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Name == simulation.Name))
            {
                throw new RowNotInTableException($"No simulation found having name {simulation.Name}.");
            }

            _unitOfDataPersistenceWork.Context.SelectableTreatment
                .Include(_ => _.TreatmentBudgetJoins)
                .ThenInclude(_ => _.Budget)
                .ThenInclude(_ => _.BudgetAmounts)
                .Include(_ => _.TreatmentConsequences)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.TreatmentConsequences)
                .ThenInclude(_ => _.ConditionalTreatmentConsequenceEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.TreatmentConsequences)
                .ThenInclude(_ => _.CriterionLibraryConditionalTreatmentConsequenceJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.TreatmentCosts)
                .ThenInclude(_ => _.TreatmentCostEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.TreatmentCosts)
                .ThenInclude(_ => _.CriterionLibraryTreatmentCostJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.CriterionLibrarySelectableTreatmentJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.TreatmentSchedulings)
                .Include(_ => _.TreatmentSupersessions)
                .Where(_ => _.TreatmentLibrary.TreatmentLibrarySimulationJoins.SingleOrDefault(__ =>
                    __.Simulation.Name == simulation.Name) != null)
                .ForEach(_ => _.CreateSelectableTreatment(simulation));
        }

        public Task<List<TreatmentLibraryDTO>> TreatmentLibrariesWithTreatments()
        {
            if (!_unitOfDataPersistenceWork.Context.SelectableTreatment.Any())
            {
                return Task.Factory.StartNew(() => new List<TreatmentLibraryDTO>());
            }

            return Task.Factory.StartNew(() => _unitOfDataPersistenceWork.Context.TreatmentLibrary
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
                .Include(_ => _.TreatmentLibrarySimulationJoins)
                .Select(_ => _.ToDto())
                .ToList());
        }

        public void AddOrUpdateTreatmentLibrary(TreatmentLibraryDTO dto, Guid simulationId)
        {
            var entity = dto.ToEntity();

            _unitOfDataPersistenceWork.Context.AddOrUpdate(entity, dto.Id);

            if (simulationId != Guid.Empty)
            {
                if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
                {
                    throw new RowNotInTableException($"No simulation found having id {simulationId}.");
                }

                _unitOfDataPersistenceWork.Context.Delete<TreatmentLibrarySimulationEntity>(_ => _.SimulationId == simulationId);

                _unitOfDataPersistenceWork.Context.TreatmentLibrarySimulation.Add(
                    new TreatmentLibrarySimulationEntity { TreatmentLibraryId = dto.Id, SimulationId = simulationId });
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        public void AddOrUpdateOrDeleteTreatments(List<TreatmentDTO> treatments, Guid libraryId)
        {
            if (!_unitOfDataPersistenceWork.Context.TreatmentLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException($"No treatment library found having id {libraryId}.");
            }

            var entities = treatments.Select(_ => _.ToEntity(libraryId)).ToList();

            var entityIds = entities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfDataPersistenceWork.Context.SelectableTreatment
                .Where(_ => _.TreatmentLibraryId == libraryId && entityIds.Contains(_.Id)).Select(_ => _.Id)
                .ToList();

            var predicatesPerCrudOperation = new Dictionary<string, Expression<Func<SelectableTreatmentEntity, bool>>>
            {
                {"delete", _ => _.TreatmentLibraryId == libraryId && !entityIds.Contains(_.Id)},
                {"update", _ => existingEntityIds.Contains(_.Id)},
                {"add", _ => !existingEntityIds.Contains(_.Id)}
            };

            var equationIds = _unitOfDataPersistenceWork.Context.TreatmentCostEquation
                .Where(_ => _.TreatmentCost.SelectableTreatment.TreatmentLibraryId == libraryId)
                .Select(_ => _.EquationId).ToList();

            equationIds.AddRange(_unitOfDataPersistenceWork.Context.TreatmentConsequenceEquation
                .Where(_ => _.ConditionalTreatmentConsequence.SelectableTreatment.TreatmentLibraryId == libraryId)
                .Select(_ => _.EquationId).ToList());

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.AddOrUpdateOrDelete(entities, predicatesPerCrudOperation);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkAddOrUpdateOrDelete(entities, predicatesPerCrudOperation);
            }

            _unitOfDataPersistenceWork.Context.DeleteAll<EquationEntity>(_ => equationIds.Contains(_.Id));

            _unitOfDataPersistenceWork.Context.DeleteAll<SelectableTreatmentBudgetEntity>(_ =>
                _.SelectableTreatment.TreatmentLibraryId == libraryId);

            _unitOfDataPersistenceWork.Context.DeleteAll<CriterionLibrarySelectableTreatmentEntity>(_ =>
                _.SelectableTreatment.TreatmentLibraryId == libraryId);

            _unitOfDataPersistenceWork.Context.DeleteAll<CriterionLibraryTreatmentCostEntity>(_ =>
                _.TreatmentCost.SelectableTreatment.TreatmentLibraryId == libraryId);

            _unitOfDataPersistenceWork.Context.DeleteAll<CriterionLibraryConditionalTreatmentConsequenceEntity>(_ =>
                _.ConditionalTreatmentConsequence.SelectableTreatment.TreatmentLibraryId == libraryId);

            if (treatments.Any(_ => _.Costs.Any()))
            {
                var costsPerTreatmentId =
                    treatments.Where(_ => _.Costs.Any()).ToList().ToDictionary(_ => _.Id, _ => _.Costs);
                _unitOfDataPersistenceWork.TreatmentCostRepo.AddOrUpdateOrDeleteTreatmentCosts(costsPerTreatmentId, libraryId);
            }

            if (treatments.Any(_ => _.Consequences.Any()))
            {
                var consequencesPerTreatmentId = treatments.Where(_ => _.Consequences.Any()).ToList()
                    .ToDictionary(_ => _.Id, _ => _.Consequences);
                _unitOfDataPersistenceWork.TreatmentConsequenceRepo.AddOrUpdateOrDeleteTreatmentConsequences(consequencesPerTreatmentId, libraryId);
            }

            if (treatments.Any(_ => _.BudgetIds.Any()))
            {
                var treatmentBudgetJoinsToAdd = treatments.Where(_ => _.BudgetIds.Any()).SelectMany(_ =>
                    _.BudgetIds.Select(budgetId =>
                        new SelectableTreatmentBudgetEntity { SelectableTreatmentId = _.Id, BudgetId = budgetId })).ToList();

                if (IsRunningFromXUnit)
                {
                    _unitOfDataPersistenceWork.Context.TreatmentBudget.AddRange(treatmentBudgetJoinsToAdd);
                }
                else
                {
                    _unitOfDataPersistenceWork.Context.BulkInsert(treatmentBudgetJoinsToAdd);
                }
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

                if (IsRunningFromXUnit)
                {
                    _unitOfDataPersistenceWork.Context.CriterionLibrarySelectableTreatment.AddRange(criterionLibraryJoinsToAdd);
                }
                else
                {
                    _unitOfDataPersistenceWork.Context.BulkInsert(criterionLibraryJoinsToAdd);
                }
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        public void DeleteTreatmentLibrary(Guid libraryId)
        {
            if (!_unitOfDataPersistenceWork.Context.TreatmentLibrary.Any(_ => _.Id == libraryId))
            {
                return;
            }

            var libraryToDelete = _unitOfDataPersistenceWork.Context.TreatmentLibrary.Single(_ => _.Id == libraryId);

            var equationIds = _unitOfDataPersistenceWork.Context.TreatmentCostEquation
                .Where(_ => _.TreatmentCost.SelectableTreatment.TreatmentLibraryId == libraryId)
                .Select(_ => _.EquationId).ToList();

            equationIds.AddRange(_unitOfDataPersistenceWork.Context.TreatmentConsequenceEquation
                .Where(_ => _.ConditionalTreatmentConsequence.SelectableTreatment.TreatmentLibraryId == libraryId)
                .Select(_ => _.EquationId).ToList());

            _unitOfDataPersistenceWork.Context.TreatmentLibrary.Remove(libraryToDelete);

            _unitOfDataPersistenceWork.Context.SaveChanges();

            _unitOfDataPersistenceWork.Context.DeleteAll<EquationEntity>(_ => equationIds.Contains(_.Id));

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }
    }
}
