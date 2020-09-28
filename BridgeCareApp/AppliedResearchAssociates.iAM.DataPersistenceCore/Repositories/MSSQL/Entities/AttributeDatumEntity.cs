using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public abstract class AttributeDatumEntity
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public Guid SegmentId { get; set; }

        [ForeignKey("SegmentId")]
        public virtual SegmentEntity Segment { get; set; }

        public virtual LocationEntity Location { get; set; }

        public virtual AttributeEntity Attribute { get; set; }
    }
}
