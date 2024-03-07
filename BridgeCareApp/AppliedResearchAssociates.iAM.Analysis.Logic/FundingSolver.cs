namespace AppliedResearchAssociates.iAM.Analysis.Logic;

internal sealed partial record FundingSolver(
    bool[,] AllocationIsAllowedPerBudgetAndTreatment,
    decimal[][] AmountPerBudgetPerYear,
    decimal[] CostPerTreatment,
    decimal[] CostPercentagePerYear,
    Funding.Settings Settings,
    decimal?[][,] AllocationPerBudgetAndTreatmentPerYear,
    int NumberOfYears,
    int NumberOfBudgets,
    int NumberOfTreatments)
{
    public bool TrySolve() =>
        Settings.UnlimitedSpending
        ? UnlimitedSpending() :
        NumberOfBudgets == 1
        ? OneBudget() :
        NumberOfTreatments == 1 && (NumberOfYears == 1 || !Settings.BudgetCarryoverIsAllowed)
        ? OneTreatmentAndNoCarryover() :
        Settings.MultipleBudgetsCanFundEachTreatment
        ? LinearProgram() : ConstraintProgram();

    private static decimal TotalAmountPerBudget(IEnumerable<decimal[]> amountPerBudgetPerYear, int b)
    {
        var totalAmount = 0m;

        foreach (var amountPerBudget in amountPerBudgetPerYear)
        {
            totalAmount += amountPerBudget[b];
        }

        return totalAmount;
    }

    #region Degenerate cases

    private bool UnlimitedSpending()
    {
        for (var y = 0; y < NumberOfYears; ++y)
        {
            var allocationPerBudgetAndTreatment = AllocationPerBudgetAndTreatmentPerYear[y];
            var costFraction = CostPercentagePerYear[y] / 100;

            for (var t = 0; t < NumberOfTreatments; ++t)
            {
                var allocationHasHappened = false;

                for (var b = 0; b < NumberOfBudgets; ++b)
                {
                    if (AllocationIsAllowedPerBudgetAndTreatment[b, t])
                    {
                        if (allocationHasHappened)
                        {
                            allocationPerBudgetAndTreatment[b, t] = 0;
                        }
                        else
                        {
                            var cost = CostPerTreatment[t] * costFraction;
                            allocationPerBudgetAndTreatment[b, t] = cost.RoundToCent();

                            allocationHasHappened = true;
                        }
                    }
                }
            }
        }

        return true;
    }

    private bool OneBudget()
    {
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

                if (!AllocationIsAllowedPerBudgetAndTreatment[0, t] || cost > amountRemaining)
                {
                    return false;
                }

                amountRemaining -= cost;

                allocationPerBudgetAndTreatment[0, t] = cost.RoundToCent();
            }
        }

        return true;
    }

    private bool OneTreatmentAndNoCarryover()
    {
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
                    if (costRemaining <= amountAvailable)
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

    #endregion
}
