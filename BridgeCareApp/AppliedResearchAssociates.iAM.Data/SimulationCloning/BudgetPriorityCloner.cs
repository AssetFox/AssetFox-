using AppliedResearchAssociates.iAM.DTOs;
using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
{
    internal class BudgetPriorityCloner
    {
        internal static BudgetPriorityDTO Clone(BudgetPriorityDTO budgetPriority, Guid ownerId)
        {
            var cloneCriterionLibrary = CriterionLibraryCloner.CloneNullPropagating(budgetPriority.CriterionLibrary, ownerId);
            var clone = new BudgetPriorityDTO
            {
               Id = Guid.NewGuid(),
               libraryId = budgetPriority.libraryId,
               CriterionLibrary = cloneCriterionLibrary,
               PriorityLevel = budgetPriority.PriorityLevel,
               BudgetPercentagePairs = budgetPriority.BudgetPercentagePairs,
               IsModified = budgetPriority.IsModified,
               Year = budgetPriority.Year,
            };
            return clone;
        }      
        internal static List<BudgetPriorityDTO> CloneList(IEnumerable<BudgetPriorityDTO> budgetPriorities, Guid ownerId)
        {
            var clone = new List<BudgetPriorityDTO>();
            foreach (var budgetPrioritiy in budgetPriorities)
            {
                var childClone = Clone(budgetPrioritiy, ownerId);
                clone.Add(childClone);
            }
            return clone;

        }
    }
}
