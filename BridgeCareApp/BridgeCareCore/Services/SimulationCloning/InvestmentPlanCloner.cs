using System;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services.SimulationCloning
{
    internal class InvestmentPlanCloner
    {
        internal static InvestmentPlanDTO Clone(InvestmentPlanDTO investmentPlan)
        {
            var clone = new InvestmentPlanDTO
            {
                Id = Guid.NewGuid(),
                MinimumProjectCostLimit = investmentPlan.MinimumProjectCostLimit,
                FirstYearOfAnalysisPeriod = investmentPlan.FirstYearOfAnalysisPeriod,               
                InflationRatePercentage = investmentPlan.InflationRatePercentage,
                NumberOfYearsInAnalysisPeriod = investmentPlan.NumberOfYearsInAnalysisPeriod,
                ShouldAccumulateUnusedBudgetAmounts = investmentPlan.ShouldAccumulateUnusedBudgetAmounts,
            };
            return clone;
        }

    }
}
