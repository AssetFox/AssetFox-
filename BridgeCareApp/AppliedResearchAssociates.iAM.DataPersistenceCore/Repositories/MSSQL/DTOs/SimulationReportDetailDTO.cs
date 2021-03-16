using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class SimulationReportDetailDTO
    {
        public Guid SimulationId{ get; set; }
        public string Status { get; set; }
    }
}
