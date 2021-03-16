﻿using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class SimulationDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

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
}
