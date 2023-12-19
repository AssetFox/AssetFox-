using Google.OrTools.LinearSolver;

namespace AppliedResearchAssociates.iAM.Analysis.Logic;

public static class Funding
{
    public sealed record Settings(
        bool BudgetCarryoverIsAllowed = false,
        bool CashFlowEnforcesFirstYearFundingFractions = false,
        bool MultipleBudgetsCanFundEachTreatment = false);

    private static readonly decimal?[,] EmptyAllocationMatrix = { };

    public static int? TryCashFlow(
        decimal[] cashFlowPercentagePerYear,
        bool[,] allocationIsAllowedPerBudgetAndTreatment,
        decimal[][] amountPerBudgetPerYear,
        decimal[] costPerTreatment,
        Settings settings,
        out decimal?[][,] allocationPerBudgetAndTreatmentPerYear)
    {
        allocationPerBudgetAndTreatmentPerYear = Array.Empty<decimal?[,]>();
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

        if (amountPerBudget.Length == 0)
        {
            throw new ArgumentException("No budget amounts are given.", nameof(amountPerBudget));
        }

        if (costPerTreatment.Length == 0)
        {
            throw new ArgumentException("No treatment costs are given.", nameof(costPerTreatment));
        }

        if (allocationIsAllowedPerBudgetAndTreatment.GetLength(0) != amountPerBudget.Length ||
            allocationIsAllowedPerBudgetAndTreatment.GetLength(1) != costPerTreatment.Length)
        {
            throw new Exception("Inconsistent input sizes.");
        }

        for (var b = 0; b < amountPerBudget.Length; ++b)
        {
            if (amountPerBudget[b] <= 0)
            {
                throw new ArgumentException($"Budget amount [{b}] is non-positive: [{amountPerBudget[b]}]");
            }
        }

        for (var t = 0; t < costPerTreatment.Length; ++t)
        {
            if (costPerTreatment[t] <= 0)
            {
                throw new ArgumentException($"Treatment cost [{t}] is non-positive: [{costPerTreatment[t]}]");
            }
        }

        if (Math.Round(amountPerBudget.Sum(), 2) < Math.Round(costPerTreatment.Sum(), 2))
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

                allocationPerBudgetAndTreatment[0, t] = Math.Round(cost, 2);
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
                        allocationPerBudgetAndTreatment[b, 0] = Math.Round(costRemaining, 2);
                        costRemaining = 0;
                    }
                    else if (settings.MultipleBudgetsCanFundEachTreatment)
                    {
                        allocationPerBudgetAndTreatment[b, 0] = Math.Round(amountAvailable, 2);
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
                    // change the space of feasible solutions.)
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
                        allocationPerBudgetAndTreatment[b, t] = Math.Round((decimal)allocationVariable.SolutionValue(), 2);
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
            var spending = totalBudgetSpending(allocationPerBudgetAndTreatment, b);
            var amount = Math.Round(amountPerBudget[b], 2);

            if (spending > amount)
            {
                throw new Exception($"Budget spending exceeds amount by [{spending - amount}].");
            }
        }

        for (var t = 0; t < costPerTreatment.Length; ++t)
        {
            var funding = totalTreatmentFunding(allocationPerBudgetAndTreatment, t);
            var cost = Math.Round(costPerTreatment[t], 2);

            if (funding < cost)
            {
                throw new Exception($"Treatment cost exceeds funding by [{cost - funding}].");
            }
            else if (funding > cost)
            {
                throw new Exception($"Treatment funding exceeds cost by [{funding - cost}].");
            }
        }

        return true;

        // Local functions

        static decimal totalBudgetSpending(decimal?[,] solution, int b)
        {
            decimal? total = 0;

            for (var t = 0; t < solution.GetLength(1); ++t)
            {
                total += solution[b, t] ?? 0;
            }

            return Math.Round(total.Value, 2);
        }

        static decimal totalTreatmentFunding(decimal?[,] solution, int t)
        {
            decimal? total = 0;

            for (var b = 0; b < solution.GetLength(0); ++b)
            {
                total += solution[b, t] ?? 0;
            }

            return Math.Round(total.Value, 2);
        }
    }
}
