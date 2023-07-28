using AppliedResearchAssociates.iAM.DTOs;
using System;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
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
