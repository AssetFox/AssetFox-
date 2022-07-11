using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class NetworkAttributeEntity : BaseEntity
    {
        public Guid NetworkId { get; set; }
        public Guid AttributeId { get; set; }
        public virtual AttributeEntity Attribute { get; set; }
        public virtual NetworkEntity Network { get; set; }
    }
}
