using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.RemainingLifeLimit
{
    public class CriterionLibraryRemainingLifeLimitEntity : BaseCriterionLibraryJoinEntity
    {
        public Guid RemainingLifeLimitId { get; set; }

        public virtual RemainingLifeLimitEntity RemainingLifeLimit { get; set; }
    }
}
