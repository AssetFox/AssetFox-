using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class NetworkRollupDetailEntity : BaseEntity
    {
        public Guid NetworkId { get; set; }
        public string Status { get; set; }
        public virtual NetworkEntity Network { get; set; }
    }
}
