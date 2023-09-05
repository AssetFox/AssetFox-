using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
{
    internal class CashFlowRuleCloner
    {
        internal static CashFlowRuleDTO Clone(CashFlowRuleDTO cashFlowRule, Guid ownerId)
        {
            var cloneCriterionLibrary = CriterionLibraryCloner.CloneNullPropagating(cashFlowRule.CriterionLibrary, ownerId);
            var cloneDistributionRules = CashFlowDistributionRuleCloner.CloneList(cashFlowRule.CashFlowDistributionRules);
            var clone = new CashFlowRuleDTO
            {
                Id = Guid.NewGuid(),
                LibraryId = cashFlowRule.LibraryId,
                CriterionLibrary = cloneCriterionLibrary,
                CashFlowDistributionRules = cloneDistributionRules,
                IsModified = cashFlowRule.IsModified,
                Name = cashFlowRule.Name,                
            };
            return clone;
        }

        internal static List<CashFlowRuleDTO> CloneList(IEnumerable<CashFlowRuleDTO> cashFlowRules, Guid ownerId)
        {
            var clone = new List<CashFlowRuleDTO>();
            foreach (var cashFlowRule in cashFlowRules)
            {
                var childClone = Clone(cashFlowRule, ownerId);
                clone.Add(childClone);
            }
            return clone;

        }
    }
}
