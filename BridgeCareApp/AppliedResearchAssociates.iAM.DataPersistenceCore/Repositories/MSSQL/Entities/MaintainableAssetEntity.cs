using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class MaintainableAssetEntity
    {
        [Key]
        public Guid Id { get; set; }

        public Guid NetworkId { get; set; }

        [ForeignKey("NetworkId")]
        public virtual NetworkEntity Network { get; set; }

        public virtual MaintainableAssetLocationEntity MaintainableAssetLocation { get; set; }

        public virtual ICollection<AttributeDatumEntity> AttributeData { get; set; }

        public virtual ICollection<AggregatedResultEntity> AggregatedResults { get; set; }
    }
}
