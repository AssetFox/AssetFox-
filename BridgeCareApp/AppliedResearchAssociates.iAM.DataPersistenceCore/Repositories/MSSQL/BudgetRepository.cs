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
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class BudgetRepository : IBudgetRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public BudgetRepository(UnitOfDataPersistenceWork unitOfDataPersistenceWork) =>
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ??
                                         throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));

        public void CreateBudgetLibrary(string name, Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            var budgetLibraryEntity = new BudgetLibraryEntity {Id = Guid.NewGuid(), Name = name};

            _unitOfDataPersistenceWork.Context.BudgetLibrary.Add(budgetLibraryEntity);

            _unitOfDataPersistenceWork.Context.BudgetLibrarySimulation.Add(new BudgetLibrarySimulationEntity
            {
                BudgetLibraryId = budgetLibraryEntity.Id, SimulationId = simulationId
            });

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        public void CreateBudgets(List<Budget> budgets, Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            var simulationEntity = _unitOfDataPersistenceWork.Context.Simulation
                .Include(_ => _.BudgetLibrarySimulationJoin)
                .Single(_ => _.Id == simulationId);

            if (simulationEntity.BudgetLibrarySimulationJoin == null)
            {
                throw new RowNotInTableException($"No budget library found for simulation having id {simulationId}");
            }

            var budgetEntities = budgets
                .Select(_ => _.ToEntity(simulationEntity.BudgetLibrarySimulationJoin.BudgetLibraryId))
                .ToList();

            _unitOfDataPersistenceWork.Context.Budget.AddRange(budgetEntities);
            _unitOfDataPersistenceWork.Context.SaveChanges();

            var budgetAmountsPerBudgetId = budgets
                .Where(_ => _.YearlyAmounts.Any())
                .ToDictionary(_ => _.Id, _ => _.YearlyAmounts.ToList());

            if (budgetAmountsPerBudgetId.Values.Any())
            {
                _unitOfDataPersistenceWork.BudgetAmountRepo.CreateBudgetAmounts(budgetAmountsPerBudgetId, simulationId);
            }
        }

        public Task<List<SimpleBudgetDetailDTO>> ScenarioSimpleBudgetDetails(Guid simulationId)
        {
            if (simulationId == Guid.Empty)
            {
                return Task.Factory.StartNew(() => new List<SimpleBudgetDetailDTO>());
            }

            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            if (!_unitOfDataPersistenceWork.Context.BudgetLibrarySimulation.Any(_ => _.SimulationId == simulationId))
            {
                return Task.Factory.StartNew(() => new List<SimpleBudgetDetailDTO>());
            }

            return Task.Factory.StartNew(() =>
                _unitOfDataPersistenceWork.Context.BudgetLibrary
                    .Include(_ => _.Budgets)
                    .Single(_ =>
                        _.BudgetLibrarySimulationJoins.FirstOrDefault(__ => __.SimulationId == simulationId) != null)
                    .Budgets.Select(_ => new SimpleBudgetDetailDTO {Id = _.Id, Name = _.Name}).OrderBy(_ => _.Name)
                    .ToList());
        }

        public Task<List<BudgetLibraryDTO>> BudgetLibrariesWithBudgets()
        {
            if (!_unitOfDataPersistenceWork.Context.BudgetLibrary.Any())
            {
                return Task.Factory.StartNew(() => new List<BudgetLibraryDTO>());
            }

            return Task.Factory.StartNew(() => _unitOfDataPersistenceWork.Context.BudgetLibrary
                .Include(_ => _.Budgets)
                .ThenInclude(_ => _.BudgetAmounts)
                .Include(_ => _.Budgets)
                .ThenInclude(_ => _.CriterionLibraryBudgetJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.BudgetLibrarySimulationJoins)
                .Select(_ => _.ToDto())
                .ToList());
        }

        public void AddOrUpdateBudgetLibrary(BudgetLibraryDTO dto, Guid simulationId)
        {
            var entity = dto.ToEntity();

            _unitOfDataPersistenceWork.Context.AddOrUpdate(entity, dto.Id);

            if (simulationId != Guid.Empty)
            {
                if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
                {
                    throw new RowNotInTableException($"No simulation found having id {simulationId}.");
                }

                _unitOfDataPersistenceWork.Context.Delete<BudgetLibrarySimulationEntity>(_ => _.SimulationId == simulationId);

                _unitOfDataPersistenceWork.Context.BudgetLibrarySimulation.Add(
                    new BudgetLibrarySimulationEntity {BudgetLibraryId = dto.Id, SimulationId = simulationId});
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        public void AddOrUpdateOrDeleteBudgets(List<BudgetDTO> budgets, Guid libraryId)
        {
            if (!_unitOfDataPersistenceWork.Context.BudgetLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException($"No budget library found having id {libraryId}.");
            }

            var entities = budgets.Select(_ => _.ToEntity(libraryId)).ToList();

            var entityIds = entities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfDataPersistenceWork.Context.Budget
                .Where(_ => _.BudgetLibraryId == libraryId && entityIds.Contains(_.Id)).Select(_ => _.Id).ToList();

            var predicatesPerCrudOperation = new Dictionary<string, Expression<Func<BudgetEntity, bool>>>
            {
                {"delete", _ => _.BudgetLibraryId == libraryId && !entityIds.Contains(_.Id)},
                {"update", _ => existingEntityIds.Contains(_.Id)},
                {"add", _ => !existingEntityIds.Contains(_.Id)}
            };

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.AddOrUpdateOrDelete(entities, predicatesPerCrudOperation);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkAddOrUpdateOrDelete(entities, predicatesPerCrudOperation);
            }

            _unitOfDataPersistenceWork.Context.DeleteAll<CriterionLibraryBudgetEntity>(_ =>
                _.Budget.BudgetLibraryId == libraryId);

            var budgetAmountsPerBudgetId = budgets.ToDictionary(_ => _.Id, _ => _.BudgetAmounts);

            _unitOfDataPersistenceWork.BudgetAmountRepo.AddOrUpdateOrDeleteBudgetAmounts(budgetAmountsPerBudgetId, libraryId);

            if (budgets.Any(_ =>
                _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
            {
                var criterionLibraryJoinsToAdd = budgets
                    .Where(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)).Select(_ =>
                        new CriterionLibraryBudgetEntity {CriterionLibraryId = _.CriterionLibrary.Id, BudgetId = _.Id})
                    .ToList();

                if (IsRunningFromXUnit)
                {
                    _unitOfDataPersistenceWork.Context.CriterionLibraryBudget.AddRange(criterionLibraryJoinsToAdd);
                }
                else
                {
                    _unitOfDataPersistenceWork.Context.BulkInsert(criterionLibraryJoinsToAdd);
                }
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        public void DeleteBudgetLibrary(Guid libraryId)
        {
            if (!_unitOfDataPersistenceWork.Context.BudgetLibrary.Any(_ => _.Id == libraryId))
            {
                return;
            }

            var libraryToDelete = _unitOfDataPersistenceWork.Context.BudgetLibrary.Single(_ => _.Id == libraryId);

            _unitOfDataPersistenceWork.Context.BudgetLibrary.Remove(libraryToDelete);

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }
    }
}
