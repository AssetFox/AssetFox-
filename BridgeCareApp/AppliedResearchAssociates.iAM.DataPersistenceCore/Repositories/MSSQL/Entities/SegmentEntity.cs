using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class SegmentEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid NetworkId { get; set; }
        public Guid LocationId { get; set; }
        public string UniqueIdentifier { get; set; }

        [ForeignKey("NetworkId")]
        public virtual NetworkEntity Network { get; set; }

        public virtual ICollection<AttributeDatumEntity> AttributeData { get; set; }

        [ForeignKey("LocationId")]
        public virtual LocationEntity Location { get; set; }

        public virtual ICollection<AggregationResultEntity> AggregatedResults { get; set; }
    }
}
