using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public abstract class LocationEntity
    {
        [Key]
        public Guid Id { get; set; }

        public Guid SegmentId { get; set; }
        public string UniqueIdentifier { get; set; }
        [ForeignKey("SegmentId")]
        public virtual SegmentEntity Segment { get; set; }

        public virtual AttributeDatumEntity AttributeData { get; set; }
    }
}
