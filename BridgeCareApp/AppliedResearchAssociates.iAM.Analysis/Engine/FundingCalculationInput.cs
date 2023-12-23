using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis.Logic;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

public sealed class FundingCalculationInput
{
    public FundingCalculationInput()
    {
    }

    public FundingCalculationInput(FundingCalculationInput original)
    {
        BudgetsToSpend.AddRange(original.BudgetsToSpend);
        CashFlowDistribution.AddRange(original.CashFlowDistribution);
        ExclusionMatrix.AddRange(original.ExclusionMatrix);
        TreatmentsToFund.AddRange(original.TreatmentsToFund);

        Settings = original.Settings;
    }

    public enum ExclusionReason
    {
        Unknown,
        Other,
        TreatmentSettings,
        BudgetConditions,
    }

    public List<Budget> BudgetsToSpend { get; } = new();

    public List<CashFlowPoint> CashFlowDistribution { get; } = new();

    public List<Exclusion> ExclusionMatrix { get; } = new();

    public Funding.Settings Settings { get; set; } = new();

    public List<Treatment> TreatmentsToFund { get; } = new();

    public sealed record Budget(string Name, decimal Amount, int Year);

    public sealed record CashFlowPoint(int Year, decimal Percentage);

    public sealed record Exclusion(string BudgetName, string TreatmentName, ExclusionReason Reason);

    public sealed record Treatment(string Name, decimal Cost);
}
