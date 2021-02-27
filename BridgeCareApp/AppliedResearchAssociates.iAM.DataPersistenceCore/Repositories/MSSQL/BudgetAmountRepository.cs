using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using System.Linq;
using System.Linq.Expressions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class BudgetAmountRepository : IBudgetAmountRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public BudgetAmountRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfDataPersistenceWork) => _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));

        public void CreateBudgetAmounts(Dictionary<Guid, List<BudgetAmount>> budgetAmountsPerBudgetEntityId, Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            var simulationEntity = _unitOfDataPersistenceWork.Context.Simulation
                .Include(_ => _.InvestmentPlan)
                .Single(_ => _.Id == simulationId);

            if (simulationEntity.InvestmentPlan == null)
            {
                throw new RowNotInTableException($"No investment plan found for simulation having id {simulationId}.");
            }

            var budgetAmountEntities = new List<BudgetAmountEntity>();

            budgetAmountsPerBudgetEntityId.Keys.ForEach(budgetEntityId =>
            {
                var year = simulationEntity.InvestmentPlan.FirstYearOfAnalysisPeriod;
                budgetAmountsPerBudgetEntityId[budgetEntityId].ForEach(_ =>
                {
                    budgetAmountEntities.Add(_.ToEntity(budgetEntityId, year));
                    year++;
                });
            });

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.BudgetAmount.AddRange(budgetAmountEntities);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkInsert(budgetAmountEntities);
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        public void AddOrUpdateOrDeleteBudgetAmounts(Dictionary<Guid, List<BudgetAmountDTO>> budgetAmountsPerBudgetId, Guid libraryId)
        {
            var entities = budgetAmountsPerBudgetId.SelectMany(_ => _.Value.Select(__ => __.ToEntity(_.Key))).ToList();

            var entityIds = entities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfDataPersistenceWork.Context.BudgetAmount
                .Where(_ => _.Budget.BudgetLibraryId == libraryId && entityIds.Contains(_.Id)).Select(_ => _.Id)
                .ToList();

            var predicatesPerCrudOperation = new Dictionary<string, Expression<Func<BudgetAmountEntity, bool>>>
            {
                {"delete", _ => _.Budget.BudgetLibraryId == libraryId && !entityIds.Contains(_.Id)},
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
        }
    }
}
