using System;
using Google.OrTools.LinearSolver;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

public static class MultipleTreatmentCostsAllocation
{
    public static SolvedBudget[] Solve(
        (int budget, int treatment)[] allowedSpending,
        double[] budgetAmounts,
        double[] treatmentCosts)
    {
        var solution = new SolvedBudget[budgetAmounts.Length];

        for (var b_solving = 0;  b_solving < budgetAmounts.Length; ++b_solving)
        {
            var solver = Solver.CreateSolver("GLOP") ??
                throw new Exception("Solver could not be created.");

            for (var b_solved = 0; b_solved < b_solving; ++b_solved)
            {
                // vars
            }

            // b_solving vars

            for (var b_unsolved = b_solving + 1; b_unsolved < budgetAmounts.Length; ++b_unsolved)
            {
                // vars
            }
        }
    }

    public sealed record SolvedBudget(double?[] Spent, double Unspent);
}
