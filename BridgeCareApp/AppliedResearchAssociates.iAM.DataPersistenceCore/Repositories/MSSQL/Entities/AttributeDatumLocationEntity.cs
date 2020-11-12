using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class AttributeDatumLocationEntity : LocationEntity
    {
        public AttributeDatumLocationEntity(Guid id, string discriminator, string locationIdentifier) : base(id,
            discriminator, locationIdentifier) { }

        public Guid AttributeDatumId { get; set; }

        public virtual AttributeDatumEntity AttributeDatum { get; set; }
    }
}
