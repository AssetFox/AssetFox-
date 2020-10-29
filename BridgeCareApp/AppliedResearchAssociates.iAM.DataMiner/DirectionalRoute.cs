namespace AppliedResearchAssociates.iAM.DataMiner
{
    public class DirectionalRoute : Route
    {
        public Direction Direction { get; }

        public DirectionalRoute(string uniqueIdentifier, Direction direction) : base(uniqueIdentifier)
        {
            Direction = direction;
        }

        internal override bool MatchOn(Route route)
        {
            var directionalRoute = (DirectionalRoute)route;
            return ((LocationIdentifier == directionalRoute.LocationIdentifier &&
                Direction == directionalRoute.Direction) ||
                LocationIdentifier == directionalRoute.LocationIdentifier);
        }
    }
}
