using AppliedResearchAssociates.iAM.DTOs;
using System;
using System.Collections.Generic;

namespace BridgeCareCore.Services.SimulationCloning
{

    internal class DeficientConditionGoalCloner
    {
        internal static DeficientConditionGoalDTO Clone(DeficientConditionGoalDTO deficientConditionGoal, Guid ownerId)
        {
            var cloneCriterionLibrary = CriterionLibraryCloner.CloneNullPropagating(deficientConditionGoal.CriterionLibrary, ownerId);
            var clone = new DeficientConditionGoalDTO
            {
                LibraryId = deficientConditionGoal.LibraryId,
                CriterionLibrary = cloneCriterionLibrary,
                Attribute = deficientConditionGoal.Attribute,
                DeficientLimit = deficientConditionGoal.DeficientLimit,
                AllowedDeficientPercentage = deficientConditionGoal.AllowedDeficientPercentage,
                IsModified = deficientConditionGoal.IsModified,
                Name = deficientConditionGoal.Name,
            };
            return clone;
        }
        internal static List<DeficientConditionGoalDTO> CloneList(IEnumerable<DeficientConditionGoalDTO> deficientConditionGoals, Guid ownerId)
        {
            var clone = new List<DeficientConditionGoalDTO>();
            foreach (var deficientConditionGoal in deficientConditionGoals)
            {
                var childClone = Clone(deficientConditionGoal, ownerId);
                clone.Add(childClone);
            }
            return clone;

        }
    }
}
