using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class MaintainableAssetLocationEntity : LocationEntity
    {
        public MaintainableAssetLocationEntity() {}

        public MaintainableAssetLocationEntity(Guid id, string discriminator, string locationIdentifier) : base(id,
            discriminator, locationIdentifier)
        { }

        public Guid MaintainableAssetId { get; set; }

        public virtual MaintainableAssetEntity MaintainableAsset { get; set; }
    }
}
