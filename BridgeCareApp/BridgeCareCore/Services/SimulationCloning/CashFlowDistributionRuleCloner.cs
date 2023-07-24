using System;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services.SimulationCloning
{
    internal class CashFlowDistributionRuleCloner
    {
        internal static CashFlowDistributionRuleDTO Clone(CashFlowDistributionRuleDTO cashFlowDistributionRule)
        {           
            var clone = new CashFlowDistributionRuleDTO
            {
                Id = Guid.NewGuid(),
                CostCeiling = cashFlowDistributionRule.CostCeiling,
                DurationInYears = cashFlowDistributionRule.DurationInYears,
                YearlyPercentages = cashFlowDistributionRule.YearlyPercentages,
            };
            return clone;
        }

    }
}
