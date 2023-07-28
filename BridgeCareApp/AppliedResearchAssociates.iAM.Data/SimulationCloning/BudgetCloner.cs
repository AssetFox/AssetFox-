using AppliedResearchAssociates.iAM.DTOs;
using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
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

        internal static List<BudgetDTO> CloneList(IEnumerable<BudgetDTO> budgets)
        {
            var clone = new List<BudgetDTO>();
            foreach (var budget in budgets)
            {
                var childClone = Clone(budget);
                clone.Add(childClone);
            }
            return clone;

        }
    }
}
