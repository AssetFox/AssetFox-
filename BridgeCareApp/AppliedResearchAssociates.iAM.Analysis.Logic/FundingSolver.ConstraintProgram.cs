using Google.OrTools.ConstraintSolver;
using Google.OrTools.Sat;

namespace AppliedResearchAssociates.iAM.Analysis.Logic;

partial record FundingSolver
{
    private bool ConstraintProgram()
    {
        // Unlike the API used in the linear program implementation, the API used here doesn't allow
        // updating the model as you go along. Because of that, we have to recreate the entire model
        // for each budget whose spending we're maximizing.

        // In budget order, maximize each budget's total spending.

        var maximizedTotalSpendingPerBudget = new List<long>(NumberOfBudgets);
        var currentAllocationPerYearAndBudgetAndTreatment = new long?[NumberOfYears, NumberOfBudgets, NumberOfTreatments];

        for (var budgetToMaximize = 0; budgetToMaximize < NumberOfBudgets - 1; ++budgetToMaximize)
        {
            // Create data structures to organize model variables and (later) retrieve certain subsets
            // and associated coefficients.

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

            var costCoefficientPerAllocationVariable =
                new Dictionary<BoolVar, long>(NumberOfYears * NumberOfBudgets * NumberOfTreatments);

            // Create the model.

            var model = new CpModel();

            // Create the model variables.

            for (var t = 0; t < NumberOfTreatments; ++t)
            {
                var totalCost = CostPerTreatment[t];

                for (var b = 0; b < NumberOfBudgets; ++b)
                {
                    if (AllocationIsAllowedPerBudgetAndTreatment[b, t])
                    {
                        for (var y = 0; y < NumberOfYears; ++y)
                        {
                            var allocationVariable = model.NewBoolVar($"allocation[{y},{b},{t}]");

                            allocationVariablesMatrix[y, b, t] = allocationVariable;
                            allocationVariablesPerBudget[b].Add(allocationVariable);
                            allocationVariablesPerYearAndBudget[y, b].Add(allocationVariable);
                            allocationVariablesPerYearAndTreatment[y, t].Add(allocationVariable);

                            var costFraction = CostPercentagePerYear[y] / 100;
                            var cost = totalCost * costFraction;
                            var costCoefficient = cost.ToCpModelMoneyInteger();

                            costCoefficientPerAllocationVariable.Add(allocationVariable, costCoefficient);
                        }
                    }
                }
            }

            // Create the model constraints.

            for (var y = 0; y < NumberOfYears; ++y)
            {
                var amountPerBudget = AmountPerBudgetPerYear[y];

                for (var b = 0; b < NumberOfBudgets; ++b)
                {
                    var amount = amountPerBudget[b];

                    var allocationVariables = allocationVariablesPerYearAndBudget[y, b];
                    if (Settings.BudgetCarryoverIsAllowed)
                    {
                        allocationVariables = allocationVariables.ToList();
                        for (var y0 = 0; y0 < y; ++y0)
                        {
                            var previousYearAllocationVariables = allocationVariablesPerYearAndBudget[y0, b];
                            allocationVariables.AddRange(previousYearAllocationVariables);

                            var previousYearAmount = AmountPerBudgetPerYear[y0][b];
                            amount += previousYearAmount;
                        }
                    }

                    var costCoefficients = getCostCoefficients(allocationVariables);
                    var spendingConstraintExpression = LinearExpr.WeightedSum(allocationVariables, costCoefficients);

                    _ = model.AddLinearConstraint(spendingConstraintExpression, 0, amount.ToCpModelMoneyInteger());
                }

                for (var t = 0; t < NumberOfTreatments; ++t)
                {
                    var allocationVariables = allocationVariablesPerYearAndTreatment[y, t];
                    _ = model.AddExactlyOne(allocationVariables);
                }
            }

            // Configure the model with previous solution data, if any.

            for (var b = 0; b < maximizedTotalSpendingPerBudget.Count; ++b)
            {
                var maximizedExpression = createTotalSpendingExpression(b);
                var maximizedValue = maximizedTotalSpendingPerBudget[b];
                var absoluteMaximumValue = TotalAmountPerBudget(AmountPerBudgetPerYear, b).ToCpModelMoneyInteger();
                _ = model.AddLinearConstraint(maximizedExpression, maximizedValue, absoluteMaximumValue);
            }

            for (var y = 0; y < NumberOfYears; ++y)
            {
                for (var b = 0; b < NumberOfBudgets; ++b)
                {
                    for (var t = 0; t < NumberOfTreatments; ++t)
                    {
                        if (currentAllocationPerYearAndBudgetAndTreatment[y, b, t] is long value)
                        {
                            var variable = allocationVariablesMatrix[y, b, t];
                            model.AddHint(variable, value);
                        }
                    }
                }
            }

            // Solve the model.

            var maximizationExpression = createTotalSpendingExpression(budgetToMaximize);
            model.Maximize(maximizationExpression);

            var solver = new CpSolver();
            var solverStatus = solver.Solve(model);

            if (solverStatus is not (CpSolverStatus.Optimal or CpSolverStatus.Feasible))
            {
                return false;
            }

            // Update the solution data.

            var maximizedTotalSpending = solver.Value(maximizationExpression);
            maximizedTotalSpendingPerBudget.Add(maximizedTotalSpending);

            for (var y = 0; y < NumberOfYears; ++y)
            {
                for (var b = 0; b < NumberOfBudgets; ++b)
                {
                    for (var t = 0; t < NumberOfTreatments; ++t)
                    {
                        if (allocationVariablesMatrix[y, b, t] is BoolVar variable)
                        {
                            var value = solver.Value(variable);
                            currentAllocationPerYearAndBudgetAndTreatment[y, b, t] = value;
                        }
                        else
                        {
                            currentAllocationPerYearAndBudgetAndTreatment[y, b, t] = null;
                        }
                    }
                }
            }

            // --- local functions ---

            LinearExpr createTotalSpendingExpression(int b)
            {
                var variables = allocationVariablesPerBudget[b];
                var coefficients = getCostCoefficients(variables);
                var expression = LinearExpr.WeightedSum(variables, coefficients);
                return expression;
            }

            long[] getCostCoefficients(List<BoolVar> allocationVariables)
            {
                var costCoefficients = new long[allocationVariables.Count];

                for (var i = 0; i < costCoefficients.Length; ++i)
                {
                    var v = allocationVariables[i];
                    costCoefficients[i] = costCoefficientPerAllocationVariable[v];
                }

                return costCoefficients;
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
                    if (currentAllocationPerYearAndBudgetAndTreatment[y, b, t] is long value)
                    {
                        var allocation = (decimal)allocationVariable.SolutionValue();
                        allocationPerBudgetAndTreatment[b, t] = allocation.RoundToCent();
                    }
                }
            }
        }

        return true;
    }
}
