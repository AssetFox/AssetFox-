using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistence.Models
{
    public class AggregateDataSegment
    {
        public AggregateDataSegment(Segment segment) => Segment = segment;
        public Segment Segment { get; set; }
    }
}
