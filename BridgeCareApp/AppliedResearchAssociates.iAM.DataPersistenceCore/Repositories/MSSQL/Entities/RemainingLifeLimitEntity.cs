using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class RemainingLifeLimitEntity
    {
        public Guid Id { get; set; }

        public Guid AttributeId { get; set; }

        public Guid RemainingLifeLimitLibraryId { get; set; }

        public double Value { get; set; }

        public virtual AttributeEntity Attribute { get; set; }

        public virtual RemainingLifeLimitLibraryEntity RemainingLifeLimitLibrary { get; set; }

        public virtual CriterionLibraryRemainingLifeLimitEntity CriterionLibraryRemainingLifeLimitJoin { get; set; }
    }
}
