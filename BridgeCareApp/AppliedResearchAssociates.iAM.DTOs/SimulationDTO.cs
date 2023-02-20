using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class SimulationDTO : BaseDTO
    {
        public string Name { get; set; }

        public Guid NetworkId { get; set;}

        public string NetworkName { get; set; }

        public string Owner { get; set; }

        public string Creator { get; set; }

        public bool NoTreatmentBeforeCommittedProjects { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public DateTime? LastRun { get; set; }

        public List<SimulationUserDTO> Users { get; set; } = new List<SimulationUserDTO>();

        public string Status { get; set; }

        public string ReportStatus { get; set; }

        public string RunTime { get; set; }
    }
}
