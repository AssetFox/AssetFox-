using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class SimulationLogDTO
    {
        public Guid Id { get; set; }
        public Guid SimulationId { get; set; }
        public int Status { get; set; }
        public DateTime TimeStamp { get; set; }
        public int Subject { get; set; }
        public string Message { get; set; }
    }
}
