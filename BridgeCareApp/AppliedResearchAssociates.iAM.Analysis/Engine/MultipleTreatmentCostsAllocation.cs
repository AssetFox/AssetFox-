using System;
using System.Linq;
using Google.OrTools.LinearSolver;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

public static class MultipleTreatmentCostsAllocation
{
    public static SolvedBudget[] Solve(
        bool[,] allowedSpending,
        decimal[] budgetAmounts,
        decimal[] treatmentCosts,
        bool allowFundingEachTreatmentWithMultipleBudgets)
    {
        if (allowedSpending.GetLength(0) != budgetAmounts.Length ||
            allowedSpending.GetLength(1) != treatmentCosts.Length)
        {
            throw new Exception("Inconsistent input sizes.");
        }

        for (var b = 0; b < budgetAmounts.Length; ++b)
        {
            Round(ref budgetAmounts[b], 2);
        }

        for (var t = 0; t < treatmentCosts.Length; ++t)
        {
            Round(ref treatmentCosts[t], 2);
        }

        var solution = new SolvedBudget[budgetAmounts.Length];

        if (allowFundingEachTreatmentWithMultipleBudgets)
        {
            // LP with GLOP

            Variable[,] spentVariables;
            var unspentValues = new double[budgetAmounts.Length];

            for (var b_solving = 0; b_solving < budgetAmounts.Length; ++b_solving)
            {
                // if no allowed spending, set unspent value and skip to next.

                using var solver = Solver.CreateSolver("GLOP") ??
                    throw new Exception("Solver could not be created.");

                spentVariables = new Variable[budgetAmounts.Length, treatmentCosts.Length];
                var unspentVariable = solver.MakeNumVar(
                    0,
                    (double)budgetAmounts[b_solving],
                    $"u[{b_solving}]");

                for (var b_solved = 0; b_solved < b_solving; ++b_solved)
                {
                    var amount = budgetAmounts[b_solved];
                    for (var t = 0; t < treatmentCosts.Length; ++t)
                    {
                        if (allowedSpending[b_solved, t])
                        {
                            spentVariables[b_solved, t] = solver.MakeNumVar(
                                0,
                                (double)Math.Min(amount, treatmentCosts[t]),
                                $"s[{b_solved},{t}]");
                        }
                    }

                    // add constraint \sum_t s[b,t] = u_b
                }

                // b_solving vars

                for (var b_unsolved = b_solving + 1; b_unsolved < budgetAmounts.Length; ++b_unsolved)
                {
                    // vars
                }
            }

            // fill solution from latest spent vars and unspent vals
        }
        else
        {
            // MIP with CP-SAT
        }

        for (var b = 0; b < budgetAmounts.Length; ++b)
        {
            var (spent, unspent) = solution[b];
            if (spent.Sum() + unspent != budgetAmounts[b])
            {
                throw new Exception("Budget spending is invalid.");
            }
        }

        for (var t = 0;  t < treatmentCosts.Length; ++t)
        {
            if (solution.Sum(b => b.Spent[t]) != treatmentCosts[t])
            {
                throw new Exception("Treatment funding is invalid.");
            }
        }

        return solution;
    }

    private static void Round(ref decimal d, int decimals) => d = Math.Round(d, decimals);

    public sealed record SolvedBudget(decimal?[] Spent, decimal Unspent);
}
