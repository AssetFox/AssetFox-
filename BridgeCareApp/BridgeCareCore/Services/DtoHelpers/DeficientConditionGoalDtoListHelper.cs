using AppliedResearchAssociates.iAM.DTOs;
using System.Collections.Generic;
using System;

namespace BridgeCareCore.Services
{
    public static class DeficientConditionGoalDtoListHelper
    {
        public static void AddLibraryIdToScenarioDeficientConditionGoal(List<DeficientConditionGoalDTO> deficientConditionGoalDTOs, Guid? libraryId)
        {
            if (libraryId != null)
            {
                foreach (var dto in deficientConditionGoalDTOs)
                {
                    dto.LibraryId = (Guid)libraryId;
                }
            }
        }

        public static void AddModifiedToScenarioDeficientConditionGoal(List<DeficientConditionGoalDTO> deficientConditionGoalDTOs, bool IsModified)
        {
            foreach (var dto in deficientConditionGoalDTOs)
            {
                dto.IsModified = IsModified;
            }
        }
    }
}
