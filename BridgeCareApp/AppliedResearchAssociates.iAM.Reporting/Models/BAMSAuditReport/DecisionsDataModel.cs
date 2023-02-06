using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSAuditReport
{
    public class DecisionsDataModel
    {
        public double BRKey { get; set; }

        public int AnalysisYear { get; set; }

        public List<double> CurrentAttributesValues { get; set; } // Calcuation for CI/value of resp benefit attr, note CI coln no.

        public List<decimal> BudgetLevels { get; set; } // TODO check later if double stored in budgetlevel values

        public List<DecisionTreatment> decisionsTreatments { get; set; }
    }

    public class DecisionTreatment
    {
        public bool Feasiable { get; set; }

        public double CIImprovement { get; set; } // Calculation = 10 - CI

        public double Cost { get; set; }

        public double BCRatio { get; set; }

        public bool Selected { get; set; }

        public decimal? AmountSpent { get; set; }

        public string BudgetsUsed { get; set; }

        public string RejectionReason { get; set ; }
    }
}
