using AppliedResearchAssociates.iAM.DTOs;
using System;
using System.Collections.Generic;

namespace BridgeCareCore.Services.SimulationCloning
{
    internal class TargetConditionGoalCloner
    {
        internal static TargetConditionGoalDTO Clone(TargetConditionGoalDTO targetConditionGoal)
        {
            var cloneCritionLibrary = CriterionLibraryCloner.Clone(targetConditionGoal.CriterionLibrary);
            var clone = new TargetConditionGoalDTO
            {
               Id = Guid.NewGuid(),
               LibraryId = targetConditionGoal.LibraryId,
               CriterionLibrary = cloneCritionLibrary,
               Attribute = targetConditionGoal.Attribute,
               IsModified = targetConditionGoal.IsModified,
               Name = targetConditionGoal.Name,
               Target = targetConditionGoal.Target,
               Year = targetConditionGoal.Year,
            };
            return clone;
        }
        internal static List<TargetConditionGoalDTO> CloneList(IEnumerable<TargetConditionGoalDTO> targetConditionGoals)
        {
            var clone = new List<TargetConditionGoalDTO>();
            foreach (var targetConditionGoal in targetConditionGoals)
            {
                var childClone = Clone(targetConditionGoal);
                clone.Add(childClone);
            }
            return clone;

        }
    }
}
