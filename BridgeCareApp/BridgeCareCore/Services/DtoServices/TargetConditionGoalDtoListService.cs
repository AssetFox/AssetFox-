using AppliedResearchAssociates.iAM.DTOs;
using System.Collections.Generic;
using System;

namespace BridgeCareCore.Services
{
    public static class TargetConditionGoalDtoListService
    {
        public static void AddLibraryIdToScenarioTargetConditionGoal(List<TargetConditionGoalDTO> targetConditionGoalDTOs, Guid? libraryId)
        {
            if (libraryId == null) return;
            foreach (var dto in targetConditionGoalDTOs)
            {
                dto.LibraryId = (Guid)libraryId;
            }
        }

        public static void AddModifiedToScenarioTargetConditionGoal(List<TargetConditionGoalDTO> targetConditionGoalDTOs, bool IsModified)
        {
            foreach (var dto in targetConditionGoalDTOs)
            {
                dto.IsModified = IsModified;
            }
        }
    }
}
