using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer;

public sealed class CashFlowDistributionRule
{
    public decimal? CostCeiling { get; set; }

    public List<decimal> YearlyPercentages { get; set; }
}
