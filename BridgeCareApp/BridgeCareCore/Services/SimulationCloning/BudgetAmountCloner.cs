using System;
using AppliedResearchAssociates.iAM.DTOs;

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

    }
}
