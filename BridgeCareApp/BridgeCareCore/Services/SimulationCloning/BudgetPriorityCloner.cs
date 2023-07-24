using AppliedResearchAssociates.iAM.DTOs;
using System;

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

    }
}
