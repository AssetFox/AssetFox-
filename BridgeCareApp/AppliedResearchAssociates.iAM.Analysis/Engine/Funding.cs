using System;
using System.Collections.Generic;
using System.Numerics;
using Google.OrTools.LinearSolver;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

public static class Funding
{
    public static decimal?[,] Solve(
        bool[,] spendingIsAllowed,
        decimal[] budgetAmounts,
        decimal[] treatmentCosts,
        bool multipleBudgetsCanFundEachTreatment)
    {
        // Pre-validation

        if (spendingIsAllowed is null)
        {
            throw new ArgumentNullException(nameof(spendingIsAllowed));
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

        if (spendingIsAllowed.GetLength(0) != budgetAmounts.Length ||
            spendingIsAllowed.GetLength(1) != treatmentCosts.Length)
        {
            throw new Exception("Inconsistent input sizes.");
        }

        var amounts = new double[budgetAmounts.Length];
        for (var b = 0; b < amounts.Length; ++b)
        {
            amounts[b] = Math.Round((double)budgetAmounts[b], 2);

            if (amounts[b] < 0)
            {
                throw new ArgumentException($"Budget amount [{b}] is negative.");
            }
        }

        var costs = new double[treatmentCosts.Length];
        for (var t = 0; t < costs.Length; ++t)
        {
            costs[t] = Math.Round((double)treatmentCosts[t], 2);

            if (costs[t] < 0)
            {
                throw new ArgumentException($"Treatment cost [{t}] is negative.");
            }
        }

        // Optimization

        var solution = new decimal?[amounts.Length, costs.Length];

        if (multipleBudgetsCanFundEachTreatment)
        {
            // LP with GLOP

            // Build the LP

            using var solver = Solver.CreateSolver("GLOP") ??
                throw new Exception("Solver could not be created.");

            var objective = solver.Objective();

            var amountConstraints = new Constraint[amounts.Length];
            var costConstraints = new Constraint[costs.Length];

            var variablesPerBudget = new List<Variable>[amounts.Length];
            var spendingExpressions = new LinearExpr[amounts.Length];

            for (var t = 0; t < costs.Length; ++t)
            {
                const double fundingTolerance = 0.001; // 1/10th of 1 cent
                costConstraints[t] = solver.MakeConstraint(
                    costs[t] - fundingTolerance,
                    costs[t] + fundingTolerance,
                    $"f[b,{t}]");
            }

            for (var b = 0; b < amounts.Length; ++b)
            {
                amountConstraints[b] = solver.MakeConstraint(0, amounts[b], $"a[{b},t]");
                variablesPerBudget[b] = new(costs.Length);

                for (var t = 0; t < costs.Length; ++t)
                {
                    if (spendingIsAllowed[b, t])
                    {
                        var maximumSpending = Math.Min(amounts[b], costs[t]);
                        var spendingVariable = solver.MakeNumVar(0, maximumSpending, $"s[{b},{t}]");

                        amountConstraints[b].SetCoefficient(spendingVariable, 1);
                        costConstraints[t].SetCoefficient(spendingVariable, 1);

                        variablesPerBudget[b].Add(spendingVariable);

                        if (spendingExpressions[b] is null)
                        {
                            spendingExpressions[b] = spendingVariable;
                        }
                        else
                        {
                            spendingExpressions[b] += spendingVariable;
                        }
                    }
                }
            }

            // Use the solver

            for (var b = 0; b < amounts.Length; ++b)
            {
                objective.Clear();
                objective.SetMaximization();
                foreach (var variable in variablesPerBudget[b])
                {
                    objective.SetCoefficient(variable, 1);
                }

                // solve

                // update ub for amountConstraints[b]
            }

            // Check & accept the solution

            // fill solution from latest spent vals (converting to decimal and
            // rounding to 2 places)

            // Consider manually disposing all the non-solver disposables.
        }
        else
        {
            // MIP with CP-SAT

            // update allowedSpending by disallowing where amount_b < cost_t. (don't un-disallow anything!)
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

        return solution;

        // Local functions

        static decimal totalBudgetSpending(decimal?[,] spent, int b)
        {
            decimal? total = 0;

            for (var t = 0; t < spent.GetLength(1); ++t)
            {
                total += spent[b, t];
            }

            return total.Value;
        }

        static decimal totalTreatmentFunding(decimal?[,] spent, int t)
        {
            decimal? total = 0;

            for (var b = 0; b < spent.GetLength(0); ++b)
            {
                total += spent[b, t];
            }

            return total.Value;
        }
    }
}
