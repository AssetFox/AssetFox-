using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

/// <summary>
/// The details used for a specific decision which is a unique
/// treatment-asset pair in a given year
/// </summary>
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

    /// <summary>
    /// The priority level of the budget(s) that would be used to apply the treatment
    /// </summary>
    public int? BudgetPriorityLevel { get; set; }

    /// <summary>
    ///     The amounts in each budget prior to the analysis of this consideration.
    /// </summary>
    public List<BudgetDetail> BudgetsAtDecisionTime { get; } = new List<BudgetDetail>();

    /// <summary>
    /// How each budget was or was not used to fund the treatment.
    /// </summary>
    public List<BudgetUsageDetail> BudgetUsages { get; } = new List<BudgetUsageDetail>();

    /// <summary>
    /// How each cash-flow rule was processed to determine if the treatment would be cash-flowed.
    /// </summary>
    public List<CashFlowConsiderationDetail> CashFlowConsiderations { get; } = new List<CashFlowConsiderationDetail>();

    /// <summary>
    /// The treatment being considered
    /// </summary>
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
