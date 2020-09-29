namespace AppliedResearchAssociates.iAM.DataMiner
{
    public abstract class Route
    {
        public string UniqueIdentifier { get; }

        public Route(string uniqueIdentifier) => UniqueIdentifier = uniqueIdentifier;

        // Determines if two routes match in a comparison.
        internal abstract bool MatchOn(Route route);
    }
}
