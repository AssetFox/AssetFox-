using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CommittedProjectLocationEntity : LocationEntity
    {
        public CommittedProjectLocationEntity(Guid id, string discriminator, string locationIdentifier)
            : base(id, discriminator, locationIdentifier) { }

        public CommittedProjectLocationEntity() : base() { }

        public Guid CommittedProjectId { get; set; }

        public virtual CommittedProjectEntity CommittedProject { get; set; }
    }
}
