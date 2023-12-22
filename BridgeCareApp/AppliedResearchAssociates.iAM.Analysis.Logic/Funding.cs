using Google.OrTools.LinearSolver;

namespace AppliedResearchAssociates.iAM.Analysis.Logic;

public static class Funding
{
    public sealed record Settings(
        bool BudgetCarryoverIsAllowed = false,
        bool MultipleBudgetsCanFundEachTreatment = false);

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

        for (var t = 0; t < numberOfTreatments; ++t)
        {
            var fundingIsPossible = false;
            for (var b = 0; b < numberOfBudgets; ++b)
            {
                if (allocationIsAllowedPerBudgetAndTreatment[b, t])
                {
                    fundingIsPossible = true;
                    break;
                }
            }

            if (!fundingIsPossible)
            {
                throw new ArgumentException(
                    $"Treatment [{t}] funding is not permitted.",
                    nameof(allocationIsAllowedPerBudgetAndTreatment));
            }
        }

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

        for (var t = 0; t < costPerTreatment.Length; ++t)
        {
            var cost = costPerTreatment[t];
            if (cost <= 0)
            {
                throw new ArgumentException(
                    $"Treatment [{t}] cost [{cost}] is non-positive.",
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

        if (amountPerBudgetPerYear.Select(Enumerable.Sum).Sum() < costPerTreatment.Sum())
        {
            // Trivially unsolvable.
            AssignEmptyArray(out allocationPerBudgetAndTreatmentPerYear);
            return false;
        }

        // Optimization

        Allocate(out allocationPerBudgetAndTreatmentPerYear, numberOfYears);
        for (var y = 0; y < allocationPerBudgetAndTreatmentPerYear.Length; ++y)
        {
            Allocate(out allocationPerBudgetAndTreatmentPerYear[y], numberOfBudgets, numberOfTreatments);
        }

        if (numberOfBudgets == 1)
        {
            // Degenerate case.

            for (var y = 0; y < numberOfYears; ++y)
            {
                var amountRemaining = amountPerBudget[0];

                for (var t = 0; t < costPerTreatment.Length; ++t)
                {
                    var cost = costPerTreatment[t];

                    if (!allocationIsAllowedPerBudgetAndTreatment[0, t] || cost > amountRemaining)
                    {
                        return false;
                    }

                    amountRemaining -= cost;

                    allocationPerBudgetAndTreatment[0, t] = cost.RoundToCent();
                }
            }
        }
        else if (numberOfTreatments == 1 && !settings.BudgetCarryoverIsAllowed)
        {
            // Less degenerate case.

            var costRemaining = costPerTreatment[0];

            for (var b = 0; b < amountPerBudget.Length; ++b)
            {
                if (allocationIsAllowedPerBudgetAndTreatment[b, 0])
                {
                    var amountAvailable = amountPerBudget[b];
                    if (costRemaining <= amountAvailable)
                    {
                        allocationPerBudgetAndTreatment[b, 0] = costRemaining.RoundToCent();
                        costRemaining = 0;
                    }
                    else if (settings.MultipleBudgetsCanFundEachTreatment)
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
        else if (settings.MultipleBudgetsCanFundEachTreatment)
        {
            // Use LP with GLOP. Build the LP.

            using var solver = Solver.CreateSolver("GLOP") ??
                throw new Exception("GLOP solver could not be created.");

            var allocationVariablesMatrix = new Variable[numberOfYears, numberOfBudgets, numberOfTreatments];
            var allocationVariablesPerBudget = new List<Variable>[numberOfBudgets];
            var allocationVariablesVector = new MPVariableVector(allocationVariablesMatrix.Length);

            var spendingConstraintPerYearAndBudget = new Constraint[numberOfYears, numberOfBudgets];
            var fundingConstraintPerYearAndTreatment = new Constraint[numberOfYears, numberOfTreatments];

            var totalSpendingConstraintPerBudget = new Constraint[numberOfBudgets];

            // Create budget total spending constraints.
            for (var b = 0; b < numberOfBudgets; ++b)
            {
                allocationVariablesPerBudget[b] = new(numberOfYears * numberOfTreatments);

                var totalAmount = TotalAmountPerBudget(amountPerBudgetPerYear, b);

                var totalSpendingConstraint = solver.MakeConstraint($"totalSpending[{b}]");
                totalSpendingConstraintPerBudget[b] = totalSpendingConstraint;

                totalSpendingConstraint.SetLb(0);
                totalSpendingConstraint.SetUb((double)totalAmount);
            }

            // Create yearly constraints and variables.
            for (var y = 0; y < numberOfYears; ++y)
            {
                var amountPerBudget = amountPerBudgetPerYear[y];
                var costFraction = costPercentagePerYear[y] / 100;

                // Create budget spending constraints.
                for (var b = 0; b < numberOfBudgets; ++b)
                {
                    var spendingConstraint = solver.MakeConstraint($"spending[{y},{b}]");
                    spendingConstraintPerYearAndBudget[y, b] = spendingConstraint;

                    var amount = amountPerBudget[b];

                    if (settings.BudgetCarryoverIsAllowed)
                    {
                        // Incorporate each previous year's budget amount and allocation variables.
                        for (var y0 = 0; y0 < y; ++y0)
                        {
                            amount += amountPerBudgetPerYear[y0][b];

                            for (var t = 0; t < numberOfTreatments; ++t)
                            {
                                if (allocationVariablesMatrix[y0, b, t] is Variable allocationVariable)
                                {
                                    spendingConstraint.SetCoefficient(allocationVariable, 1);
                                }
                            }
                        }
                    }

                    spendingConstraint.SetLb(0);
                    spendingConstraint.SetUb((double)amount);
                }

                // Create treatment funding constraints.
                for (var t = 0; t < numberOfTreatments; ++t)
                {
                    var fundingConstraint = solver.MakeConstraint($"funding[{y},{t}]");
                    fundingConstraintPerYearAndTreatment[y, t] = fundingConstraint;

                    var cost = costPerTreatment[t] * costFraction;
                    const decimal fundingTolerance = 0.001m; // 1/10th of 1 cent

                    fundingConstraint.SetLb((double)(cost - fundingTolerance));
                    fundingConstraint.SetUb((double)(cost + fundingTolerance));
                }

                // Create allocation variables.
                for (var b = 0; b < numberOfBudgets; ++b)
                {
                    for (var t = 0; t < numberOfTreatments; ++t)
                    {
                        if (allocationIsAllowedPerBudgetAndTreatment[b, t])
                        {
                            var spendingConstraint = spendingConstraintPerYearAndBudget[y, b];
                            var fundingConstraint = fundingConstraintPerYearAndTreatment[y, t];

                            var amount = spendingConstraint.Ub();
                            var cost = fundingConstraint.Ub();

                            var maximumAllocation = Math.Min(amount, cost);
                            var allocationVariable = solver.MakeNumVar(
                                0,
                                maximumAllocation,
                                $"allocation[{y},{b},{t}]");

                            allocationVariablesMatrix[y, b, t] = allocationVariable;
                            allocationVariablesPerBudget[b].Add(allocationVariable);
                            allocationVariablesVector.Add(allocationVariable);

                            spendingConstraint.SetCoefficient(allocationVariable, 1);
                            fundingConstraint.SetCoefficient(allocationVariable, 1);

                            totalSpendingConstraintPerBudget[b].SetCoefficient(allocationVariable, 1);
                        }
                    }
                }
            }

            // Use the solver. In budget order, maximize each budget's total spending.

            var allocationValues = new double[allocationVariablesVector.Count];

            using var objective = solver.Objective();

            for (var b = 0; b < numberOfBudgets - 1; ++b)
            {
                if (b > 0)
                {
                    // Copy all variable values from the previous iteration's solution.
                    for (var i = 0; i < allocationValues.Length; ++i)
                    {
                        allocationValues[i] = allocationVariablesVector[i].SolutionValue();
                    }

                    // This is what was maximized on the previous iteration, for the budget before
                    // the current one. We want to maintain this maximized value, so we will update
                    // the previous budget's spending constraint.
                    var maximizedSpending = objective.Value();

                    // This constraint update also resets all variable values. (Probably an intended
                    // side-effect, because changing a constraint bound can potentially change the
                    // solution space.)
                    totalSpendingConstraintPerBudget[b - 1].SetLb(maximizedSpending);

                    // Un-reset all variable values to continue from the previous solution.
                    solver.SetHint(allocationVariablesVector, allocationValues);
                }

                objective.Clear();
                objective.SetMaximization();
                foreach (var allocationVariable in allocationVariablesPerBudget[b])
                {
                    objective.SetCoefficient(allocationVariable, 1);
                }

                var resultStatus = solver.Solve();
                if (resultStatus is not Solver.ResultStatus.OPTIMAL or Solver.ResultStatus.FEASIBLE)
                {
                    return false;
                }
            }

            // Prepare the solution.

            for (var y = 0; y < numberOfYears; ++y)
            {
                var allocationPerBudgetAndTreatment = allocationPerBudgetAndTreatmentPerYear[y];

                for (var b = 0; b < numberOfBudgets; ++b)
                {
                    for (var t = 0; t < numberOfTreatments; ++t)
                    {
                        if (allocationVariablesMatrix[y, b, t] is Variable allocationVariable)
                        {
                            var allocation = (decimal)allocationVariable.SolutionValue();
                            allocationPerBudgetAndTreatment[b, t] = allocation.RoundToCent();
                        }
                    }
                }
            }

            // Manually dispose collections of disposables.

            foreach (var v in allocationVariablesVector)
            {
                v.Dispose();
            }

            foreach (var c in spendingConstraintPerYearAndBudget)
            {
                c.Dispose();
            }

            foreach (var c in fundingConstraintPerYearAndTreatment)
            {
                c.Dispose();
            }

            foreach (var c in totalSpendingConstraintPerBudget)
            {
                c.Dispose();
            }
        }
        else
        {
            // Use MIP with CP-SAT. Build the MIP.

            // update allocationIsAllowed by disallowing where amount_b < cost_t. (don't un-disallow anything!)

            throw new NotImplementedException("MIP for single-budget funding is still WIP.");
        }

        // Output validation

        for (var b = 0; b < numberOfBudgets; ++b)
        {
            var spending = TotalSpendingPerBudget(allocationPerBudgetAndTreatmentPerYear, b).RoundToCent();
            var amount = TotalAmountPerBudget(amountPerBudgetPerYear, b).RoundToCent();

            if (spending > amount)
            {
                throw new Exception($"Budget [{b}] total spending [{spending}] exceeds total amount [{amount}].");
            }
        }

        for (var t = 0; t < numberOfTreatments; ++t)
        {
            var funding = TotalFundingPerTreatment(allocationPerBudgetAndTreatmentPerYear, t).RoundToCent();
            var cost = costPerTreatment[t].RoundToCent();

            if (funding != cost)
            {
                throw new Exception($"Treatment [{t}] total funding [{funding}] does not match total cost [{cost}].");
            }
        }

        return true;
    }

    private static void Allocate<T>(out T[] array, int length) => array = new T[length];

    private static void Allocate<T>(out T[,] array, int length0, int length1) => array = new T[length0, length1];

    private static int ArrayLength<T>(T[] array) => array.Length;

    private static void AssignEmptyArray<T>(out T[] array) => array = Array.Empty<T>();

    private static bool IsNull<T>(T value) => value is null;

    private static decimal RoundToCent(this decimal value) => Math.Round(value, 2);

    private static decimal TotalAmountPerBudget(decimal[][] amountPerBudgetPerYear, int b)
    {
        var totalAmount = 0m;

        for (var y = 0; y < amountPerBudgetPerYear.Length; ++y)
        {
            totalAmount += amountPerBudgetPerYear[y][b];
        }

        return totalAmount;
    }

    private static decimal TotalFundingPerTreatment(decimal?[,] allocationPerBudgetAndTreatment, int t)
    {
        var totalFunding = 0m;

        for (var b = 0; b < allocationPerBudgetAndTreatment.GetLength(0); ++b)
        {
            totalFunding += allocationPerBudgetAndTreatment[b, t] ?? 0;
        }

        return totalFunding;
    }

    private static decimal TotalFundingPerTreatment(decimal?[][,] allocationPerBudgetAndTreatmentPerYear, int t)
    {
        var totalFunding = 0m;

        foreach (var allocationPerBudgetAndTreatment in allocationPerBudgetAndTreatmentPerYear)
        {
            totalFunding += TotalFundingPerTreatment(allocationPerBudgetAndTreatment, t);
        }

        return totalFunding;
    }

    private static decimal TotalSpendingPerBudget(decimal?[,] allocationPerBudgetAndTreatment, int b)
    {
        var totalSpending = 0m;

        for (var t = 0; t < allocationPerBudgetAndTreatment.GetLength(1); ++t)
        {
            totalSpending += allocationPerBudgetAndTreatment[b, t] ?? 0;
        }

        return totalSpending;
    }

    private static decimal TotalSpendingPerBudget(decimal?[][,] allocationPerBudgetAndTreatmentPerYear, int b)
    {
        var totalSpending = 0m;

        foreach (var allocationPerBudgetAndTreatment in allocationPerBudgetAndTreatmentPerYear)
        {
            totalSpending += TotalFundingPerTreatment(allocationPerBudgetAndTreatment, b);
        }

        return totalSpending;
    }
}
