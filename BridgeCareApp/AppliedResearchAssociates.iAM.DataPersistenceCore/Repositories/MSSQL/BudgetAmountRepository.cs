using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class BudgetAmountRepository : IBudgetAmountRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfWork _unitOfWork;

        public BudgetAmountRepository(UnitOfWork.UnitOfWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateBudgetAmounts(Dictionary<Guid, List<BudgetAmount>> budgetAmountsPerBudgetEntityId, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            var simulationEntity = _unitOfWork.Context.Simulation
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
                _unitOfWork.Context.BudgetAmount.AddRange(budgetAmountEntities);
            }
            else
            {
                _unitOfWork.Context.BulkInsert(budgetAmountEntities);
            }

            _unitOfWork.Context.SaveChanges();
        }
    }
}
