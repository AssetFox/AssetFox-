using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport
{
    public class WorkSummaryByBudgetModel
    {
        public string BudgetName { get; set; }

        public List<YearsData> YearlyData { get; set; }
    }
}
