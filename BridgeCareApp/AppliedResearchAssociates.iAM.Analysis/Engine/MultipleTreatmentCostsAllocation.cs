using System;
using System.Linq;
using Google.OrTools.LinearSolver;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

public static class MultipleTreatmentCostsAllocation
{
    public static decimal?[,] Solve(
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

        var solution = new decimal?[budgetAmounts.Length, treatmentCosts.Length];

        if (allowFundingEachTreatmentWithMultipleBudgets)
        {
            // LP with GLOP

            var spentValues = new double?[budgetAmounts.Length, treatmentCosts.Length];
            var unspentValues = new double[budgetAmounts.Length];

            for (var bSolving = 0; bSolving < budgetAmounts.Length; ++bSolving)
            {
                using var solver = Solver.CreateSolver("GLOP") ??
                    throw new Exception("Solver could not be created.");

                var spentVariables = new Variable[budgetAmounts.Length, treatmentCosts.Length];

                for (var bSolved = 0; bSolved < bSolving; ++bSolved)
                {
                    for (var t = 0; t < treatmentCosts.Length; ++t)
                    {
                        if (allowedSpending[bSolved, t])
                        {
                            spentVariables[bSolved, t] = solver.MakeNumVar(
                                0,
                                (double)Math.Min(budgetAmounts[bSolved], treatmentCosts[t]),
                                $"s[{bSolved},{t}]");
                        }
                    }

                    var spentConstraint = solver.MakeConstraint(
                        unspentValues[bSolved],
                        unspentValues[bSolved],
                        $"b[{bSolved}]_spent");

                    for (var t = 0; t < treatmentCosts.Length; ++t)
                    {
                        spentConstraint.SetCoefficient(spentVariables[bSolved, t], 1);
                    }
                }

                //

                for (var t = 0; t < treatmentCosts.Length; ++t)
                {
                    if (allowedSpending[bSolving, t])
                    {
                        spentVariables[bSolving, t] = solver.MakeNumVar(
                            0,
                            (double)Math.Min(budgetAmounts[bSolving], treatmentCosts[t]),
                            $"s[{bSolving},{t}]");
                    }
                }

                var unspentVariable = solver.MakeNumVar(
                    0,
                    (double)budgetAmounts[bSolving],
                    $"u[{bSolving}]");

                var spendingConstraint = solver.MakeConstraint(
                    (double)budgetAmounts[bSolving],
                    (double)budgetAmounts[bSolving],
                    $"b[{bSolving}]_spending");

                //

                for (var bUnsolved = bSolving + 1; bUnsolved < budgetAmounts.Length; ++bUnsolved)
                {
                    // vars, amount constraint
                }

                // cost constraints
            }

            // fill solution from latest spent and unspent vals (converting to decimal and
            // rounding to 2 places)
        }
        else
        {
            // MIP with CP-SAT

            // update allowedSpending by disallowing where amount_b < cost_t. (don't un-disallow anything!)
        }

        for (var b = 0; b < budgetAmounts.Length; ++b)
        {
            var (spent, unspent) = solution[b];
            if (spent.Sum() + unspent > budgetAmounts[b])
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
}
