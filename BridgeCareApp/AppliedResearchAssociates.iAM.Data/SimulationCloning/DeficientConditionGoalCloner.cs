using AppliedResearchAssociates.iAM.DTOs;
using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
{

    internal class DeficientConditionGoalCloner
    {
        internal static DeficientConditionGoalDTO Clone(DeficientConditionGoalDTO deficientConditionGoal)
        {
            var cloneCritionLibrary = CriterionLibraryCloner.Clone(deficientConditionGoal.CriterionLibrary);
            var clone = new DeficientConditionGoalDTO
            {
                LibraryId = deficientConditionGoal.LibraryId,
                CriterionLibrary = cloneCritionLibrary,
                DeficientLimit = deficientConditionGoal.DeficientLimit,
                AllowedDeficientPercentage = deficientConditionGoal.AllowedDeficientPercentage,
                IsModified = deficientConditionGoal.IsModified,
                Name = deficientConditionGoal.Name,
            };
            return clone;
        }
        internal static List<DeficientConditionGoalDTO> CloneList(IEnumerable<DeficientConditionGoalDTO> deficientConditionGoals)
        {
            var clone = new List<DeficientConditionGoalDTO>();
            foreach (var deficientConditionGoal in deficientConditionGoals)
            {
                var childClone = Clone(deficientConditionGoal);
                clone.Add(childClone);
            }
            return clone;

        }
    }
}
