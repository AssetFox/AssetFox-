using System.Collections.Generic;

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
        public string Feasiable { get; set; }

        public double? CIImprovement { get; set; }

        public double Cost { get; set; }

        public double BCRatio { get; set; }

        public string Selected { get; set; }

        public decimal? AmountSpent { get; set; }

        public string BudgetsUsed { get; set; }

        public string RejectionReason { get; set ; }        
    }
}
