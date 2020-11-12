using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class RemainingLifeLimitEntity
    {
        public Guid Id { get; set; }
        public Guid RemainingLifeLimitLibraryId { get; set; }

        public virtual RemainingLifeLimitLibraryEntity RemainingLifeLimitLibrary { get; set; }
        public virtual CriterionLibraryRemainingLifeLimitEntity CriterionLibraryRemainingLifeLimitJoin { get; set; }
    }
}
