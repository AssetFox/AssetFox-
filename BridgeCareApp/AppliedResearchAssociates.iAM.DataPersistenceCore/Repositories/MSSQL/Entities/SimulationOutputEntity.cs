using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class SimulationOutputEntity : BaseEntity
    {
        public string Output { get; set; }

        public Guid SimulationId { get; set; }

        public virtual SimulationEntity Simulation { get; set; }
    }
}
