using System;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    /// <summary>
    /// .
    /// </summary>
    public sealed class BudgetDetail
    {
        public BudgetDetail(Budget budget, decimal availableFunding)
        {
            BudgetName = budget?.Name ?? throw new ArgumentNullException(nameof(budget));
            AvailableFunding = availableFunding;
        }

        [JsonConstructor]
        public BudgetDetail(decimal availableFunding, string budgetName)
        {
            AvailableFunding = availableFunding;
            BudgetName = budgetName ?? throw new ArgumentNullException(nameof(budgetName));
        }

        /// <summary>
        /// .
        /// </summary>
        public decimal AvailableFunding { get; }

        /// <summary>
        /// .
        /// </summary>
        public string BudgetName { get; }

        internal BudgetDetail(BudgetDetail original)
        {
            AvailableFunding = original.AvailableFunding;
            BudgetName = original.BudgetName;
        }
    }
}
