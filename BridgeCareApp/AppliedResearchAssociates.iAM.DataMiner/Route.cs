namespace AppliedResearchAssociates.iAM.DataMiner
{
    public abstract class Route
    {
        public string LocationIdentifier { get; }

        public Route(string uniqueIdentifier) => LocationIdentifier = uniqueIdentifier;

        // Determines if two routes match in a comparison.
        public abstract bool MatchOn(Route route);
    }
}
