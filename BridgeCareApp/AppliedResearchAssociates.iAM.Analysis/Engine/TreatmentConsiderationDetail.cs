using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

/// <summary>
///     The details used for a specific decision which is a unique treatment-asset pair in a given year.
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

    internal TreatmentConsiderationDetail(TreatmentConsiderationDetail original)
    {
        TreatmentName = original.TreatmentName;
        BudgetPriorityLevel = original.BudgetPriorityLevel;
        CashFlowConsiderations.AddRange(original.CashFlowConsiderations.Select(_ => new CashFlowConsiderationDetail(_)));

        FundingCalculationInput = new(original.FundingCalculationInput);
        FundingCalculationOutput = new(original.FundingCalculationOutput);
    }

    /// <summary>
    ///     The priority level of the budget(s) that would be used to apply the treatment.
    /// </summary>
    public int? BudgetPriorityLevel { get; set; }

    /// <summary>
    ///     How each cash-flow rule was processed to determine if the treatment would be cash-flowed.
    /// </summary>
    public List<CashFlowConsiderationDetail> CashFlowConsiderations { get; } = new List<CashFlowConsiderationDetail>();

    public FundingCalculationInput FundingCalculationInput { get; } = new();

    public FundingCalculationOutput FundingCalculationOutput { get; } = new();

    /// <summary>
    ///     The treatment being considered.
    /// </summary>
    public string TreatmentName { get; }
}
