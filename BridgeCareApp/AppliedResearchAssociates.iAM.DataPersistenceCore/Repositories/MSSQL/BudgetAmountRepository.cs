using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class BudgetAmountRepository : IBudgetAmountRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public BudgetAmountRepository(UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateScenarioBudgetAmounts(Dictionary<Guid, List<BudgetAmount>> budgetAmountsPerBudgetEntityId, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            var simulationEntity = _unitOfWork.Context.Simulation.AsNoTracking()
                .Where(_ => _.Id == simulationId)
                .Select(simulation => new SimulationEntity
                {
                    InvestmentPlan = simulation.InvestmentPlan != null
                        ? new InvestmentPlanEntity
                            {
                                FirstYearOfAnalysisPeriod = simulation.InvestmentPlan.FirstYearOfAnalysisPeriod
                            }
                        : null
                }).Single();

            if (simulationEntity.InvestmentPlan == null)
            {
                throw new RowNotInTableException("No investment plan found for given scenario.");
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
            var budgetAmountEntities = budgetAmountsPerBudgetId
                .SelectMany(_ => _.Value.Select(__ => __.ToLibraryEntity(_.Key))).ToList();

            var entityIds = budgetAmountEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.BudgetAmount.AsNoTracking()
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
                .SelectMany(_ => _.Value.Select(amount => amount.ToScenarioEntity(_.Key))).ToList();

            var entityIds = budgetAmountEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.ScenarioBudgetAmount.AsNoTracking()
                .Where(_ => _.ScenarioBudget.SimulationId == simulationId && entityIds.Contains(_.Id)).Select(_ => _.Id)
                .ToList();

            _unitOfWork.Context.DeleteAll<ScenarioBudgetAmountEntity>(_ =>
                _.ScenarioBudget.SimulationId == simulationId && !entityIds.Contains(_.Id));

            _unitOfWork.Context.UpdateAll(
                budgetAmountEntities.Where(_ => existingEntityIds.Contains(_.Id)).ToList(), _unitOfWork.UserEntity?.Id);

            _unitOfWork.Context.AddAll(
                budgetAmountEntities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList(), _unitOfWork.UserEntity?.Id);
        }

        public List<BudgetAmountDTO> GetLibraryBudgetAmounts(Guid libraryId)
        {
            if (!_unitOfWork.Context.BudgetLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException("The specified budget library was not found.");
            }

            var dtos = _unitOfWork.Context.BudgetAmount
                .Include(_ => _.Budget)
                .OrderBy(_ => _.Budget.BudgetOrder)
                .Where(_ => _.Budget.BudgetLibrary.Id == libraryId)
                .Select(budgetAmount => new BudgetAmountDTO
                {
                    Year = budgetAmount.Year,
                    Value = budgetAmount.Value,
                    BudgetName = budgetAmount.Budget.Name,
                }).ToList();
            return dtos;
        }

        public List<BudgetAmountDTO> GetScenarioBudgetAmounts(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("The specified simulation was not found for the given scenario.");
            }

            return _unitOfWork.Context.ScenarioBudgetAmount.AsNoTracking()
                .Where(_ => _.ScenarioBudget.SimulationId == simulationId)
                .Include(sb => sb.ScenarioBudget)
                .Select(budgetAmount => new BudgetAmountDTO
                {
                    Year = budgetAmount.Year,
                    Value = budgetAmount.Value,
                    BudgetName = budgetAmount.ScenarioBudget.Name,
                }).AsNoTracking().ToList();
        }
    }
}
