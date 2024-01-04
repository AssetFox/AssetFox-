﻿using Google.OrTools.Sat;

namespace AppliedResearchAssociates.iAM.Analysis.Logic;

partial record FundingSolver
{
    private bool CP()
    {
        // Build the CP model.

        var model = new CpModel();

        // basically the same. the allocation variables become bool, and in the spending
        // constraints the coefficient on each variable transforms from 1 to the cost of the
        // treatment corresponding to the variable. the funding constraints can simplify to ExactlyOne's.

        // need a method that will regenerate the model given fixed LBs for budget spending and hints.

        var allocationVariablesMatrix = new BoolVar[NumberOfYears, NumberOfBudgets, NumberOfTreatments];

        var allocationVariablesPerBudget = new List<BoolVar>[NumberOfBudgets];
        for (var b = 0; b < NumberOfBudgets; ++b)
        {
            allocationVariablesPerBudget[b] = new(NumberOfYears * NumberOfTreatments);
        }

        var allocationVariablesPerYearAndBudget = new List<BoolVar>[NumberOfYears, NumberOfBudgets];
        var allocationVariablesPerYearAndTreatment = new List<BoolVar>[NumberOfYears, NumberOfTreatments];
        for (var y = 0; y < NumberOfYears; ++y)
        {
            for (var b = 0; b < NumberOfBudgets; ++b)
            {
                allocationVariablesPerYearAndBudget[y, b] = new(NumberOfTreatments);
            }

            for (var t = 0; t < NumberOfTreatments; ++t)
            {
                allocationVariablesPerYearAndTreatment[y, t] = new(NumberOfBudgets);
            }
        }

        // These constraints are needed to maximize and then fix each budget's spending in order.
        var totalSpendingConstraintPerBudget = new Constraint[NumberOfBudgets];

        // Create yearly constraints and variables.
        for (var y = 0; y < NumberOfYears; ++y)
        {
            var amountPerBudget = AmountPerBudgetPerYear[y];
            var costFraction = CostPercentagePerYear[y] / 100;

            // Create budget spending constraints.
            for (var b = 0; b < NumberOfBudgets; ++b)
            {
                var spendingConstraint = solver.MakeConstraint($"spending[{y},{b}]");
                spendingConstraintPerYearAndBudget[y, b] = spendingConstraint;

                spendingConstraint.SetLb(0);

                if (Settings.UnlimitedSpending)
                {
                    spendingConstraint.SetUb(double.PositiveInfinity);
                }
                else
                {
                    var amount = amountPerBudget[b];

                    if (Settings.BudgetCarryoverIsAllowed)
                    {
                        // Incorporate each previous year's budget amount and allocation variables.
                        for (var y0 = 0; y0 < y; ++y0)
                        {
                            amount += AmountPerBudgetPerYear[y0][b];

                            for (var t = 0; t < NumberOfTreatments; ++t)
                            {
                                if (allocationVariablesMatrix[y0, b, t] is Variable allocationVariable)
                                {
                                    spendingConstraint.SetCoefficient(allocationVariable, 1);
                                }
                            }
                        }
                    }

                    spendingConstraint.SetUb((double)amount);
                }
            }

            // Create treatment funding constraints.
            for (var t = 0; t < NumberOfTreatments; ++t)
            {
                var fundingConstraint = solver.MakeConstraint($"funding[{y},{t}]");
                fundingConstraintPerYearAndTreatment[y, t] = fundingConstraint;

                var cost = CostPerTreatment[t] * costFraction;

                fundingConstraint.SetLb((double)cost);
                fundingConstraint.SetUb((double)cost);
            }

            // Create allocation variables.
            for (var b = 0; b < NumberOfBudgets; ++b)
            {
                for (var t = 0; t < NumberOfTreatments; ++t)
                {
                    if (AllocationIsAllowedPerBudgetAndTreatment[b, t])
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

        // Use the LP solver. In budget order, maximize each budget's total spending.

        var allocationValues = new double[allocationVariablesVector.Count];

        using var objective = solver.Objective();

        for (var b = 0; b < NumberOfBudgets - 1; ++b)
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

        for (var y = 0; y < NumberOfYears; ++y)
        {
            var allocationPerBudgetAndTreatment = AllocationPerBudgetAndTreatmentPerYear[y];

            for (var b = 0; b < NumberOfBudgets; ++b)
            {
                for (var t = 0; t < NumberOfTreatments; ++t)
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

        return true;
    }
}
