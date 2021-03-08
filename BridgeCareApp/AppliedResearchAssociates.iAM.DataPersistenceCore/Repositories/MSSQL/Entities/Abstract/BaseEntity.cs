using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract
{
    public abstract class BaseEntity
    {
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime LastModifiedDate { get; set; } = DateTime.Now;

        // TODO: default these to empty guids for now until BridgeCareCore is integrated with authentication
        public Guid CreatedBy { get; set; } = Guid.Empty;

        public Guid LastModifiedBy { get; set; } = Guid.Empty;
    }
}
