using AppliedResearchAssociates.iAM.DTOs;
using System;

namespace BridgeCareCore.Services.SimulationCloning
{
    internal class BudgetCloner
    {
        internal static BudgetDTO Clone(BudgetDTO budget)
        {
            var cloneCritionLibrary = CriterionLibraryCloner.Clone(budget.CriterionLibrary);
            var clone = new BudgetDTO
            {
                LibraryId = budget.LibraryId,
                CriterionLibrary = cloneCritionLibrary,
                BudgetAmounts = budget.BudgetAmounts,
                BudgetOrder = budget.BudgetOrder,
                IsModified = budget.IsModified,
                Name = budget.Name,

            };
            return clone;
        }

    }
}
