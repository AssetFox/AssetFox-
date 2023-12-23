using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

public sealed class FundingCalculationOutput
{
    public FundingCalculationOutput()
    {
    }

    public FundingCalculationOutput(FundingCalculationOutput original)
    {
        AllocationMatrix.AddRange(original.AllocationMatrix);
    }

    public List<Allocation> AllocationMatrix { get; } = new();

    public sealed record Allocation(int Year, string BudgetName, string TreatmentName, decimal AllocatedAmount);
}
