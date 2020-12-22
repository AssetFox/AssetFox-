using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class SimulationAnalysisDetailEntity : BaseEntity
    {
        public Guid SimulationId { get; set; }
        public DateTime LastRun { get; set; }
        public string Status { get; set; }
        public string RunTime { get; set; }
        public virtual SimulationEntity Simulation { get; set; }
    }
}
