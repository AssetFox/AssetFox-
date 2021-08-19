using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract
{
    public abstract class BaseRemainingLifeLimitEntity : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid AttributeId { get; set; }
        public virtual AttributeEntity Attribute { get; set; }
    }
}
