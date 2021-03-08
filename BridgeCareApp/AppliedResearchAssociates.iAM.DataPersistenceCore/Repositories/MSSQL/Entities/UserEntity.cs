using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class UserEntity : BaseEntity
    {
        public UserEntity()
        {
            SimulationUserJoins = new HashSet<SimulationUserEntity>();
        }

        public Guid Id { get; set; }
        public string Username { get; set; }
        public bool HasInventoryAccess { get; set; }
        public virtual CriterionLibraryUserEntity CriterionLibraryUserJoin { get; set; }
        public ICollection<SimulationUserEntity> SimulationUserJoins { get; set; }
    }
}
