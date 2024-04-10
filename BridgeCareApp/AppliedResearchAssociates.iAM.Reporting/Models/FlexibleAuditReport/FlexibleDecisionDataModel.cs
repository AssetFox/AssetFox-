using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.Reporting.Models.FlexibleAuditReport
{
    public class FlexibleDecisionDataModel
    {
        public string CRS { get; set; }

        public int AnalysisYear { get; set; }

        public List<double> CurrentAttributesValues { get; set; }

        public List<decimal> BudgetLevels { get; set; }

        public List<FlexibleDecisionTreatment> DecisionsTreatments { get; set; }
        public List<FlexibleDecisionAggregated> DecisionsAggregated { get; set; }
    }

    public class FlexibleDecisionAggregated
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


    public class FlexibleDecisionTreatment
    {
        public string Feasible { get; set; }

        public double? CIImprovement { get; set; }

        public double Cost { get; set; }

        public double BCRatio { get; set; }

        public string Selected { get; set; }

        public decimal? AmountSpent { get; set; }

        public string BudgetsUsed { get; set; }

        public string BudgetUsageStatuses { get; set; }
    }
}
