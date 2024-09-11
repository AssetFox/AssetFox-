using System;
using System.ComponentModel.DataAnnotations;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class SimulationReportDetailDTO
    {
        public Guid SimulationId { get; set; }

        public string Status { get; set; }

        public string ReportType { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
