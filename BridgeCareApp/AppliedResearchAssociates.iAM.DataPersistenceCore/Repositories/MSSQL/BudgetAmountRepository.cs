using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class BudgetAmountRepository : IBudgetAmountRepository
    {
        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfWork;

        public BudgetAmountRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateBudgetAmounts(Dictionary<Guid, List<BudgetAmount>> budgetAmountsPerBudgetEntityId, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found for the given scenario.");
            }

            var simulationEntity = _unitOfWork.Context.Simulation
                .Include(_ => _.InvestmentPlan)
                .Single(_ => _.Id == simulationId);

            if (simulationEntity.InvestmentPlan == null)
            {
                throw new RowNotInTableException($"No investment plan found for given scenario.");
            }

            var budgetAmountEntities = new List<ScenarioBudgetAmountEntity>();

            budgetAmountsPerBudgetEntityId.Keys.ForEach(budgetEntityId =>
            {
                var year = simulationEntity.InvestmentPlan.FirstYearOfAnalysisPeriod;
                budgetAmountsPerBudgetEntityId[budgetEntityId].ForEach(_ =>
                {
                    budgetAmountEntities.Add(_.ToScenarioEntity(budgetEntityId, year));
                    year++;
                });
            });

            _unitOfWork.Context.AddAll(budgetAmountEntities);
        }

        public void UpsertOrDeleteBudgetAmounts(Dictionary<Guid, List<BudgetAmountDTO>> budgetAmountsPerBudgetId, Guid libraryId)
        {
            var budgetAmountEntities = budgetAmountsPerBudgetId.SelectMany(_ => _.Value.Select(__ => __.ToLibraryEntity(_.Key))).ToList();

            var entityIds = budgetAmountEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.BudgetAmount
                .Where(_ => _.Budget.BudgetLibraryId == libraryId && entityIds.Contains(_.Id)).Select(_ => _.Id)
                .ToList();

            _unitOfWork.Context.DeleteAll<BudgetAmountEntity>(_ =>
                _.Budget.BudgetLibraryId == libraryId && !entityIds.Contains(_.Id));

            _unitOfWork.Context.UpdateAll(
                budgetAmountEntities.Where(_ => existingEntityIds.Contains(_.Id)).ToList(), _unitOfWork.UserEntity?.Id);

            _unitOfWork.Context.AddAll(
                budgetAmountEntities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList(), _unitOfWork.UserEntity?.Id);
        }

        public void UpsertOrDeleteScenarioBudgetAmounts(Dictionary<Guid, List<BudgetAmountDTO>> budgetAmountsPerBudgetId, Guid simulationId)
        {
            var budgetAmountEntities = budgetAmountsPerBudgetId
                .SelectMany(_ => _.Value.Select(__ => __.ToScenarioEntity(_.Key))).ToList();

            var entityIds = budgetAmountEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.ScenarioBudgetAmount
                .Where(_ => _.ScenarioBudget.SimulationId == simulationId && entityIds.Contains(_.Id)).Select(_ => _.Id)
                .ToList();

            _unitOfWork.Context.DeleteAll<ScenarioBudgetAmountEntity>(_ =>
                _.ScenarioBudget.SimulationId == simulationId && !entityIds.Contains(_.Id));

            _unitOfWork.Context.UpdateAll(
                budgetAmountEntities.Where(_ => existingEntityIds.Contains(_.Id)).ToList(), _unitOfWork.UserEntity?.Id);

            _unitOfWork.Context.AddAll(
                budgetAmountEntities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList(), _unitOfWork.UserEntity?.Id);
        }

        public List<BudgetAmountEntity> GetBudgetAmounts(Guid libraryId)
        {
            if (!_unitOfWork.Context.BudgetLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException($"Could not find budget library.");
            }

            return _unitOfWork.Context.BudgetAmount.Where(_ => _.Budget.BudgetLibrary.Id == libraryId)
                .Select(budgetAmount => new BudgetAmountEntity
                {
                    Year = budgetAmount.Year,
                    Value = budgetAmount.Value,
                    Budget = new BudgetEntity {Name = budgetAmount.Budget.Name}
                }).AsNoTracking().ToList();
        }

        public List<ScenarioBudgetAmountEntity> GetScenarioBudgetAmounts(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"Could not find simulation.");
            }

            return _unitOfWork.Context.ScenarioBudgetAmount.Where(_ => _.ScenarioBudget.SimulationId == simulationId)
                .Select(budgetAmount => new ScenarioBudgetAmountEntity
                {
                    Year = budgetAmount.Year,
                    Value = budgetAmount.Value,
                    ScenarioBudget = new ScenarioBudgetEntity {Name = budgetAmount.ScenarioBudget.Name}
                }).AsNoTracking().ToList();
        }
    }
}
