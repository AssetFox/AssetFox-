using System;

namespace AppliedResearchAssociates.iAM.DataMiner
{
    public class LinearLocation : Location
    {
        public Route Route { get; }

        public double Start { get; }

        public double End { get; }

        // The uniqueIdentifier can really be any uniquely identifiable string of characters.
        // (ROUTE-BMP-EMP-DIR for example).
        public LinearLocation(Guid id, Route route, string uniqueIdentifier, double start, double end) : base(id, uniqueIdentifier)
        {
            Route = route;
            Start = start;
            End = end;
        }

        public override bool MatchOn(Location location)
        {
            return location is LinearLocation linearLocation
                ? linearLocation.Start <= Start &&
                    linearLocation.End > End &&
                    linearLocation.Route.MatchOn(Route)
                : false;
        }
    }
}
