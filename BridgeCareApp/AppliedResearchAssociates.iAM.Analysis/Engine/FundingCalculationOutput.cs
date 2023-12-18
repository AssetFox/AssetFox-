using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

public sealed class FundingCalculationOutput
{
    public FundingCalculationOutput()
    {
    }
    public FundingCalculationOutput(FundingCalculationOutput original)
    {
        SpendingMatrix.AddRange(original.SpendingMatrix);
    }
    public List<Spending> SpendingMatrix { get; } = new();
    public sealed record Spending(string BudgetName, string TreatmentName, decimal Spent);
}
