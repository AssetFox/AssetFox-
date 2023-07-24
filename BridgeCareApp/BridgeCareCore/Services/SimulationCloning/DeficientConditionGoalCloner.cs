using AppliedResearchAssociates.iAM.DTOs;
using System;

namespace BridgeCareCore.Services.SimulationCloning
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

    }
}
