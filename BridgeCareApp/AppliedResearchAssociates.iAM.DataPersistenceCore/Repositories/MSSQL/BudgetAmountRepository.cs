using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore.Internal;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class BudgetAmountRepository : MSSQLRepository, IBudgetAmountRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        public BudgetAmountRepository(IAMContext context) : base(context) { }

        public void CreateBudgetAmounts(Dictionary<Guid, List<BudgetAmount>> budgetAmountsPerBudgetEntityId, Guid investmentPlanId)
        {
            if (!Context.InvestmentPlan.Any(_ => _.Id == investmentPlanId))
            {
                throw new RowNotInTableException($"No investment plan found having id {investmentPlanId}.");
            }

            var investmentPlanEntity = Context.InvestmentPlan.Single(_ => _.Id == investmentPlanId);

            var budgetAmountEntities = new List<BudgetAmountEntity>();

            budgetAmountsPerBudgetEntityId.Keys.ForEach(budgetEntityId =>
            {
                var year = investmentPlanEntity.FirstYearOfAnalysisPeriod;
                budgetAmountsPerBudgetEntityId[budgetEntityId].ForEach(_ =>
                {
                    budgetAmountEntities.Add(_.ToEntity(budgetEntityId, year));
                    year++;
                });
            });

            if (IsRunningFromXUnit)
            {
                Context.BudgetAmount.AddRange(budgetAmountEntities);
            }
            else
            {
                Context.BulkInsert(budgetAmountEntities);
            }

            Context.SaveChanges();
        }
    }
}
