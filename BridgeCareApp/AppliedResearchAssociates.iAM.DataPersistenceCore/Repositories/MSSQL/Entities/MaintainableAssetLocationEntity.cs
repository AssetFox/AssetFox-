using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class MaintainableAssetLocationEntity : LocationEntity
    {
        public MaintainableAssetLocationEntity(Guid id, string discriminator, string locationIdentifier) : base(id, discriminator, locationIdentifier) { }

        public Guid MaintainableAssetId { get; set; }

        [ForeignKey("MaintainableAssetId")]
        public virtual MaintainableAssetEntity MaintainableAsset { get; set; }
    }
}
