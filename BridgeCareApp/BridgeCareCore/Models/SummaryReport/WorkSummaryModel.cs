using System.Collections.Generic;

namespace BridgeCareCore.Models.SummaryReport
{
    public class WorkSummaryModel
    {
        public List<double> PreviousYearInitialMinC { get; set; }
        public Dictionary<int, (int on, int off)> PoorOnOffCount { get; set; }
        public ParametersModel ParametersModel { get; set; }
    }
}
