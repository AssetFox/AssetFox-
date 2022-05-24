using System;

namespace AppliedResearchAssociates.iAM.Data
{
    public class SectionLocation : Location
    {
        public SectionLocation(Guid id, string locationIdentifier) : base(id, locationIdentifier)
        {
        }

        public override bool MatchOn(Location location) =>
            location is SectionLocation sectionLocation && sectionLocation.LocationIdentifier == LocationIdentifier;
    }
}
