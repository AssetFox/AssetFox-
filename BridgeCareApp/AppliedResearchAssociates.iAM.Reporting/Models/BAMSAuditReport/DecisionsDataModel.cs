using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSAuditReport
{
    public class DecisionsDataModel
    {
        public int BRKey { get; set; }

        public int AnalysisYear { get; set; }

        public List<string> CurrentAttributesValues { get; set; }

        public List<double> BudgetLevels { get; set; } // TODO check later if double stored in budgetlevel values

        public List<DecisionTreatment> decisionsTreatments { get; set; }
    }

    public class DecisionTreatment
    {
        public bool Feasiable { get; set; }

        public double CIImprovement { get; set; } // TODO ask what should this be

        public double Cost { get; set; }

        public double BCRatio { get; set; }

        public bool Selected { get; set; }

        public double? AmountSpent { get; set; }

        public string BudgetsUsed { get; set; }

        public string RejectionReason { get; set ; }
    }
}
