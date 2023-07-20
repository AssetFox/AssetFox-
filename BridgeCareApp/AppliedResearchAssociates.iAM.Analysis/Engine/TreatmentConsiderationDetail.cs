using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

public sealed class TreatmentConsiderationDetail
{
    public TreatmentConsiderationDetail(string treatmentName)
    {
        if (string.IsNullOrWhiteSpace(treatmentName))
        {
            throw new ArgumentException("Treatment name is blank.", nameof(treatmentName));
        }

        TreatmentName = treatmentName;
    }

    public int? BudgetPriorityLevel { get; set; }

    /// <summary>
    ///     These are the amounts in each budget prior to this consideration being decided.
    /// </summary>
    public List<BudgetDetail> BudgetsAtDecisionTime { get; } = new List<BudgetDetail>();

    public List<BudgetUsageDetail> BudgetUsages { get; } = new List<BudgetUsageDetail>();

    public List<CashFlowConsiderationDetail> CashFlowConsiderations { get; } = new List<CashFlowConsiderationDetail>();

    public string TreatmentName { get; }

    internal TreatmentConsiderationDetail(TreatmentConsiderationDetail original)
    {
        TreatmentName = original.TreatmentName;
        BudgetUsages.AddRange(original.BudgetUsages.Select(_ => new BudgetUsageDetail(_)));
        CashFlowConsiderations.AddRange(original.CashFlowConsiderations.Select(_ => new CashFlowConsiderationDetail(_)));
        BudgetsAtDecisionTime.AddRange(original.BudgetsAtDecisionTime.Select(_ => new BudgetDetail(_)));

        BudgetPriorityLevel = original.BudgetPriorityLevel;
    }
}
