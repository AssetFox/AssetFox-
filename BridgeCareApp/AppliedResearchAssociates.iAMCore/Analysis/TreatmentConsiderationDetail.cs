﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAMCore.Analysis
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

        public List<BudgetUsageDetail> BudgetUsages { get; } = new List<BudgetUsageDetail>();

        public List<CashFlowConsiderationDetail> CashFlowConsiderations { get; } = new List<CashFlowConsiderationDetail>();

        public string TreatmentName { get; }

        internal TreatmentConsiderationDetail(TreatmentConsiderationDetail original)
        {
            TreatmentName = original.TreatmentName;
            BudgetUsages.AddRange(original.BudgetUsages.Select(_ => new BudgetUsageDetail(_)));
            CashFlowConsiderations.AddRange(original.CashFlowConsiderations.Select(_ => new CashFlowConsiderationDetail(_)));

            BudgetPriorityLevel = original.BudgetPriorityLevel;
        }
    }
}
