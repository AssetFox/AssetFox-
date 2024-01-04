namespace AppliedResearchAssociates.iAM.Analysis.Logic;

internal sealed partial record FundingSolver(
    bool[,] AllocationIsAllowedPerBudgetAndTreatment,
    List<decimal[]> AmountPerBudgetPerYear,
    decimal[] CostPerTreatment,
    decimal[] CostPercentagePerYear,
    Funding.Settings Settings,
    List<decimal?[,]> AllocationPerBudgetAndTreatmentPerYear,
    int NumberOfYears,
    int NumberOfBudgets,
    int NumberOfTreatments)
{
    public bool TrySolve() =>
        NumberOfBudgets == 1
        ? SingleBudget() :
        NumberOfTreatments == 1 && (NumberOfYears == 1 || !Settings.BudgetCarryoverIsAllowed)
        ? SingleTreatmentWithoutCarryover() :
        Settings.MultipleBudgetsCanFundEachTreatment
        ? LP() : CP();

    private static decimal TotalAmountPerBudget(IEnumerable<decimal[]> amountPerBudgetPerYear, int b)
    {
        var totalAmount = 0m;

        foreach (var amountPerBudget in amountPerBudgetPerYear)
        {
            totalAmount += amountPerBudget[b];
        }

        return totalAmount;
    }

    private bool SingleBudget()
    {
        // Degenerate case.

        var amountRemaining = 0m;

        for (var y = 0; y < NumberOfYears; ++y)
        {
            var amountPerBudget = AmountPerBudgetPerYear[y];
            var allocationPerBudgetAndTreatment = AllocationPerBudgetAndTreatmentPerYear[y];
            var costFraction = CostPercentagePerYear[y] / 100;

            if (Settings.BudgetCarryoverIsAllowed)
            {
                amountRemaining += amountPerBudget[0];
            }
            else
            {
                amountRemaining = amountPerBudget[0];
            }

            for (var t = 0; t < CostPerTreatment.Length; ++t)
            {
                var cost = CostPerTreatment[t] * costFraction;

                if (!AllocationIsAllowedPerBudgetAndTreatment[0, t] ||
                    cost > amountRemaining && !Settings.UnlimitedSpending)
                {
                    return false;
                }

                amountRemaining -= cost;

                allocationPerBudgetAndTreatment[0, t] = cost.RoundToCent();
            }
        }

        return true;
    }

    private bool SingleTreatmentWithoutCarryover()
    {
        // Less degenerate case.

        for (var y = 0; y < NumberOfYears; ++y)
        {
            var amountPerBudget = AmountPerBudgetPerYear[y];
            var allocationPerBudgetAndTreatment = AllocationPerBudgetAndTreatmentPerYear[y];
            var costFraction = CostPercentagePerYear[y] / 100;

            var costRemaining = CostPerTreatment[0] * costFraction;

            for (var b = 0; b < amountPerBudget.Length; ++b)
            {
                if (AllocationIsAllowedPerBudgetAndTreatment[b, 0])
                {
                    var amountAvailable = amountPerBudget[b];
                    if (costRemaining <= amountAvailable || Settings.UnlimitedSpending)
                    {
                        allocationPerBudgetAndTreatment[b, 0] = costRemaining.RoundToCent();
                        costRemaining = 0;
                    }
                    else if (Settings.MultipleBudgetsCanFundEachTreatment)
                    {
                        allocationPerBudgetAndTreatment[b, 0] = amountAvailable.RoundToCent();
                        costRemaining -= amountAvailable;
                    }
                }
            }

            if (costRemaining != 0)
            {
                return false;
            }
        }

        return true;
    }
}
