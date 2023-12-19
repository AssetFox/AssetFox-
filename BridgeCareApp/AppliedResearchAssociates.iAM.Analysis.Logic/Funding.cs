using Google.OrTools.LinearSolver;

namespace AppliedResearchAssociates.iAM.Analysis.Logic;

public static class Funding
{
    public sealed record Settings(
        bool BudgetCarryoverIsAllowed = false,
        bool CashFlowEnforcesFirstYearFundingFractions = false,
        bool MultipleBudgetsCanFundEachTreatment = false);

    private static readonly decimal?[,] EmptyAllocationMatrix = { };

    private delegate void UpdateBudgetAmount(ref decimal amount, decimal update);

    /// <summary>
    ///     Try to fund one or more treatments via multi-year cash-flow.
    /// </summary>
    /// <returns>
    ///     null if funding was successful; otherwise, the cash-flow year (as a zero-based index) in
    ///     which funding failed
    /// </returns>
    public static int? TryCashFlow(
        decimal[] cashFlowPercentagePerYear,
        bool[,] allocationIsAllowedPerBudgetAndTreatment,
        decimal[][] amountPerBudgetPerYear,
        decimal[] costPerTreatment,
        Settings settings,
        out decimal?[][,] allocationPerBudgetAndTreatmentPerYear)
    {
        if (cashFlowPercentagePerYear is null)
        {
            throw new ArgumentNullException(nameof(cashFlowPercentagePerYear));
        }

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

        if (settings is null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        if (cashFlowPercentagePerYear.Length != amountPerBudgetPerYear.Length)
        {
            throw new ArgumentException("Inconsistent input sizes.");
        }

        if (cashFlowPercentagePerYear.Length < 2)
        {
            throw new ArgumentException("Less than two years of cash flow.");
        }

        if (cashFlowPercentagePerYear.Any(IsNegative) || cashFlowPercentagePerYear.Sum() != 100)
        {
            throw new ArgumentException("Invalid cash flow percentages.", nameof(cashFlowPercentagePerYear));
        }

        if (amountPerBudgetPerYear.Any(IsNull))
        {
            throw new ArgumentException("Incomplete input.", nameof(amountPerBudgetPerYear));
        }

        if (amountPerBudgetPerYear.Select(ArrayLength).Distinct().Count() != 1)
        {
            throw new ArgumentException("Inconsistent input sizes.", nameof(amountPerBudgetPerYear));
        }

        Allocate(out allocationPerBudgetAndTreatmentPerYear, cashFlowPercentagePerYear.Length);

        Allocate(out decimal[] workingAmountPerBudget, amountPerBudgetPerYear[0].Length);
        Allocate(out decimal[] workingCostPerTreatment, costPerTreatment.Length);

        UpdateBudgetAmount updateBudgetAmount = settings.BudgetCarryoverIsAllowed ? Increment : Assign;

        for (var year = 0; year < cashFlowPercentagePerYear.Length; ++year)
        {
            var amountPerBudget = amountPerBudgetPerYear[year];

            for (var b = 0; b < workingAmountPerBudget.Length; ++b)
            {
                updateBudgetAmount(ref workingAmountPerBudget[b], amountPerBudget[b]);
            }

            var cashFlowFraction = cashFlowPercentagePerYear[year] / 100;

            for (var t = 0; t < workingCostPerTreatment.Length; ++t)
            {
                workingCostPerTreatment[t] = costPerTreatment[t] * cashFlowFraction;
            }

            ref var allocationPerBudgetAndTreatment = ref allocationPerBudgetAndTreatmentPerYear[year];

            var solved = TrySolve(
                allocationIsAllowedPerBudgetAndTreatment,
                workingAmountPerBudget,
                workingCostPerTreatment,
                settings,
                out allocationPerBudgetAndTreatment);

            if (!solved)
            {
                return year;
            }

            for (var b = 0; b < workingAmountPerBudget.Length; ++b)
            {
                var totalSpending = TotalSpendingPerBudget(allocationPerBudgetAndTreatment, b);
                workingAmountPerBudget[b] -= totalSpending;
            }

            if (settings.CashFlowEnforcesFirstYearFundingFractions)
            {
                // to-do: deal with first-year fractions

                break;
            }
        }

        for (var t = 0; t < costPerTreatment.Length; ++t)
        {
            var funding = TotalFundingPerTreatment(allocationPerBudgetAndTreatmentPerYear, t).RoundToCent();
            var cost = costPerTreatment[t].RoundToCent();

            if (funding != cost)
            {
                throw new Exception($"Treatment funding [{funding}] does not match cost [{cost}].");
            }
        }

        return null;
    }

    public static bool TrySolve(
        bool[,] allocationIsAllowedPerBudgetAndTreatment,
        decimal[] amountPerBudget,
        decimal[] costPerTreatment,
        Settings settings,
        out decimal?[,] allocationPerBudgetAndTreatment)
    {
        // Pre-validation

        if (allocationIsAllowedPerBudgetAndTreatment is null)
        {
            throw new ArgumentNullException(nameof(allocationIsAllowedPerBudgetAndTreatment));
        }

        if (amountPerBudget is null)
        {
            throw new ArgumentNullException(nameof(amountPerBudget));
        }

        if (costPerTreatment is null)
        {
            throw new ArgumentNullException(nameof(costPerTreatment));
        }

        if (settings is null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        if (amountPerBudget.Length == 0)
        {
            throw new ArgumentException("No budget amounts.", nameof(amountPerBudget));
        }

        if (costPerTreatment.Length == 0)
        {
            throw new ArgumentException("No treatment costs.", nameof(costPerTreatment));
        }

        if (allocationIsAllowedPerBudgetAndTreatment.GetLength(0) != amountPerBudget.Length ||
            allocationIsAllowedPerBudgetAndTreatment.GetLength(1) != costPerTreatment.Length)
        {
            throw new ArgumentException("Inconsistent input sizes.");
        }

        for (var b = 0; b < amountPerBudget.Length; ++b)
        {
            var amount = amountPerBudget[b];
            if (amount <= 0)
            {
                throw new ArgumentException($"Budget [{b}] amount [{amount}] is non-positive.");
            }
        }

        for (var t = 0; t < costPerTreatment.Length; ++t)
        {
            var cost = costPerTreatment[t];
            if (cost <= 0)
            {
                throw new ArgumentException($"Treatment [{t}] cost [{cost}] is non-positive.");
            }
        }

        if (amountPerBudget.Sum().RoundToCent() < costPerTreatment.Sum().RoundToCent())
        {
            // Trivially unsolvable.
            allocationPerBudgetAndTreatment = EmptyAllocationMatrix;
            return false;
        }

        // Optimization

        allocationPerBudgetAndTreatment = new decimal?[amountPerBudget.Length, costPerTreatment.Length];

        if (amountPerBudget.Length == 1)
        {
            // Degenerate case.

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
        else if (costPerTreatment.Length == 1)
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
                throw new Exception("Solver could not be created.");

            var allocationVariablesMatrix = new Variable[amountPerBudget.Length, costPerTreatment.Length];
            var allocationVariablesPerBudget = new List<Variable>[amountPerBudget.Length];
            var allocationVariablesVector = new MPVariableVector(allocationVariablesMatrix.Length);

            var spendingConstraints = new Constraint[amountPerBudget.Length];
            var fundingConstraints = new Constraint[costPerTreatment.Length];

            for (var t = 0; t < costPerTreatment.Length; ++t)
            {
                const decimal fundingTolerance = 0.001m; // 1/10th of 1 cent
                fundingConstraints[t] = solver.MakeConstraint(
                    (double)(costPerTreatment[t] - fundingTolerance),
                    (double)(costPerTreatment[t] + fundingTolerance),
                    $"funding[{t}]");
            }

            for (var b = 0; b < amountPerBudget.Length; ++b)
            {
                spendingConstraints[b] = solver.MakeConstraint(0, (double)amountPerBudget[b], $"spending[{b}]");
                allocationVariablesPerBudget[b] = new(costPerTreatment.Length);

                for (var t = 0; t < costPerTreatment.Length; ++t)
                {
                    if (allocationIsAllowedPerBudgetAndTreatment[b, t])
                    {
                        var maximumAllocation = Math.Min(amountPerBudget[b], costPerTreatment[t]);
                        var allocationVariable = solver.MakeNumVar(0, (double)maximumAllocation, $"allocation[{b},{t}]");

                        allocationVariablesMatrix[b, t] = allocationVariable;
                        allocationVariablesPerBudget[b].Add(allocationVariable);
                        allocationVariablesVector.Add(allocationVariable);

                        spendingConstraints[b].SetCoefficient(allocationVariable, 1);
                        fundingConstraints[t].SetCoefficient(allocationVariable, 1);
                    }
                }
            }

            // Use the solver. In budget order, maximize each budget's total spending.

            var allocationValues = new double[allocationVariablesVector.Count];

            using var objective = solver.Objective();

            for (var b = 0; b < amountPerBudget.Length - 1; ++b)
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

                    // This constraint update also resets all variable values. (Probably a
                    // deliberate side-effect, because changing a constraint bound can potentially
                    // change the solution space.)
                    spendingConstraints[b - 1].SetLb(maximizedSpending);

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

            for (var b = 0; b < amountPerBudget.Length; ++b)
            {
                for (var t = 0; t < costPerTreatment.Length; ++t)
                {
                    if (allocationVariablesMatrix[b, t] is Variable allocationVariable)
                    {
                        var allocation = (decimal)allocationVariable.SolutionValue();
                        allocationPerBudgetAndTreatment[b, t] = allocation.RoundToCent();
                    }
                }
            }

            // Manually dispose collections of disposables.

            foreach (var v in allocationVariablesVector)
            {
                v.Dispose();
            }

            foreach (var c in spendingConstraints)
            {
                c.Dispose();
            }

            foreach (var c in fundingConstraints)
            {
                c.Dispose();
            }
        }
        else
        {
            // Use MIP with CP-SAT. Build the MIP.

            // update allowedSpending by disallowing where amount_b < cost_t. (don't un-disallow anything!)

            throw new NotImplementedException();
        }

        // Post-validation

        for (var b = 0; b < amountPerBudget.Length; ++b)
        {
            var spending = TotalSpendingPerBudget(allocationPerBudgetAndTreatment, b).RoundToCent();
            var amount = amountPerBudget[b].RoundToCent();

            if (spending > amount)
            {
                throw new Exception($"Budget spending [{spending}] exceeds amount [{amount}].");
            }
        }

        for (var t = 0; t < costPerTreatment.Length; ++t)
        {
            var funding = TotalFundingPerTreatment(allocationPerBudgetAndTreatment, t).RoundToCent();
            var cost = costPerTreatment[t].RoundToCent();

            if (funding != cost)
            {
                throw new Exception($"Treatment funding [{funding}] does not match cost [{cost}].");
            }
        }

        return true;
    }

    private static void Allocate<T>(out T[] array, int length) => array = new T[length];

    private static int ArrayLength<T>(T[] array) => array.Length;

    private static void Assign(ref decimal variable, decimal value) => variable = value;

    private static void Increment(ref decimal variable, decimal value) => variable += value;

    private static bool IsNegative(decimal value) => value < 0;

    private static bool IsNull<T>(T value) => value is null;

    private static decimal RoundToCent(this decimal value) => Math.Round(value, 2);

    private static decimal TotalFundingPerTreatment(decimal?[,] allocationPerBudgetAndTreatment, int t)
    {
        decimal? totalFunding = 0;

        for (var b = 0; b < allocationPerBudgetAndTreatment.GetLength(0); ++b)
        {
            totalFunding += allocationPerBudgetAndTreatment[b, t] ?? 0;
        }

        return totalFunding.Value;
    }

    private static decimal TotalFundingPerTreatment(decimal?[][,] allocationPerBudgetAndTreatmentPerYear, int t)
    {
        decimal? totalFunding = 0;

        foreach (var allocationPerBudgetAndTreatment in allocationPerBudgetAndTreatmentPerYear)
        {
            totalFunding += TotalFundingPerTreatment(allocationPerBudgetAndTreatment, t);
        }

        return totalFunding.Value;
    }

    private static decimal TotalSpendingPerBudget(decimal?[,] allocationPerBudgetAndTreatment, int b)
    {
        decimal? totalSpending = 0;

        for (var t = 0; t < allocationPerBudgetAndTreatment.GetLength(1); ++t)
        {
            totalSpending += allocationPerBudgetAndTreatment[b, t] ?? 0;
        }

        return totalSpending.Value;
    }
}
