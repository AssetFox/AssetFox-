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
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Domains;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class BudgetRepository : IBudgetRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public BudgetRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ??
                                         throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateBudgetLibrary(string name, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            var budgetLibraryEntity = new BudgetLibraryEntity { Id = Guid.NewGuid(), Name = name };

            _unitOfWork.Context.AddEntity(budgetLibraryEntity);

            _unitOfWork.Context.AddEntity(new BudgetLibrarySimulationEntity
            {
                BudgetLibraryId = budgetLibraryEntity.Id,
                SimulationId = simulationId
            });
        }

        public void CreateBudgets(List<Budget> budgets, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            var simulationEntity = _unitOfWork.Context.Simulation
                .Include(_ => _.BudgetLibrarySimulationJoin)
                .Single(_ => _.Id == simulationId);

            if (simulationEntity.BudgetLibrarySimulationJoin == null)
            {
                throw new RowNotInTableException($"No budget library found for simulation having id {simulationId}");
            }

            var budgetEntities = budgets
                .Select(_ => _.ToEntity(simulationEntity.BudgetLibrarySimulationJoin.BudgetLibraryId))
                .ToList();

            _unitOfWork.Context.AddAll(budgetEntities);

            if (budgets.Any(_ => _.YearlyAmounts.Any()))
            {
                var budgetAmountsPerBudgetId = budgets
                    .Where(_ => _.YearlyAmounts.Any())
                    .ToDictionary(_ => _.Id, _ => _.YearlyAmounts.ToList());

                _unitOfWork.BudgetAmountRepo.CreateBudgetAmounts(budgetAmountsPerBudgetId, simulationId);
            }
        }

        public Task<List<SimpleBudgetDetailDTO>> ScenarioSimpleBudgetDetails(Guid simulationId)
        {
            if (simulationId == Guid.Empty)
            {
                return Task.Factory.StartNew(() => new List<SimpleBudgetDetailDTO>());
            }

            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            if (!_unitOfWork.Context.BudgetLibrarySimulation.Any(_ => _.SimulationId == simulationId))
            {
                return Task.Factory.StartNew(() => new List<SimpleBudgetDetailDTO>());
            }

            return Task.Factory.StartNew(() =>
                _unitOfWork.Context.BudgetLibrary
                    .Include(_ => _.Budgets)
                    .Single(_ =>
                        _.BudgetLibrarySimulationJoins.FirstOrDefault(__ => __.SimulationId == simulationId) != null)
                    .Budgets.Select(_ => new SimpleBudgetDetailDTO { Id = _.Id, Name = _.Name }).OrderBy(_ => _.Name)
                    .ToList());
        }

        public Task<List<BudgetLibraryDTO>> BudgetLibrariesWithBudgets()
        {
            if (!_unitOfWork.Context.BudgetLibrary.Any())
            {
                return Task.Factory.StartNew(() => new List<BudgetLibraryDTO>());
            }

            return Task.Factory.StartNew(() => _unitOfWork.Context.BudgetLibrary
                .Include(_ => _.Budgets)
                .ThenInclude(_ => _.BudgetAmounts)
                .Include(_ => _.Budgets)
                .ThenInclude(_ => _.CriterionLibraryBudgetJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.BudgetLibrarySimulationJoins)
                .Select(_ => _.ToDto())
                .ToList());
        }

        public void UpsertPermitted(UserInfoDTO userInfo, Guid simulationId, BudgetLibraryDTO dto)
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

            UpsertBudgetLibrary(dto, simulationId, userInfo);
            UpsertOrDeleteBudgets(dto.Budgets, dto.Id, userInfo);
        }

        public void UpsertBudgetLibrary(BudgetLibraryDTO dto, Guid simulationId, UserInfoDTO userInfo)
        {
            var userEntity = _unitOfWork.Context.User.SingleOrDefault(_ => _.Username == userInfo.Sub);

            var budgetLibraryEntity = dto.ToEntity();

            _unitOfWork.Context.Upsert(budgetLibraryEntity, dto.Id, userEntity?.Id);

            if (simulationId != Guid.Empty)
            {
                if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
                {
                    throw new RowNotInTableException($"No simulation found having id {simulationId}.");
                }

                _unitOfWork.Context.DeleteEntity<BudgetLibrarySimulationEntity>(_ => _.SimulationId == simulationId);

                _unitOfWork.Context.AddEntity(
                    new BudgetLibrarySimulationEntity { BudgetLibraryId = dto.Id, SimulationId = simulationId },
                    userEntity?.Id);
            }
        }

        public void UpsertOrDeleteBudgets(List<BudgetDTO> budgets, Guid libraryId, UserInfoDTO userInfo)
        {
            if (!_unitOfWork.Context.BudgetLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException($"No budget library found having id {libraryId}.");
            }

            var userEntity = _unitOfWork.Context.User.SingleOrDefault(_ => _.Username == userInfo.Sub);

            var budgetEntities = budgets.Select(_ => _.ToEntity(libraryId)).ToList();

            var entityIds = budgetEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.Budget
                .Where(_ => _.BudgetLibraryId == libraryId && entityIds.Contains(_.Id)).Select(_ => _.Id).ToList();

            var predicatesPerCrudOperation = new Dictionary<string, Expression<Func<BudgetEntity, bool>>>
            {
                {"delete", _ => _.BudgetLibraryId == libraryId && !entityIds.Contains(_.Id)},
                {"update", _ => existingEntityIds.Contains(_.Id)},
                {"add", _ => !existingEntityIds.Contains(_.Id)}
            };

            if (IsRunningFromXUnit)
            {
                _unitOfWork.Context.UpsertOrDelete(budgetEntities, predicatesPerCrudOperation, userEntity?.Id);
            }
            else
            {
                _unitOfWork.Context.BulkUpsertOrDelete(budgetEntities, predicatesPerCrudOperation, userEntity?.Id);
            }

            _unitOfWork.Context.DeleteAll<CriterionLibraryBudgetEntity>(_ =>
                _.Budget.BudgetLibraryId == libraryId);

            var budgetAmountsPerBudgetId = budgets.ToDictionary(_ => _.Id, _ => _.BudgetAmounts);

            _unitOfWork.BudgetAmountRepo.UpsertOrDeleteBudgetAmounts(budgetAmountsPerBudgetId, libraryId);

            if (budgets.Any(_ =>
                _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
            {
                var criterionLibraryJoinsToAdd = budgets
                    .Where(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)).Select(_ =>
                        new CriterionLibraryBudgetEntity { CriterionLibraryId = _.CriterionLibrary.Id, BudgetId = _.Id })
                    .ToList();

                _unitOfWork.Context.AddAll(criterionLibraryJoinsToAdd, userEntity?.Id);
            }
        }

        public void DeleteBudgetLibrary(Guid libraryId)
        {
            if (!_unitOfWork.Context.BudgetLibrary.Any(_ => _.Id == libraryId))
            {
                return;
            }

            _unitOfWork.Context.DeleteEntity<BudgetLibraryEntity>(_ => _.Id == libraryId);
        }
    }
}
