using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Reporting.Models.PAMSAuditReport
{
    public class PAMSDecisionDataModel
    {
        public string CRS { get; set; }

        public int AnalysisYear { get; set; }

        public List<double> CurrentAttributesValues { get; set; }

        public List<decimal> BudgetLevels { get; set; }

        public List<PAMSDecisionTreatment> DecisionsTreatments { get; set; }
        public List<PAMSDecisionAggregated> DecisionsAggregated { get; set; }
    }

    public class PAMSDecisionAggregated
    {
        public string Feasible { get; set; }
        public string IncludedBundles { get; set; }

        public double? CIImprovement { get; set; }

        public double Cost { get; set; }

        public double BCRatio { get; set; }

        public string Selected { get; set; }

        public decimal? AmountSpent { get; set; }

        public string BudgetsUsed { get; set; }

        public string BudgetUsageStatuses { get; set; }
    }


    public class PAMSDecisionTreatment
    {
        public string Feasible { get; set; }

        public double? CIImprovement { get; set; }

        public double Cost { get; set; }

        public double BCRatio { get; set; }

        public string Selected { get; set; }

        public decimal? AmountSpent { get; set; }

        public string BudgetsUsed { get; set; }

        public string BudgetUsageStatuses { get; set; }

        public string Superseded { get; set; }
    }
}
