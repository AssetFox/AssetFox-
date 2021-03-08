using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class SimulationAnalysisDetailDTO
    {
        public SimulationAnalysisDetailDTO() { }

        public SimulationAnalysisDetailDTO(SimulationAnalysisDetailEntity entity)
        {
            SimulationId = entity.SimulationId;
            LastRun = entity.LastRun;
            Status = entity.Status;
            RunTime = entity.RunTime;
        }

        public Guid SimulationId { get; set; }

        public DateTime? LastRun { get; set; }

        public string Status { get; set; }

        public string RunTime { get; set; }
    }
}
