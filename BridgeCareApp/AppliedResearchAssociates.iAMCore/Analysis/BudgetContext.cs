using System;
using System.Linq;

namespace AppliedResearchAssociates.iAMCore.Analysis
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

        public void LimitPreviousAmountToCurrentAmount()
        {
            if (CurrentYearIndex > 0)
            {
                var previousYearIndex = CurrentYearIndex - 1;
                CumulativeAmountPerYear[previousYearIndex] = Math.Min(CumulativeAmountPerYear[previousYearIndex], CumulativeAmountPerYear[CurrentYearIndex]);
            }
        }

        public void MoveToNextYear() => SetYearIndex(CurrentYearIndex + 1);

        public void SetYear(int year) => SetYearIndex(year - FirstYearOfAnalysisPeriod);

        internal BudgetContext(BudgetContext original)
        {
            Budget = original.Budget;
            FirstYearOfAnalysisPeriod = original.FirstYearOfAnalysisPeriod;

            CumulativeAmountPerYear = original.CumulativeAmountPerYear.ToArray();

            CurrentYearIndex = original.CurrentYearIndex;
            CurrentPrioritizedAmount = original.CurrentPrioritizedAmount;
        }

        private readonly decimal[] CumulativeAmountPerYear;

        private readonly int FirstYearOfAnalysisPeriod;

        private int CurrentYearIndex = 0;

        private void SetYearIndex(int yearIndex)
        {
            CurrentYearIndex = yearIndex;
            CurrentPrioritizedAmount = null;
        }
    }
}
