namespace AppliedResearchAssociates.iAM.Data
{
    public class DirectionalRoute : Route
    {
        public Direction Direction { get; }

        public DirectionalRoute(string uniqueIdentifier, Direction direction) : base(uniqueIdentifier)
        {
            Direction = direction;
        }

        public override bool MatchOn(Route route)
        {
            var directionalRoute = (DirectionalRoute)route;
            return ((LocationIdentifier == directionalRoute.LocationIdentifier &&
                Direction == directionalRoute.Direction) ||
                LocationIdentifier == directionalRoute.LocationIdentifier);
        }
    }
}
