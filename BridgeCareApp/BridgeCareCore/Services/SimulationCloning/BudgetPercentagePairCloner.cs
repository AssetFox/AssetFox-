using AppliedResearchAssociates.iAM.DTOs;
using System;

namespace BridgeCareCore.Services.SimulationCloning
{
    internal class BudgetPercentagePairCloner
    {
        internal static BudgetPercentagePairDTO Clone(BudgetPercentagePairDTO budgetPercentagePair)
        {
            var clone = new BudgetPercentagePairDTO
            {
                Id = Guid.NewGuid(),
                BudgetId = budgetPercentagePair.Id,
                BudgetName = budgetPercentagePair.BudgetName,
                Percentage = budgetPercentagePair.Percentage,                
            };
            return clone;
        }

    }
}
