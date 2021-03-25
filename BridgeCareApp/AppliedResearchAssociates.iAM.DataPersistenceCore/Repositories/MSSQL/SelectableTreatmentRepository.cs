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

            _unitOfWork.Context.AddEntity(new TreatmentLibrarySimulationEntity
            {
                TreatmentLibraryId = treatmentLibraryEntity.Id,
                SimulationId = simulationId
            });
        }

        public void CreateSelectableTreatments(List<SelectableTreatment> selectableTreatments, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            var simulationEntity = _unitOfWork.Context.Simulation
                .Include(_ => _.TreatmentLibrarySimulationJoin)
                .Single(_ => _.Id == simulationId);

            if (simulationEntity.TreatmentLibrarySimulationJoin == null)
            {
                throw new RowNotInTableException($"No treatment library found for simulation having id {simulationId}.");
            }

            var treatmentEntities = selectableTreatments
                .Select(_ => _.ToEntity(simulationEntity.TreatmentLibrarySimulationJoin.TreatmentLibraryId))
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
                .Include(_ => _.TreatmentLibrarySimulationJoin)
                .ThenInclude(_ => _.TreatmentLibrary)
                .ThenInclude(_ => _.Treatments)
                .ThenInclude(_ => _.TreatmentBudgetJoins)
                .ThenInclude(_ => _.Budget)
                .ThenInclude(_ => _.BudgetAmounts)
                .Include(_ => _.TreatmentLibrarySimulationJoin)
                .ThenInclude(_ => _.TreatmentLibrary)
                .ThenInclude(_ => _.Treatments)
                .ThenInclude(_ => _.TreatmentConsequences)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.TreatmentLibrarySimulationJoin)
                .ThenInclude(_ => _.TreatmentLibrary)
                .ThenInclude(_ => _.Treatments)
                .ThenInclude(_ => _.TreatmentConsequences)
                .ThenInclude(_ => _.ConditionalTreatmentConsequenceEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.TreatmentLibrarySimulationJoin)
                .ThenInclude(_ => _.TreatmentLibrary)
                .ThenInclude(_ => _.Treatments)
                .ThenInclude(_ => _.TreatmentConsequences)
                .ThenInclude(_ => _.CriterionLibraryConditionalTreatmentConsequenceJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.TreatmentLibrarySimulationJoin)
                .ThenInclude(_ => _.TreatmentLibrary)
                .ThenInclude(_ => _.Treatments)
                .ThenInclude(_ => _.TreatmentCosts)
                .ThenInclude(_ => _.TreatmentCostEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.TreatmentLibrarySimulationJoin)
                .ThenInclude(_ => _.TreatmentLibrary)
                .ThenInclude(_ => _.Treatments)
                .ThenInclude(_ => _.TreatmentCosts)
                .ThenInclude(_ => _.CriterionLibraryTreatmentCostJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.TreatmentLibrarySimulationJoin)
                .ThenInclude(_ => _.TreatmentLibrary)
                .ThenInclude(_ => _.Treatments)
                .ThenInclude(_ => _.CriterionLibrarySelectableTreatmentJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.TreatmentLibrarySimulationJoin)
                .ThenInclude(_ => _.TreatmentLibrary)
                .ThenInclude(_ => _.Treatments)
                .ThenInclude(_ => _.TreatmentSchedulings)
                .Include(_ => _.TreatmentLibrarySimulationJoin)
                .ThenInclude(_ => _.TreatmentLibrary)
                .ThenInclude(_ => _.Treatments)
                .ThenInclude(_ => _.TreatmentSupersessions)
                .Single(_ => _.Id == simulation.Id);

            simulationEntity.TreatmentLibrarySimulationJoin?.TreatmentLibrary?.Treatments?.ForEach(_ =>
                _.CreateSelectableTreatment(simulation));
        }

        public Task<List<TreatmentLibraryDTO>> TreatmentLibrariesWithTreatments()
        {
            if (!_unitOfWork.Context.SelectableTreatment.Any())
            {
                return Task.Factory.StartNew(() => new List<TreatmentLibraryDTO>());
            }

            return Task.Factory.StartNew(() => _unitOfWork.Context.TreatmentLibrary
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

        public void UpsertPermitted(UserInfoDTO userInfo, Guid simulationId, TreatmentLibraryDTO dto)
        {
            if (simulationId != Guid.Empty)
            {
                if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
                {
                    throw new RowNotInTableException($"No simulation found having id {dto.Id}");
                }

                if (!_unitOfWork.Context.Simulation.Any(_ =>
                    _.Id == dto.Id && _.SimulationUserJoins.Any(__ => __.User.Username == userInfo.Sub && __.CanModify)))
                {
                    throw new UnauthorizedAccessException("You are not authorized to modify this simulation.");
                }
            }

            UpsertTreatmentLibrary(dto, simulationId, userInfo);
            UpsertOrDeleteTreatments(dto.Treatments, dto.Id, userInfo);
        }

        public void UpsertTreatmentLibrary(TreatmentLibraryDTO dto, Guid simulationId, UserInfoDTO userInfo)
        {
            var userEntity = _unitOfWork.Context.User.SingleOrDefault(_ => _.Username == userInfo.Sub);

            var treatmentLibraryEntity = dto.ToEntity();

            _unitOfWork.Context.Upsert(treatmentLibraryEntity, dto.Id, userEntity?.Id);

            if (simulationId != Guid.Empty)
            {
                if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
                {
                    throw new RowNotInTableException($"No simulation found having id {simulationId}.");
                }

                _unitOfWork.Context.DeleteEntity<TreatmentLibrarySimulationEntity>(_ => _.SimulationId == simulationId);

                _unitOfWork.Context.AddEntity(
                    new TreatmentLibrarySimulationEntity { TreatmentLibraryId = dto.Id, SimulationId = simulationId },
                    userEntity?.Id);
            }
        }

        public void UpsertOrDeleteTreatments(List<TreatmentDTO> treatments, Guid libraryId, UserInfoDTO userInfo)
        {
            if (!_unitOfWork.Context.TreatmentLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException($"No treatment library found having id {libraryId}.");
            }

            var userEntity = _unitOfWork.Context.User.SingleOrDefault(_ => _.Username == userInfo.Sub);

            var selectableTreatmentEntities = treatments.Select(_ => _.ToEntity(libraryId)).ToList();

            var entityIds = selectableTreatmentEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.SelectableTreatment
                .Where(_ => _.TreatmentLibraryId == libraryId && entityIds.Contains(_.Id)).Select(_ => _.Id)
                .ToList();

            var predicatesPerCrudOperation = new Dictionary<string, Expression<Func<SelectableTreatmentEntity, bool>>>
            {
                {"delete", _ => _.TreatmentLibraryId == libraryId && !entityIds.Contains(_.Id)},
                {"update", _ => existingEntityIds.Contains(_.Id)},
                {"add", _ => !existingEntityIds.Contains(_.Id)}
            };

            _unitOfWork.Context.DeleteAll<EquationEntity>(_ =>
                _.TreatmentCostEquationJoin.TreatmentCost.SelectableTreatment.TreatmentLibraryId == libraryId ||
                _.ConditionalTreatmentConsequenceEquationJoin.ConditionalTreatmentConsequence.SelectableTreatment
                    .TreatmentLibraryId == libraryId);

            if (IsRunningFromXUnit)
            {
                _unitOfWork.Context.UpsertOrDelete(selectableTreatmentEntities, predicatesPerCrudOperation, userEntity?.Id);
            }
            else
            {
                _unitOfWork.Context.BulkUpsertOrDelete(selectableTreatmentEntities, predicatesPerCrudOperation, userEntity?.Id);
            }

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

                _unitOfWork.Context.AddAll(treatmentBudgetJoinsToAdd, userEntity?.Id);
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

                _unitOfWork.Context.AddAll(criterionLibraryJoinsToAdd, userEntity?.Id);
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
    }
}
