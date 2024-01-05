using Google.OrTools.LinearSolver;

namespace AppliedResearchAssociates.iAM.Analysis.Logic;

public static partial class Funding
{
    public sealed record Settings(
        bool BudgetCarryoverIsAllowed = false,
        bool MultipleBudgetsCanFundEachTreatment = false,
        bool UnlimitedSpending = false);

    private static readonly decimal?[,] EmptyMatrix = { };

    private static readonly decimal[] SingleYearCostPercentage = { 100m };

    public static bool TrySolve(
        bool[,] allocationIsAllowedPerBudgetAndTreatment,
        decimal[] amountPerBudget,
        decimal[] costPerTreatment,
        Settings settings,
        out decimal?[,] allocationPerBudgetAndTreatment)
    {
        var solved = TrySolve(
            allocationIsAllowedPerBudgetAndTreatment,
            new[] { amountPerBudget },
            costPerTreatment,
            SingleYearCostPercentage,
            settings,
            out var allocationPerBudgetAndTreatmentPerYear);

        allocationPerBudgetAndTreatment = solved
            ? allocationPerBudgetAndTreatmentPerYear[0]
            : EmptyMatrix;

        return solved;
    }

    public static bool TrySolve(
        bool[,] allocationIsAllowedPerBudgetAndTreatment,
        decimal[][] amountPerBudgetPerYear,
        decimal[] costPerTreatment,
        decimal[] costPercentagePerYear,
        Settings settings,
        out decimal?[][,] allocationPerBudgetAndTreatmentPerYear)
    {
        // Input validation

        if (allocationIsAllowedPerBudgetAndTreatment is null)
        {
            throw new ArgumentNullException(nameof(allocationIsAllowedPerBudgetAndTreatment));
        }

        if (amountPerBudgetPerYear is null)
        {
            throw new ArgumentNullException(nameof(amountPerBudgetPerYear));
        }

        if (costPerTreatment is null)
        {
            throw new ArgumentNullException(nameof(costPerTreatment));
        }

        if (costPercentagePerYear is null)
        {
            throw new ArgumentNullException(nameof(costPercentagePerYear));
        }

        if (settings is null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        if (amountPerBudgetPerYear.Length != costPercentagePerYear.Length)
        {
            throw new ArgumentException("Inconsistent input sizes (number of years).");
        }

        var numberOfYears = amountPerBudgetPerYear.Length;

        if (numberOfYears == 0)
        {
            throw new ArgumentException("Zero years of input.");
        }

        if (amountPerBudgetPerYear.Any(IsNull))
        {
            throw new ArgumentException(
                "Incomplete input (budgets per year).",
                nameof(amountPerBudgetPerYear));
        }

        if (amountPerBudgetPerYear.Select(ArrayLength).Distinct().Count() != 1)
        {
            throw new ArgumentException(
                "Inconsistent input sizes (number of budgets per year).",
                nameof(amountPerBudgetPerYear));
        }

        var numberOfBudgets = amountPerBudgetPerYear[0].Length;

        if (numberOfBudgets == 0)
        {
            throw new ArgumentException(
                "No budget amounts.",
                nameof(amountPerBudgetPerYear));
        }

        var numberOfTreatments = costPerTreatment.Length;

        if (numberOfTreatments == 0)
        {
            throw new ArgumentException(
                "No treatment costs.",
                nameof(costPerTreatment));
        }

        if (allocationIsAllowedPerBudgetAndTreatment.GetLength(0) != numberOfBudgets ||
            allocationIsAllowedPerBudgetAndTreatment.GetLength(1) != numberOfTreatments)
        {
            throw new ArgumentException(
                "Inconsistent input sizes (budget-to-treatment allocation permissions matrix).",
                nameof(allocationIsAllowedPerBudgetAndTreatment));
        }

        if (!settings.UnlimitedSpending)
        {
            for (var y = 0; y < amountPerBudgetPerYear.Length; ++y)
            {
                var amountPerBudget = amountPerBudgetPerYear[y];
                for (var b = 0; b < amountPerBudget.Length; ++b)
                {
                    var amount = amountPerBudget[b];
                    if (amount < 0)
                    {
                        throw new ArgumentException(
                            $"Year [{y}] budget [{b}] amount [{amount}] is negative.",
                            nameof(amountPerBudgetPerYear));
                    }
                }
            }
        }

        for (var t = 0; t < costPerTreatment.Length; ++t)
        {
            var cost = costPerTreatment[t];
            if (cost < 0)
            {
                throw new ArgumentException(
                    $"Treatment [{t}] cost [{cost}] is negative.",
                    nameof(costPerTreatment));
            }
        }

        var costPercentagesSum = 0m;
        for (var y = 0; y < costPercentagePerYear.Length; ++y)
        {
            var costPercentage = costPercentagePerYear[y];
            if (costPercentage < 0)
            {
                throw new ArgumentException(
                    $"Year [{y}] cost percentage [{costPercentage}] is negative.",
                    nameof(costPercentagePerYear));
            }

            costPercentagesSum += costPercentage;
        }

        if (costPercentagesSum != 100)
        {
            throw new ArgumentException(
                $"Cost percentages sum [{costPercentagesSum}] does not equal 100.",
                nameof(costPercentagePerYear));
        }

        if (!settings.UnlimitedSpending && amountPerBudgetPerYear.Select(Enumerable.Sum).Sum() < costPerTreatment.Sum())
        {
            // Trivially unsolvable.
            allocationPerBudgetAndTreatmentPerYear = Array.Empty<decimal?[,]>();
            return false;
        }

        // Optimization

        allocationPerBudgetAndTreatmentPerYear = new decimal?[numberOfYears][,];
        for (var y = 0; y < numberOfYears; ++y)
        {
            allocationPerBudgetAndTreatmentPerYear[y] = new decimal?[numberOfBudgets, numberOfTreatments];
        }

        var solver = new FundingSolver(
            allocationIsAllowedPerBudgetAndTreatment,
            amountPerBudgetPerYear,
            costPerTreatment,
            costPercentagePerYear,
            settings,
            allocationPerBudgetAndTreatmentPerYear,
            numberOfYears,
            numberOfBudgets,
            numberOfTreatments);

        return solver.TrySolve();
    }

    private static int ArrayLength<T>(T[] array) => array.Length;

    private static bool IsNull<T>(T value) => value is null;
}
