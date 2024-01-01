using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

public sealed class FundingCalculationInput
{
    public FundingCalculationInput()
    {
    }

    public FundingCalculationInput(FundingCalculationInput original)
    {
        CurrentBudgetsToSpend = original.CurrentBudgetsToSpend.ToList();
        ExclusionsMatrix = original.ExclusionsMatrix.ToList();
        TreatmentsToFund = original.TreatmentsToFund.ToList();
    }

    public enum ExclusionReason
    {
        Unknown,
        Other,
        TreatmentSettings,
        BudgetConditions,
    }

    public List<Budget> CurrentBudgetsToSpend { get; } = new();

    public List<Exclusion> ExclusionsMatrix { get; } = new();

    public List<Treatment> TreatmentsToFund { get; } = new();

    public sealed record Budget(string Name, decimal Amount, int Year);

    public sealed record CashFlowPoint(int Year, decimal Percentage);

    public sealed record Exclusion(string BudgetName, string TreatmentName, ExclusionReason Reason);

    public sealed record Treatment(string Name, decimal Cost);

    public sealed class CashFlowSupplement
    {
        public CashFlowSupplement()
        {
        }

        public CashFlowSupplement(CashFlowSupplement original)
        {
            CashFlowDistribution = original.CashFlowDistribution.ToList();
            FutureBudgetsToSpend = original.FutureBudgetsToSpend.ToList();
        }

        public List<CashFlowPoint> CashFlowDistribution { get; } = new();

        public List<Budget> FutureBudgetsToSpend { get; } = new();
    }
}
