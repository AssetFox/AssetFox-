using System;
using System.Collections.Generic;
using System.Linq;
using Google.OrTools.LinearSolver;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

public static class Funding
{
    public static bool TrySolve(
        bool[,] allocationIsAllowed,
        decimal[] budgetAmounts,
        decimal[] treatmentCosts,
        bool multipleBudgetsCanFundEachTreatment,
        out decimal?[,] solution)
    {
        // Pre-validation

        if (allocationIsAllowed is null)
        {
            throw new ArgumentNullException(nameof(allocationIsAllowed));
        }

        if (budgetAmounts is null)
        {
            throw new ArgumentNullException(nameof(budgetAmounts));
        }

        if (treatmentCosts is null)
        {
            throw new ArgumentNullException(nameof(treatmentCosts));
        }

        if (budgetAmounts.Length == 0)
        {
            throw new ArgumentException("No budget amounts are given.", nameof(budgetAmounts));
        }

        if (treatmentCosts.Length == 0)
        {
            throw new ArgumentException("No treatment costs are given.", nameof(treatmentCosts));
        }

        if (allocationIsAllowed.GetLength(0) != budgetAmounts.Length ||
            allocationIsAllowed.GetLength(1) != treatmentCosts.Length)
        {
            throw new Exception("Inconsistent input sizes.");
        }

        for (var b = 0; b < budgetAmounts.Length; ++b)
        {
            if (budgetAmounts[b] <= 0)
            {
                throw new ArgumentException($"Budget amount [{b}] is non-positive: [{budgetAmounts[b]}]");
            }
        }

        for (var t = 0; t < treatmentCosts.Length; ++t)
        {
            if (treatmentCosts[t] <= 0)
            {
                throw new ArgumentException($"Treatment cost [{t}] is non-positive: [{treatmentCosts[t]}]");
            }
        }

        if (Math.Round(budgetAmounts.Sum(), 2) < Math.Round(treatmentCosts.Sum(), 2))
        {
            // Trivially unsolvable.
            solution = null;
            return false;
        }

        // Optimization

        solution = new decimal?[budgetAmounts.Length, treatmentCosts.Length];

        if (budgetAmounts.Length == 1)
        {
            // Degenerate case.

            var amountRemaining = budgetAmounts[0];

            for (var t = 0; t < treatmentCosts.Length; ++t)
            {
                var cost = treatmentCosts[t];

                if (!allocationIsAllowed[0, t] || cost > amountRemaining)
                {
                    return false;
                }

                amountRemaining -= cost;

                solution[0, t] = Math.Round(cost, 2);
            }
        }
        else if (treatmentCosts.Length == 1)
        {
            // Less degenerate case.

            var costRemaining = treatmentCosts[0];

            for (var b = 0; b < budgetAmounts.Length; ++b)
            {
                if (allocationIsAllowed[b, 0])
                {
                    if (costRemaining <= budgetAmounts[b])
                    {
                        solution[b, 0] = Math.Round(costRemaining, 2);
                        costRemaining = 0;
                    }
                    else if (multipleBudgetsCanFundEachTreatment)
                    {
                        solution[b, 0] = Math.Round(budgetAmounts[b], 2);
                        costRemaining -= budgetAmounts[b];
                    }
                }
            }

            if (costRemaining != 0)
            {
                return false;
            }
        }
        else if (multipleBudgetsCanFundEachTreatment)
        {
            // Use LP with GLOP. Build the LP.

            using var solver = Solver.CreateSolver("GLOP") ??
                throw new Exception("Solver could not be created.");

            var allocationVariablesMatrix = new Variable[budgetAmounts.Length, treatmentCosts.Length];
            var allocationVariablesPerBudget = new List<Variable>[budgetAmounts.Length];
            var allocationVariablesVector = new MPVariableVector(allocationVariablesMatrix.Length);

            var spendingConstraints = new Constraint[budgetAmounts.Length];
            var fundingConstraints = new Constraint[treatmentCosts.Length];

            for (var t = 0; t < treatmentCosts.Length; ++t)
            {
                const decimal fundingTolerance = 0.001m; // 1/10th of 1 cent
                fundingConstraints[t] = solver.MakeConstraint(
                    (double)(treatmentCosts[t] - fundingTolerance),
                    (double)(treatmentCosts[t] + fundingTolerance),
                    $"funding[{t}]");
            }

            for (var b = 0; b < budgetAmounts.Length; ++b)
            {
                spendingConstraints[b] = solver.MakeConstraint(0, (double)budgetAmounts[b], $"spending[{b}]");
                allocationVariablesPerBudget[b] = new(treatmentCosts.Length);

                for (var t = 0; t < treatmentCosts.Length; ++t)
                {
                    if (allocationIsAllowed[b, t])
                    {
                        var maximumAllocation = Math.Min(budgetAmounts[b], treatmentCosts[t]);
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

            for (var b = 0; b < budgetAmounts.Length - 1; ++b)
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
                    // change the feasibility of any solution.)
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

            for (var b = 0; b < budgetAmounts.Length; ++b)
            {
                for (var t = 0; t < treatmentCosts.Length; ++t)
                {
                    if (allocationVariablesMatrix[b, t] is Variable allocationVariable)
                    {
                        solution[b, t] = Math.Round((decimal)allocationVariable.SolutionValue(), 2);
                    }
                }
            }

            // Manually dispose collections of disposables.

            foreach (var v in allocationVariablesMatrix)
            {
                v?.Dispose();
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

        for (var b = 0; b < budgetAmounts.Length; ++b)
        {
            var spending = totalBudgetSpending(solution, b);
            var amount = Math.Round(budgetAmounts[b], 2);

            if (spending > amount)
            {
                throw new Exception($"Budget spending exceeds amount by [{spending - amount}].");
            }
        }

        for (var t = 0;  t < treatmentCosts.Length; ++t)
        {
            var funding = totalTreatmentFunding(solution, t);
            var cost = Math.Round(treatmentCosts[t], 2);

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
