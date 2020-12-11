using System;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class BudgetPercentagePairMapper
    {
        public static BudgetPercentagePairEntity ToEntity(this BudgetPercentagePair domain, Guid budgetPriorityId, Guid budgetId) =>
            new BudgetPercentagePairEntity
            {
                Id = domain.Id,
                BudgetPriorityId = budgetPriorityId,
                BudgetId = budgetId,
                Percentage = domain.Percentage
            };

        public static void FillBudgetPercentagePair(this BudgetPercentagePairEntity entity, InvestmentPlan investmentPlan,
            BudgetPriority budgetPriority)
        {
            var budget = investmentPlan.Budgets.Single(_ => _.Id == entity.Budget.Id);
            var budgetPercentagePair = budgetPriority.GetBudgetPercentagePair(budget);
            budgetPercentagePair.Id = entity.Id;
            budgetPercentagePair.Percentage = entity.Percentage;
        }
    }
}
