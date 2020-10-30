using System;

namespace AppliedResearchAssociates.iAM.DataMiner
{
    public abstract class Location
    {

        public Location(Guid id, string locationIdentifier)
        {
            Id = id;
            LocationIdentifier = locationIdentifier;
        }
        public Guid Id { get; }
        public string LocationIdentifier { get; }

        public abstract bool MatchOn(Location location);
    }
}
