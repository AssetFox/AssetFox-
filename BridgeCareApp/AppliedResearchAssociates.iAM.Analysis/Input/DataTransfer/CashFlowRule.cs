using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer;

public sealed class CashFlowRule
{
    public string CriterionExpression { get; set; }

    public List<CashFlowDistributionRule> DistributionRules { get; set; }

    public string Name { get; set; }
}
