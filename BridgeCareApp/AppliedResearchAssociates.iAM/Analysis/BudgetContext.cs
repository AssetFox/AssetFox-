using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis
{
    internal sealed class BudgetContext
    {
        public BudgetContext(Budget budget) => Budget = budget ?? throw new ArgumentNullException(nameof(budget));

        public Budget Budget { get; }

        public decimal CurrentAmount { get; private set; }

        public decimal? CurrentPrioritizedAmount { get; private set; }

        public BudgetPriority Priority
        {
            set
            {
                var prioritizedFraction = value?.GetBudgetPercentagePair(Budget).Percentage / 100;
                CurrentPrioritizedAmount = CurrentAmount * prioritizedFraction;
            }
        }

        public void AllocateCost(decimal cost)
        {
            CurrentAmount -= cost;
            CurrentPrioritizedAmount -= cost;
        }

        public void MoveToNextYear()
        {
            ++CurrentYearIndex;

            if (!OverriddenYearlyAmounts.TryGetValue(CurrentYearIndex, out var yearlyAmount))
            {
                yearlyAmount = Budget.YearlyAmounts[CurrentYearIndex].Value;
            }

            CurrentAmount += yearlyAmount;
            CurrentPrioritizedAmount = null;
        }

        private readonly Dictionary<int, decimal> OverriddenYearlyAmounts = new Dictionary<int, decimal>();

        private int CurrentYearIndex = -1;
    }
}
