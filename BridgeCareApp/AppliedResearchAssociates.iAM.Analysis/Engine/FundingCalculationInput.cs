using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

public sealed class FundingCalculationInput : IEquatable<FundingCalculationInput>
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

    public bool Equals(FundingCalculationInput other) =>
        other is not null && (ReferenceEquals(this, other) ||
        Enumerable.SequenceEqual(CurrentBudgetsToSpend, other.CurrentBudgetsToSpend) &&
        Enumerable.SequenceEqual(ExclusionsMatrix, other.ExclusionsMatrix) &&
        Enumerable.SequenceEqual(TreatmentsToFund, other.TreatmentsToFund));

    public override bool Equals(object obj) => Equals(obj as FundingCalculationInput);

    public override int GetHashCode() =>
        CurrentBudgetsToSpend
        .Concat<object>(ExclusionsMatrix)
        .Concat(TreatmentsToFund)
        .ReduceToHashCode()
        .ToHashCode();

    public sealed class CashFlowSupplement : IEquatable<CashFlowSupplement>
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

        public bool Equals(CashFlowSupplement other) =>
            other is not null && (ReferenceEquals(this, other) ||
            Enumerable.SequenceEqual(CashFlowDistribution, other.CashFlowDistribution) &&
            Enumerable.SequenceEqual(FutureBudgetsToSpend, other.FutureBudgetsToSpend));

        public override bool Equals(object obj) => Equals(obj as CashFlowSupplement);

        public override int GetHashCode() =>
            CashFlowDistribution
            .Concat<object>(FutureBudgetsToSpend)
            .ReduceToHashCode()
            .ToHashCode();
    }

    public sealed record Budget(string Name, decimal Amount, int Year);

    public sealed record CashFlowPoint(int Year, decimal Percentage);

    public sealed record Exclusion(string BudgetName, string TreatmentName, ExclusionReason Reason);

    public sealed record Treatment(string Name, decimal Cost);
}
