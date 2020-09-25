using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataPersistence.Models
{
    public class Segment
    {
        public Segment(Location location, IAttributeDatum segmentationAttributeDatum)
        {
            Location = location;
            SegmentationAttributeDatum = segmentationAttributeDatum;
        }

        public Location Location { get; }
        public IAttributeDatum SegmentationAttributeDatum { get; }
    }
}
