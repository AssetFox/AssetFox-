using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using System.Linq;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class BudgetPriorityMapper
    {
        public static BudgetPriorityEntity ToEntity(this BudgetPriority domain, Guid budgetPriorityLibraryId) =>
            new BudgetPriorityEntity
            {
                Id = Guid.NewGuid(),
                BudgetPriorityLibraryId = budgetPriorityLibraryId,
                PriorityLevel = domain.PriorityLevel,
                Year = domain.Year
            };

        public static void ToSimulationAnalysisDomain(this BudgetPriorityEntity entity, AnalysisMethod analysisMethod,
            InvestmentPlan investmentPlan)
        {
            var priority = analysisMethod.AddBudgetPriority();
            priority.PriorityLevel = entity.PriorityLevel;
            priority.Year = entity.Year;
            priority.Criterion.Expression =
                entity.CriterionLibraryBudgetPriorityJoin?.CriterionLibrary.MergedCriteriaExpression ?? string.Empty;

            if (entity.BudgetPercentagePairs.Any())
            {
                entity.BudgetPercentagePairs.ForEach(_ =>
                {
                    _.ToSimulationAnalysisDomain(investmentPlan, priority);
                });
            }
        }
    }
}
