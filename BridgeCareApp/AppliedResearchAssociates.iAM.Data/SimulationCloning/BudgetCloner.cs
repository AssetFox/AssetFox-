using AppliedResearchAssociates.iAM.DTOs;
using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
{
    internal class BudgetCloner
    {
        internal static BudgetDTO Clone(BudgetDTO budget, Guid ownerId)
        {
            var cloneCritionLibrary = CriterionLibraryCloner.Clone(budget.CriterionLibrary, ownerId);
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

        internal static List<BudgetDTO> CloneList(IEnumerable<BudgetDTO> budgets, Guid ownerId)
        {
            var clone = new List<BudgetDTO>();
            foreach (var budget in budgets)
            {
                var childClone = Clone(budget, ownerId);
                clone.Add(childClone);
            }
            return clone;

        }
    }
}
