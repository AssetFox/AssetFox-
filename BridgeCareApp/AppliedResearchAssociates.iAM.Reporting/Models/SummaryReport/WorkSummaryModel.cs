using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport
{
    public class WorkSummaryModel
    {
        public List<double> PreviousYearInitialMinC { get; set; }

        public Dictionary<int, (int on, int off)> PoorOnOffCount { get; set; }

        public ParametersModel ParametersModel { get; set; }

        public Dictionary<int, Dictionary<string, int>> BpnPoorOnPerYear { get; set; }
        public Dictionary<int, int> NhsPoorOnPerYear { get; set; }
        public Dictionary<int, int> NonNhsPoorOnPerYear { get; set; }

        public decimal AnnualizedAmount { get; set; }
    }
}
