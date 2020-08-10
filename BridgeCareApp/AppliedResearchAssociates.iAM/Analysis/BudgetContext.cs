using System;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Analysis
{
    internal sealed class BudgetContext
    {
        public BudgetContext(Budget budget, int firstYearOfAnalysisPeriod)
        {
            Budget = budget ?? throw new ArgumentNullException(nameof(budget));
            FirstYearOfAnalysisPeriod = firstYearOfAnalysisPeriod;

            var cumulativeAmount = 0m;
            CumulativeAmountPerYear = Budget.YearlyAmounts.Select(amount => cumulativeAmount += amount.Value).ToArray();
        }

        public Budget Budget { get; }

        public decimal CurrentAmount => CumulativeAmountPerYear[CurrentYearIndex];

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
            for (var yearIndex = CurrentYearIndex; yearIndex < CumulativeAmountPerYear.Length; ++yearIndex)
            {
                CumulativeAmountPerYear[yearIndex] -= cost;
            }

            CurrentPrioritizedAmount -= cost;
        }

        public void MoveToNextYear()
        {
            ++CurrentYearIndex;
            CurrentPrioritizedAmount = null;
        }

        public void SetYear(int year)
        {
            CurrentYearIndex = year - FirstYearOfAnalysisPeriod;
            CurrentPrioritizedAmount = null;
        }

        internal BudgetContext(BudgetContext original)
        {
            Budget = original.Budget;
            CumulativeAmountPerYear = (decimal[])CumulativeAmountPerYear.Clone();

            CurrentYearIndex = original.CurrentYearIndex;
            CurrentPrioritizedAmount = original.CurrentPrioritizedAmount;
        }

        private readonly decimal[] CumulativeAmountPerYear;

        private readonly int FirstYearOfAnalysisPeriod;

        private int CurrentYearIndex = -1;
    }
}
