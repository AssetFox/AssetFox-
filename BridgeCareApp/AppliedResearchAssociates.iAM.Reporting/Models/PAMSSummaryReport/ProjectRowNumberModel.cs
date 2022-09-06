using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport
{
    public class ProjectRowNumberModel
    {
        public Dictionary<string, int> TreatmentsCount { get; set; } = new Dictionary<string, int>();
    }
}
