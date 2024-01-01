using System;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

/// <summary>
///     The logic used to determine if a specific treatment should be funded through the cash flow
///     system which allows a treatment to be funded over multiple years.
/// </summary>
public sealed class CashFlowConsiderationDetail
{
    public CashFlowConsiderationDetail(string cashFlowRuleName)
    {
        if (string.IsNullOrWhiteSpace(cashFlowRuleName))
        {
            throw new ArgumentException("Cash flow rule name is blank.", nameof(cashFlowRuleName));
        }

        CashFlowRuleName = cashFlowRuleName;
    }

    internal CashFlowConsiderationDetail(CashFlowConsiderationDetail original)
    {
        CashFlowRuleName = original.CashFlowRuleName;
        ReasonAgainstCashFlow = original.ReasonAgainstCashFlow;

        FundingCalculationInputSupplement = new(original.FundingCalculationInputSupplement);
    }

    /// <summary>
    ///     Name of the cash flow rule being considered.
    /// </summary>
    public string CashFlowRuleName { get; }

    public FundingCalculationInput.CashFlowSupplement FundingCalculationInputSupplement { get; set; }

    /// <summary>
    ///     Result of the decision regarding this specific treatment and cash flow rule.
    /// </summary>
    public ReasonAgainstCashFlow ReasonAgainstCashFlow { get; set; }
}
