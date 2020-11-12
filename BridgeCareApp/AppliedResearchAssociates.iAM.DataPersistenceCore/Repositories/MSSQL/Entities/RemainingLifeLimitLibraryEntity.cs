using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class RemainingLifeLimitLibraryEntity
    {
        public RemainingLifeLimitLibraryEntity()
        {
            RemainingLifeLimits = new HashSet<RemainingLifeLimitEntity>();
            RemainingLifeLimitLibrarySimulationJoins = new HashSet<RemainingLifeLimitLibrarySimulationEntity>();
        }

        public Guid Id { get; set; }

        public virtual ICollection<RemainingLifeLimitEntity> RemainingLifeLimits { get; set; }
        public virtual ICollection<RemainingLifeLimitLibrarySimulationEntity> RemainingLifeLimitLibrarySimulationJoins { get; set; }
    }
}
