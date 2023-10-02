using AppliedResearchAssociates.iAM.DTOs;
using System;
using System.Collections.Generic;

namespace BridgeCareCore.Services.SimulationCloning
{
    internal class BudgetAmountCloner
    {
        internal static BudgetAmountDTO Clone(BudgetAmountDTO budgetAmount)
        {
            var clone = new BudgetAmountDTO
            {
              Id = Guid.NewGuid(),
              BudgetName = budgetAmount.BudgetName,
              Value = budgetAmount.Value,
              Year = budgetAmount.Year,
            };
            return clone;
        }
        internal static List<BudgetAmountDTO> CloneList(IEnumerable<BudgetAmountDTO> budgetAmounts)
        {
            var clone = new List<BudgetAmountDTO>();
            foreach (var budgetAmount in budgetAmounts)
            {
                var childClone = Clone(budgetAmount);
                clone.Add(childClone);
            }
            return clone;

        }

    }
}
