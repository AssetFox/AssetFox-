using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class SimulationLogEntity: BaseEntity
    {
        public Guid Id { get; set; }
        public Guid SimulationId { get; set; }
        public int Status { get; set; }
        public int Subject { get; set; }
        public string Message { get; set; }
        public virtual SimulationEntity Simulation { get; set; }
    }
}
