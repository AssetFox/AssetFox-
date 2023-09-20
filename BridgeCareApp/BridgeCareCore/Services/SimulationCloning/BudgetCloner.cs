using AppliedResearchAssociates.iAM.DTOs;
using System;
using System.Collections.Generic;

namespace BridgeCareCore.Services.SimulationCloning
{
    internal class BudgetCloner
    {
        internal static BudgetDTO Clone(BudgetDTO budget, Guid ownerId)
        {
            var cloneCriterionLibrary = CriterionLibraryCloner.CloneNullPropagating(budget.CriterionLibrary, ownerId);
            var cloneBudgetAmounts = BudgetAmountCloner.CloneList(budget.BudgetAmounts);
            var clone = new BudgetDTO
            {
                Id = Guid.NewGuid(),
                LibraryId = budget.LibraryId,
                CriterionLibrary = cloneCriterionLibrary,
                BudgetAmounts = cloneBudgetAmounts,
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
