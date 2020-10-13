using System;

namespace AppliedResearchAssociates.iAM.DataMiner
{
    public class SectionLocation : Location
    {
        public SectionLocation(Guid id, string uniqueIdentifier) : base(id, uniqueIdentifier)
        {
        }

        public override bool MatchOn(Location location) =>
            location is SectionLocation sectionLocation && sectionLocation.UniqueIdentifier == UniqueIdentifier;
    }
}
