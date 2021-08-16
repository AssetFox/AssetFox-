using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class RemainingLifeLimitLibraryEntity : LibraryEntity
    {
        public RemainingLifeLimitLibraryEntity()
        {
            RemainingLifeLimits = new HashSet<RemainingLifeLimitEntity>();
            RemainingLifeLimitLibrarySimulationJoins = new HashSet<RemainingLifeLimitLibrarySimulationEntity>();
        }

        public virtual ICollection<RemainingLifeLimitEntity> RemainingLifeLimits { get; set; }

        public virtual ICollection<RemainingLifeLimitLibrarySimulationEntity> RemainingLifeLimitLibrarySimulationJoins { get; set; }
    }
}
