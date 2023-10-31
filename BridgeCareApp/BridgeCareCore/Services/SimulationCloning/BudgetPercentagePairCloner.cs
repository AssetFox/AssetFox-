using AppliedResearchAssociates.iAM.DTOs;
using System;
using System.Collections.Generic;

namespace BridgeCareCore.Services
{
    internal class BudgetPercentagePairCloner
    {
        internal static BudgetPercentagePairDTO Clone(BudgetPercentagePairDTO budgetPercentagePair, Dictionary<Guid, Guid> budgetIdMap)
        {
            var newBudgetId = budgetIdMap[budgetPercentagePair.BudgetId];
            var clone = new BudgetPercentagePairDTO
            {
                Id = Guid.NewGuid(),
                BudgetId = newBudgetId,
                BudgetName = budgetPercentagePair.BudgetName,
                Percentage = budgetPercentagePair.Percentage,                
            };
            return clone;
        }
        internal static List<BudgetPercentagePairDTO> CloneList(IEnumerable<BudgetPercentagePairDTO> budgetPercentagePairs, Dictionary<Guid, Guid> budgetIdMap)
        {
            var clone = new List<BudgetPercentagePairDTO>();
            foreach (var budgetPercentagePair in budgetPercentagePairs)
            {
                var childClone = Clone(budgetPercentagePair, budgetIdMap);
                clone.Add(childClone);
            }
            return clone;

        }
    }
}
