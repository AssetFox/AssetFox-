using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class AggregationResultEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Discriminator { get; set; }

        public int Year { get; set; }

        public string TextValue { get; set; }

        public double? NumericValue { get; set; }

        public Guid SegmentId { get; set; }

        [ForeignKey("SegmentId")]
        public virtual SegmentEntity Segment { get; set; }

        public Guid AttributeId { get; set; }

        [ForeignKey("AttributeId")]
        public virtual AttributeEntity Attribute { get; set; }
    }
}
