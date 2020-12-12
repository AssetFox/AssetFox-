using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract
{
    public abstract class AttributeValueHistoryEntity : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid SectionId { get; set; }

        public Guid AttributeId { get; set; }

        public virtual SectionEntity Section { get; set; }

        public virtual AttributeEntity Attribute { get; set; }
    }
}
