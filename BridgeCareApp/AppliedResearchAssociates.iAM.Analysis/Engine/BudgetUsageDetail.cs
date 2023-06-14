using System;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    /// <summary>
    /// Details about how a specific budget is used in funding a treatment
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
        /// Name of the budget used
        /// </summary>
        public string BudgetName { get; }

        /// <summary>
        /// Amount of the budget used for the treatment
        /// </summary>
        public decimal CoveredCost { get; set; }

        /// <summary>
        /// Status of the decision as it relates to using this specific budget
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
