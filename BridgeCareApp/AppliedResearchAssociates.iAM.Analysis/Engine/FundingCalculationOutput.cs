using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

public sealed class FundingCalculationOutput : IEquatable<FundingCalculationOutput>
{
    public FundingCalculationOutput()
    {
    }

    public FundingCalculationOutput(FundingCalculationOutput original)
    {
        AllocationMatrix.AddRange(original.AllocationMatrix);
    }

    public List<Allocation> AllocationMatrix { get; } = new();

    public bool Equals(FundingCalculationOutput other) =>
        other is not null && (ReferenceEquals(this, other) ||
        Enumerable.SequenceEqual(AllocationMatrix, other.AllocationMatrix));

    public override bool Equals(object obj) => Equals(obj as FundingCalculationOutput);

    public override int GetHashCode() => AllocationMatrix.ReduceToHashCode().ToHashCode();

    public sealed record Allocation(int Year, string BudgetName, string TreatmentName, decimal AllocatedAmount);
}
