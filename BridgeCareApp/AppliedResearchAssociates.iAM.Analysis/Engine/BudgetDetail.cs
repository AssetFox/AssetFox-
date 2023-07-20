using System;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

/// <summary>
///     Represents a specific budget available to the scenario in a given time period.
/// </summary>
public sealed class BudgetDetail
{
    public BudgetDetail(Budget budget, decimal availableFunding)
    {
        BudgetName = budget?.Name ?? throw new ArgumentNullException(nameof(budget));
        AvailableFunding = availableFunding;
    }

    [JsonConstructor]
    public BudgetDetail(decimal availableFunding, string budgetName)
    {
        AvailableFunding = availableFunding;
        BudgetName = budgetName ?? throw new ArgumentNullException(nameof(budgetName));
    }

    /// <summary>
    ///     Amount of funding available.
    /// </summary>
    public decimal AvailableFunding { get; }

    /// <summary>
    ///     Name of the budget.
    /// </summary>
    public string BudgetName { get; }

    internal BudgetDetail(BudgetDetail original)
    {
        AvailableFunding = original.AvailableFunding;
        BudgetName = original.BudgetName;
    }
}
