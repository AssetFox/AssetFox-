﻿using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSAuditReport
{
    public class DecisionDataModel
    {
        public double BRKey { get; set; }

        public int AnalysisYear { get; set; }

        public List<double> CurrentAttributesValues { get; set; }

        public List<decimal> BudgetLevels { get; set; }

        public List<DecisionTreatment> DecisionsTreatments { get; set; }
    }

    public class DecisionTreatment
    {
        public string Feasible { get; set; }

        public double? CIImprovement { get; set; }

        public string BudgetPriorityLevel { get; set; }

        public double Cost { get; set; }

        public double BCRatio { get; set; }

        public double Benefit { get; set; }

        public string Selected { get; set; }

        public decimal? AmountSpent { get; set; }

        public string BudgetsUsed { get; set; }

        public string BudgetUsageStatuses { get; set ; }

        public string Superseded { get; set; }
    }
}
