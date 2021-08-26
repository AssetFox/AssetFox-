using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.RemainingLifeLimit
{
    public class RemainingLifeLimitLibraryEntity : LibraryEntity
    {
        public RemainingLifeLimitLibraryEntity()
        {
            RemainingLifeLimits = new HashSet<RemainingLifeLimitEntity>();
        }

        public virtual ICollection<RemainingLifeLimitEntity> RemainingLifeLimits { get; set; }
    }
}
