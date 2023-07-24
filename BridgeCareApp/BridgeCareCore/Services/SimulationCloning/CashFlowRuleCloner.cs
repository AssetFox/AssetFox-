using System;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services.SimulationCloning
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

    }
}
