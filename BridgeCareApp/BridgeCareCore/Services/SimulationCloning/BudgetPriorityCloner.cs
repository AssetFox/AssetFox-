using AppliedResearchAssociates.iAM.DTOs;
using System;
using System.Collections.Generic;

namespace BridgeCareCore.Services.SimulationCloning
{
    internal class BudgetPriorityCloner
    {
        internal static BudgetPriorityDTO Clone(BudgetPriorityDTO budgetPriority, Dictionary<Guid, Guid> budgetIdMap, Guid ownerId)
        {
            var cloneCriterionLibrary = CriterionLibraryCloner.CloneNullPropagating(budgetPriority.CriterionLibrary, ownerId);
            var cloneBudgetPercentagePair = BudgetPercentagePairCloner.CloneList(budgetPriority.BudgetPercentagePairs, budgetIdMap);
            var clone = new BudgetPriorityDTO
            {
               Id = Guid.NewGuid(),
               libraryId = budgetPriority.libraryId,
               CriterionLibrary = cloneCriterionLibrary,
               PriorityLevel = budgetPriority.PriorityLevel,
               BudgetPercentagePairs = cloneBudgetPercentagePair,
               IsModified = budgetPriority.IsModified,
               Year = budgetPriority.Year,
            };
            return clone;
        }      
        internal static List<BudgetPriorityDTO> CloneList(IEnumerable<BudgetPriorityDTO> budgetPriorities, Dictionary<Guid, Guid> budgetIdMap, Guid ownerId)
        {
            var clone = new List<BudgetPriorityDTO>();
            foreach (var budgetPrioritiy in budgetPriorities)
            {
                var childClone = Clone(budgetPrioritiy, budgetIdMap, ownerId);
                clone.Add(childClone);
            }
            return clone;

        }
    }
}
