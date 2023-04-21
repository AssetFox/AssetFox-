using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer;

public sealed class BudgetPriority
{
    public List<BudgetPercentagePair> BudgetPercentagePairs { get; set; }

    public string CriterionExpression { get; set; }

    public int PriorityLevel { get; set; }

    public int? Year { get; set; }
}
