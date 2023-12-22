using AppliedResearchAssociates.iAM.DTOs;
using System.Collections.Generic;
using System;

namespace BridgeCareCore.Services.DtoHelpers
{
    public static class BudgetPriorityDtoListHelper
    {
        public static void AddLibraryIdToScenarioBudgetPriority(List<BudgetPriorityDTO> budgetPriorityDTOs, Guid? libraryId)
        {
            if (libraryId == null) return;
            foreach (var dto in budgetPriorityDTOs)
            {
                dto.libraryId = (Guid)libraryId;
            }
        }
        public static void AddModifiedToScenarioBudgetPriority(List<BudgetPriorityDTO> budgetPriorityDTOs, bool IsModified)
        {
            foreach (var dto in budgetPriorityDTOs)
            {
                dto.IsModified = IsModified;
            }
        }
    }
}
