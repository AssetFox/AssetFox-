using System.Collections.Generic;

namespace BridgeCareCore.Models.SummaryReport
{
    public class ProjectRowNumberModel
    {
        public Dictionary<string, int> TreatmentsCount { get; set; } = new Dictionary<string, int>();
    }
}
