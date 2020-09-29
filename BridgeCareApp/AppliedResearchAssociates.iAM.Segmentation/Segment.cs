using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Segmentation
{
    public class Segment
    {
        public Segment(Location segmentLocation)
        {
            Location = segmentLocation;
        }

        public Location Location { get; }
    }
}
