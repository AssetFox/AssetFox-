using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Analysis
{
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

        public List<BudgetDetail> Budgets { get; } = new List<BudgetDetail>();

        public string CashFlowRuleName { get; set; }

        public ReasonAgainstCashFlow ReasonAgainstCashFlow { get; set; }

        public string TreatmentName { get; }

        internal TreatmentConsiderationDetail(TreatmentConsiderationDetail original)
        {
            TreatmentName = original.TreatmentName;
            Budgets.AddRange(original.Budgets.Select(_ => new BudgetDetail(_)));

            CashFlowRuleName = original.CashFlowRuleName;
            ReasonAgainstCashFlow = original.ReasonAgainstCashFlow;
        }
    }
}
