using AppliedResearchAssociates.iAM.DTOs;
using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
{
    internal class TargetConditionGoalCloner
    {
        internal static TargetConditionGoalDTO Clone(TargetConditionGoalDTO targetConditionGoal, Guid ownerId)
        {
            var cloneCritionLibrary = CriterionLibraryCloner.Clone(targetConditionGoal.CriterionLibrary, ownerId);
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
        internal static List<TargetConditionGoalDTO> CloneList(IEnumerable<TargetConditionGoalDTO> targetConditionGoals, Guid ownerId)
        {
            var clone = new List<TargetConditionGoalDTO>();
            foreach (var targetConditionGoal in targetConditionGoals)
            {
                var childClone = Clone(targetConditionGoal, ownerId);
                clone.Add(childClone);
            }
            return clone;

        }
    }
}
