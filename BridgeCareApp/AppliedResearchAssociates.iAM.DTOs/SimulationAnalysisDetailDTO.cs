using System;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class SimulationAnalysisDetailDTO
    {
        public Guid SimulationId { get; set; }

        public DateTime? LastRun { get; set; }

        public string Status { get; set; }

        public string ReportType { get; set; }

        public string RunTime { get; set; }
    }
}
