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

        public int? BudgetPriorityLevel { get; set; }

        public List<BudgetDetail> Budgets { get; } = new List<BudgetDetail>();

        public List<CashFlowConsiderationDetail> CashFlowConsiderations { get; } = new List<CashFlowConsiderationDetail>();

        public string TreatmentName { get; }

        internal TreatmentConsiderationDetail(TreatmentConsiderationDetail original)
        {
            TreatmentName = original.TreatmentName;
            Budgets.AddRange(original.Budgets.Select(_ => new BudgetDetail(_)));
            CashFlowConsiderations.AddRange(original.CashFlowConsiderations.Select(_ => new CashFlowConsiderationDetail(_)));

            BudgetPriorityLevel = original.BudgetPriorityLevel;
        }
    }
}
