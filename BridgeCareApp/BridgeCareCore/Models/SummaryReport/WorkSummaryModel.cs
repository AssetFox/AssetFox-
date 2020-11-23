using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCareCore.Models.SummaryReport
{
    public class WorkSummaryModel
    {
        public List<double> PreviousYearInitialMinC { get; set; }
        public Dictionary<int, (int on, int off)> PoorOnOffCount { get; set; }
    }
}
