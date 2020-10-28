using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Interfaces;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class MaintainableAssetLocationEntity : LocationEntity
    {
        public MaintainableAssetLocationEntity(Guid id, string discriminator, string uniqueIdentifier) : base(id, discriminator, uniqueIdentifier) { }

        public Guid MaintainableAssetId { get; set; }

        [ForeignKey("MaintainableAssetId")]
        public virtual MaintainableAssetEntity MaintainableAsset { get; set; }
    }
}
