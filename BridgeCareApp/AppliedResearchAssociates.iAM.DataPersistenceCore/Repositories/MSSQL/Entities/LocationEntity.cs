using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class LocationEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Discriminator { get; set; }

        [Required]
        public string UniqueIdentifier { get; set; }

        public double? Start { get; set; }

        public double? End { get; set; }

        public Guid? RouteId { get; set; }

        [ForeignKey("RouteId")]
        public virtual RouteEntity Route { get; set; }

        public virtual MaintainableAssetEntity MaintainableAsset { get; set; }

        public virtual ICollection<AttributeDatumEntity> AttributeData { get; set; }
    }
}
