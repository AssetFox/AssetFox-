using System.Collections.Generic;

namespace BridgeCareCore.Models.SummaryReport
{
    public class WorkSummaryByBudgetModel
    {
        public string Budget { get; set; }

        public List<YearsData> YearlyData { get; set; }
    }
}
