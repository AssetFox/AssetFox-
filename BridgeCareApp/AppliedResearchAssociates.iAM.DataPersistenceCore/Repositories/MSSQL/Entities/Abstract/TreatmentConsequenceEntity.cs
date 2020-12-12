using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract
{
    public abstract class TreatmentConsequenceEntity : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid AttributeId { get; set; }

        public string ChangeValue { get; set; }

        public virtual AttributeEntity Attribute { get; set; }
    }
}
