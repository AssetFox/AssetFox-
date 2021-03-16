using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class SimulationUserEntity : BaseEntity
    {
        public Guid SimulationId { get; set; }
        public Guid UserId { get; set; }
        public bool CanModify { get; set; }
        public bool IsOwner { get; set; }
        public virtual SimulationEntity Simulation { get; set; }
        public virtual UserEntity User { get; set; }
    }
}
