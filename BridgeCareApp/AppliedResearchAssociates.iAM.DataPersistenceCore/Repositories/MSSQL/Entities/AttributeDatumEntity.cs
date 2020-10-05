using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class AttributeDatumEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Discriminator { get; set; }
        public DateTime TimeStamp { get; set; }
        public double? NumericValue { get; set; }
        public string TextValue { get; set; }

        public Guid AttributeId { get; set; }
        [ForeignKey("AttributeId")]
        public virtual AttributeEntity Attribute { get; set; }

        public Guid LocationId { get; set; }
        [ForeignKey("LocationId")]
        public virtual LocationEntity Location { get; set; }

        public Guid SegmentId { get; set; }
        [ForeignKey("SegmentId")]
        public virtual SegmentEntity Segment { get; set; }
    }
}
