using System;

namespace AppliedResearchAssociates.iAM.DataMiner
{
    public abstract class Location
    {

        public Location(Guid id, string uniqueIdentifier)
        {
            Id = id;
            UniqueIdentifier = uniqueIdentifier;
        }
        public Guid Id { get; }
        public string UniqueIdentifier { get; }

        public abstract bool MatchOn(Location location);
    }
}
