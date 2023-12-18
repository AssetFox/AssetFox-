using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

public sealed class FundingCalculationInput
{
    public FundingCalculationInput()
    {
    }

    public FundingCalculationInput(FundingCalculationInput original)
    {
        Budgets.AddRange(original.Budgets);
        Treatments.AddRange(original.Treatments);
        ExclusionMatrix.AddRange(original.ExclusionMatrix);
        MultiBudgetFundingIsAllowed = original.MultiBudgetFundingIsAllowed;
    }

    public enum ExclusionReason
    {
        Unknown,
        Other,
        TreatmentSettings,
        BudgetConditions,
    }

    public List<Budget> Budgets { get; } = new();

    public List<Exclusion> ExclusionMatrix { get; } = new();

    public bool MultiBudgetFundingIsAllowed { get; set; }

    public List<Treatment> Treatments { get; } = new();

    public sealed record Budget(string Name, decimal Amount);

    public sealed record Treatment(string Name, decimal Cost);

    public sealed record Exclusion(string BudgetName, string TreatmentName, ExclusionReason Reason);
}
