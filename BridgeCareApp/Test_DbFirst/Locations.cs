using System;
using System.Collections.Generic;

namespace Test_DbFirst
{
    public partial class Locations
    {
        public Guid Id { get; set; }
        public Guid SegmentId { get; set; }
        public string Discriminator { get; set; }
        public double? Start { get; set; }
        public double? End { get; set; }

        public virtual Segments Segment { get; set; }
        public virtual AttributeDatumEntity AttributeDatumEntity { get; set; }
        public virtual Routes Routes { get; set; }
    }
}
