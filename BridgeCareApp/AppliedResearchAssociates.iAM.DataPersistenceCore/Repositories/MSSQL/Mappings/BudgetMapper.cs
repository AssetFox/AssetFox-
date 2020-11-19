using System;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class BudgetMapper
    {
        public static BudgetEntity ToEntity(this Budget domain, Guid investmentPlanId) =>
            new BudgetEntity
            {
                Id = Guid.NewGuid(),
                InvestmentPlanId = investmentPlanId,
                Name = domain.Name
            };

        public static Budget ToSimulationAnalysisDomain(this BudgetEntity entity, InvestmentPlan investmentPlan)
        {
            var budget = investmentPlan.AddBudget();
            budget.Name = entity.Name;
            if (entity.BudgetAmounts.Any())
            {
                var sortedBudgetAmountEntities = entity.BudgetAmounts.OrderBy(_ => _.Year);
                sortedBudgetAmountEntities.ForEach(_ =>
                {
                    var year = _.Year;
                    var yearOffset = year - investmentPlan.FirstYearOfAnalysisPeriod;
                    budget.YearlyAmounts[yearOffset].Value = _.Value;
                });
            }

            return budget;
        }
    }
}
