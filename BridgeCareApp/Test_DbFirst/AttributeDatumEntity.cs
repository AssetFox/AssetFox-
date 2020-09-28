using System;
using System.Collections.Generic;

namespace Test_DbFirst
{
    public partial class AttributeDatumEntity
    {
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public Guid SegmentId { get; set; }
        public Guid LocationId { get; set; }
        public Guid AttributeId { get; set; }
        public string Discriminator { get; set; }
        public double? Value { get; set; }
        public string TextAttributeDatumEntityValue { get; set; }

        public virtual Attributes Attribute { get; set; }
        public virtual Locations Location { get; set; }
        public virtual Segments Segment { get; set; }
    }
}
