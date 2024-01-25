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

        if (original.FundingCalculationInput != null)
        {
            FundingCalculationInput = new(original.FundingCalculationInput);
        }

        if (original.FundingCalculationOutput != null)
        {
            FundingCalculationOutput = new(original.FundingCalculationOutput);
        }
    }

    /// <summary>
    ///     The priority level of the budget(s) that would be used to apply the treatment.
    /// </summary>
    public int? BudgetPriorityLevel { get; set; }

    /// <summary>
    ///     How each cash-flow rule was processed to determine if the treatment would be cash-flowed.
    /// </summary>
    public List<CashFlowConsiderationDetail> CashFlowConsiderations { get; } = new();

    public FundingCalculationInput FundingCalculationInput { get; set; }

    public FundingCalculationOutput FundingCalculationOutput { get; set; }

    /// <summary>
    ///     The treatment being considered.
    /// </summary>
    public string TreatmentName { get; }

    public BudgetUsageStatus GetBudgetUsageStatus(int year, string budget, string treatment)
    {
        // The output allocations should always have positive amounts. If zero amounts are allowed,
        // this logic needs to be adjusted.

        if (FundingCalculationOutput.AllocationMatrix.Any(a => a.AllocatedAmount <= 0))
        {
            throw new Exception("Output allocations include non-positive amounts.");
        }

        if (FundingCalculationInput == null && FundingCalculationOutput != null)
        {
            // It was a committed project (automatically funded).

            var allocation = FundingCalculationOutput.AllocationMatrix.SingleOrDefault(
                a => (a.Year, a.BudgetName, a.TreatmentName) == (year, budget, treatment));

            return allocation != null ? BudgetUsageStatus.CostCovered : BudgetUsageStatus.NotUsable;
        }
        else if (FundingCalculationInput != null && FundingCalculationOutput == null)
        {
            // It couldn't be funded.

            var exclusion = FundingCalculationInput.ExclusionsMatrix.SingleOrDefault(
                e => (e.BudgetName, e.TreatmentName) == (budget, treatment));

            return exclusion?.Reason switch
            {
                FundingCalculationInput.ExclusionReason.TreatmentSettings => BudgetUsageStatus.NotUsable,
                FundingCalculationInput.ExclusionReason.BudgetConditions => BudgetUsageStatus.ConditionNotMet,
                null => BudgetUsageStatus.CostNotCovered,
                _ => BudgetUsageStatus.Undefined
            };
        }
        else if (FundingCalculationInput != null && FundingCalculationOutput != null)
        {
            // It was funded.

            var allocation = FundingCalculationOutput.AllocationMatrix.SingleOrDefault(
                a => (a.Year, a.BudgetName, a.TreatmentName) == (year, budget, treatment));

            if (allocation != null)
            {
                return BudgetUsageStatus.CostCovered;
            }

            var exclusion = FundingCalculationInput.ExclusionsMatrix.SingleOrDefault(
                e => (e.BudgetName, e.TreatmentName) == (budget, treatment));

            return exclusion?.Reason switch
            {
                FundingCalculationInput.ExclusionReason.TreatmentSettings => BudgetUsageStatus.NotUsable,
                FundingCalculationInput.ExclusionReason.BudgetConditions => BudgetUsageStatus.ConditionNotMet,
                null => BudgetUsageStatus.NotNeeded,
                _ => BudgetUsageStatus.Undefined
            };
        }
        else
        {
            // Something isn't right.

            throw new Exception("Treatment consideration detail is missing both input and output for funding calculation.");
        }
    }
}
