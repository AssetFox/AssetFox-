using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
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

        internal static List<CashFlowDistributionRuleDTO> CloneList(IEnumerable<CashFlowDistributionRuleDTO> cashFlowDistrubtionRules)
        {
            var clone = new List<CashFlowDistributionRuleDTO>();
            foreach (var cashFlowRule in cashFlowDistrubtionRules)
            {
                var childClone = Clone(cashFlowRule);
                clone.Add(childClone);
            }
            return clone;

        }
    }
}
