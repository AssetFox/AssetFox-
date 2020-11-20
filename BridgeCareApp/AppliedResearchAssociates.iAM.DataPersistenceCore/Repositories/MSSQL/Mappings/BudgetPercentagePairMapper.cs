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
                Id = Guid.NewGuid(),
                BudgetPriorityId = budgetPriorityId,
                BudgetId = budgetId,
                Percentage = domain.Percentage
            };

        public static void ToSimulationAnalysisDomain(this BudgetPercentagePairEntity entity, InvestmentPlan investmentPlan,
            BudgetPriority budgetPriority)
        {
            var budget = investmentPlan.Budgets.ToList()
                .Single(_ => _.Name == entity.Budget.Name);
            var budgetPercentagePair = budgetPriority.GetBudgetPercentagePair(budget);
            budgetPercentagePair.Percentage = entity.Percentage;
        }
    }
}
