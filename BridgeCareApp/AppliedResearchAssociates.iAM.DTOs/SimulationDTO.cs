using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class SimulationDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid NetworkId { get; set;}

        public string NetworkName { get; set; }

        public string Owner { get; set; }

        public string Creator { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public DateTime? LastRun { get; set; }

        public List<SimulationUserDTO> Users { get; set; } = new List<SimulationUserDTO>();

        public string Status { get; set; }

        public string ReportStatus { get; set; }

        public string RunTime { get; set; }
    }



    // Temporary class just to get up and running;
    // TODO: refactored without inheritance?
    public class QueuedSimulationDTO : SimulationDTO
    {
        public DateTime QueueEntryTimestamp { get; set; }
        public DateTime? WorkStartedTimestamp { get; set; }
        public string QueueingUser { get; set; }
    }
}
