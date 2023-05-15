using System;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    /// <summary>
    /// .
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

        /// <summary>
        /// .
        /// </summary>
        public string CashFlowRuleName { get; }

        /// <summary>
        /// .
        /// </summary>
        public ReasonAgainstCashFlow ReasonAgainstCashFlow { get; set; }

        internal CashFlowConsiderationDetail(CashFlowConsiderationDetail original)
        {
            CashFlowRuleName = original.CashFlowRuleName;
            ReasonAgainstCashFlow = original.ReasonAgainstCashFlow;
        }
    }
}
