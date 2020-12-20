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

        private readonly IAMContext _context;

        public BudgetAmountRepository(IAMContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

        public void CreateBudgetAmounts(Dictionary<Guid, List<BudgetAmount>> budgetAmountsPerBudgetEntityId, Guid simulationId)
        {
            if (!_context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            var simulationEntity = _context.Simulation
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
                _context.BudgetAmount.AddRange(budgetAmountEntities);
            }
            else
            {
                _context.BulkInsert(budgetAmountEntities);
            }
        }
    }
}
