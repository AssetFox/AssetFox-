using AppliedResearchAssociates.iAM.DTOs;
using System;
using System.Collections.Generic;

namespace BridgeCareCore.Services.SimulationCloning
{
    internal class BudgetPriorityCloner
    {
        internal static BudgetPriorityDTO Clone(BudgetPriorityDTO budgetPriority)
        {
            var cloneCritionLibrary = CriterionLibraryCloner.Clone(budgetPriority.CriterionLibrary);
            var clone = new BudgetPriorityDTO
            {
               Id = Guid.NewGuid(),
               libraryId = budgetPriority.libraryId,
               CriterionLibrary = cloneCritionLibrary,
               PriorityLevel = budgetPriority.PriorityLevel,
               BudgetPercentagePairs = budgetPriority.BudgetPercentagePairs,
               IsModified = budgetPriority.IsModified,
               Year = budgetPriority.Year,
            };
            return clone;
        }      
        internal static List<BudgetPriorityDTO> CloneList(IEnumerable<BudgetPriorityDTO> budgetPriorities)
        {
            var clone = new List<BudgetPriorityDTO>();
            foreach (var budgetPrioritiy in budgetPriorities)
            {
                var childClone = Clone(budgetPrioritiy);
                clone.Add(childClone);
            }
            return clone;

        }
    }
}
