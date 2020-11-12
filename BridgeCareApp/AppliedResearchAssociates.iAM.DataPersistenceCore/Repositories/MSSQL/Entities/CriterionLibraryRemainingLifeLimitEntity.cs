using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CriterionLibraryRemainingLifeLimitEntity
    {
        public Guid CriterionLibraryId { get; set; }
        public Guid RemainingLifeLimitId { get; set; }

        public virtual CriterionLibraryEntity CriterionLibrary { get; set; }
        public virtual RemainingLifeLimitEntity RemainingLifeLimit { get; set; }
    }
}
