using System;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

internal sealed class BudgetContext
{
    public BudgetContext(Budget budget, int firstYearOfAnalysisPeriod)
    {
        Budget = budget ?? throw new ArgumentNullException(nameof(budget));
        FirstYearOfAnalysisPeriod = firstYearOfAnalysisPeriod;

        if (Budget.InvestmentPlan.AllowFundingCarryover)
        {
            var cumulativeAmount = 0m;
            CumulativeAmountPerYear = Budget.YearlyAmounts.Select(amount => cumulativeAmount += amount.Value).ToArray();
        }
        else
        {
            CumulativeAmountPerYear = Budget.YearlyAmounts.Select(amount => amount.Value).ToArray();
        }
    }

    public Budget Budget { get; }

    public decimal CurrentAmount => CumulativeAmountPerYear[CurrentYearIndex];

    public decimal? CurrentPrioritizedAmount { get; private set; }

    public void AllocateCost(decimal cost) => _AllocateCost(cost, CurrentYearIndex);

    public void AllocateCost(decimal cost, int targetYear) => _AllocateCost(cost, targetYear - FirstYearOfAnalysisPeriod);

    public void MoveToNextYear() => SetYearIndex(CurrentYearIndex + 1);

    public void SetPriority(BudgetPriority budgetPriority)
    {
        var prioritizedFraction = budgetPriority?.GetBudgetPercentagePair(Budget).Percentage / 100;
        CurrentPrioritizedAmount = CurrentAmount * prioritizedFraction;
    }

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

    private void _AllocateCost(decimal cost, int targetYearIndex)
    {
        CurrentPrioritizedAmount -= cost;
        CumulativeAmountPerYear[targetYearIndex] -= cost;

        if (Budget.InvestmentPlan.AllowFundingCarryover)
        {
            for (var yearIndex = targetYearIndex + 1; yearIndex < CumulativeAmountPerYear.Length; ++yearIndex)
            {
                CumulativeAmountPerYear[yearIndex] -= cost;
            }

            for (var yearIndex = targetYearIndex; yearIndex > 0; --yearIndex)
            {
                var previousYearIndex = yearIndex - 1;
                CumulativeAmountPerYear[previousYearIndex] = Math.Min(
                    CumulativeAmountPerYear[previousYearIndex],
                    CumulativeAmountPerYear[yearIndex]);
            }
        }
    }

    private void SetYearIndex(int yearIndex)
    {
        CurrentYearIndex = yearIndex;
        SetPriority(null);
    }
}
