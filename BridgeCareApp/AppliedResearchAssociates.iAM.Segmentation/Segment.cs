using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Segmentation
{
    public class Segment
    {
        public Segment(Location segmentLocation, IAttributeDatum segmentationAttributeDatum)
        {
            Location = segmentLocation;
            SegmentationAttributeDatum = segmentationAttributeDatum;
        }

        public Location Location { get; }

        public IAttributeDatum SegmentationAttributeDatum { get; }
    }
}
