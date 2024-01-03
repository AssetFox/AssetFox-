using AppliedResearchAssociates.iAM.DTOs;
using System.Collections.Generic;
using System;

namespace BridgeCareCore.Services
{
    public static class CashFlowRuleDtoListService
    {
        public static void AddLibraryIdToScenarioCashFlowRule(List<CashFlowRuleDTO> cashFlowRuleDTOs, Guid? libraryId)
        {
            if (libraryId == null) return;
            foreach (var dto in cashFlowRuleDTOs)
            {
                dto.LibraryId = (Guid)libraryId;
            }
        }
        public static void AddModifiedToScenarioCashFlowRule(List<CashFlowRuleDTO> cashFlowRuleDTOs, bool IsModified)
        {
            foreach (var dto in cashFlowRuleDTOs)
            {
                dto.IsModified = IsModified;
            }
        }

    }
}
