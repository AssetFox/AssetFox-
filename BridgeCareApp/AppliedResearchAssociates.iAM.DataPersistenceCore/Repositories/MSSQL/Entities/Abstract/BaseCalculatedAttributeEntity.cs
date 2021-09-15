using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract
{
    public abstract class BaseCalculatedAttributeEntity : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid AttributeId { get; set; }

        public int CalculationTiming { get; set; }

        public virtual AttributeEntity Attribute { get; set; }
    }
}
