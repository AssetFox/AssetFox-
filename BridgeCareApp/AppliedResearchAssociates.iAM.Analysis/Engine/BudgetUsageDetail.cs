using System;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
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

        public string BudgetName { get; } // WjJake -- I'm guessing a lot of these properties will now need setters, or at least init setters

        public decimal CoveredCost { get; set; }

        public BudgetUsageStatus Status { get; set; }

        internal BudgetUsageDetail(BudgetUsageDetail original)
        {
            BudgetName = original.BudgetName;
            Status = original.Status;
            CoveredCost = original.CoveredCost;
        }
    }
}
