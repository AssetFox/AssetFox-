using AppliedResearchAssociates.iAM.DTOs;
using System.Collections.Generic;
using System;

namespace BridgeCareCore.Services
{
    public static class BudgetDtoListHelper
    {
        public static void AddLibraryIdToScenarioBudget(List<BudgetDTO> budgetDTOs, Guid? libraryId)
        {
            if (libraryId == null) return;
            foreach (var dto in budgetDTOs)
            {
                dto.LibraryId = (Guid)libraryId;
            }
        }
        public static void AddModifiedToScenarioBudget(List<BudgetDTO> budgetDTOs, bool IsModified)
        {
            foreach (var dto in budgetDTOs)
            {
                dto.IsModified = IsModified;
            }
        }
    }
}
