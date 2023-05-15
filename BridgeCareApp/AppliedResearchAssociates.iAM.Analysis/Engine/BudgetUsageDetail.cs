using System;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    /// <summary>
    /// .
    /// </summary>
    public sealed class BudgetUsageDetail
    {
        public BudgetUsageDetail(string budgetName)
        {
            if (string.IsNullOrWhiteSpace(budgetName))
            {
                throw new ArgumentException("Budget name is blank.", nameof(budgetName));
            }

            BudgetName = budgetName;
        }

        /// <summary>
        /// .
        /// </summary>
        public string BudgetName { get; }

        /// <summary>
        /// .
        /// </summary>
        public decimal CoveredCost { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public BudgetUsageStatus Status { get; set; }

        internal BudgetUsageDetail(BudgetUsageDetail original)
        {
            BudgetName = original.BudgetName;
            Status = original.Status;
            CoveredCost = original.CoveredCost;
        }
    }
}
