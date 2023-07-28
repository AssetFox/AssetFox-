using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
{
    internal class CashFlowRuleCloner
    {
        internal static CashFlowRuleDTO Clone(CashFlowRuleDTO cashFlowRule)
        {
            var cloneCritionLibrary = CriterionLibraryCloner.Clone(cashFlowRule.CriterionLibrary);
            var clone = new CashFlowRuleDTO
            {
                Id = Guid.NewGuid(),
                LibraryId = cashFlowRule.LibraryId,
                CriterionLibrary = cloneCritionLibrary,
                CashFlowDistributionRules = cashFlowRule.CashFlowDistributionRules,
                IsModified = cashFlowRule.IsModified,
                Name = cashFlowRule.Name,                
            };
            return clone;
        }

        internal static List<CashFlowRuleDTO> CloneList(IEnumerable<CashFlowRuleDTO> cashFlowRules)
        {
            var clone = new List<CashFlowRuleDTO>();
            foreach (var cashFlowRule in cashFlowRules)
            {
                var childClone = Clone(cashFlowRule);
                clone.Add(childClone);
            }
            return clone;

        }
    }
}
