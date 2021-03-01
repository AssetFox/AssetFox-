using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCareCore.Models.SummaryReport
{
    public class WorkSummaryByBudgetModel
    {
        public string Budget { get; set; }
        public List<YearsData> YearlyData { get; set; }
    }
}
