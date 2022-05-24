using System;
using AppliedResearchAssociates.iAM.Data.Attributes;

namespace AppliedResearchAssociates.iAM.Data
{
    public static class LocationBuilder
    {
        public static Location CreateLocation(
            string locationIdentifier,
            double? start = null,
            double? end = null,
            Direction? direction = null,
            string wellKnownText = null)
        {
            if (locationIdentifier != null && start != null && end != null && direction == null)
            {
                // Linear route data with no defined direction
                return new LinearLocation(Guid.NewGuid(), new SimpleRoute(locationIdentifier), locationIdentifier, start.Value, end.Value);
            }

            if (start != null && end != null && direction != null && locationIdentifier != null)
            {
                // Linear route data with a defined direction
                return new LinearLocation(Guid.NewGuid(), new DirectionalRoute(locationIdentifier, direction.Value), locationIdentifier, start.Value, end.Value);
            }

            if (locationIdentifier != null && wellKnownText != null && start == null && end == null)
            {
                return new GisLocation(Guid.NewGuid(), wellKnownText, locationIdentifier);
            }

            if (start == null && end == null && wellKnownText == null && locationIdentifier != null)
            {
                return new SectionLocation(Guid.NewGuid(), locationIdentifier);
            }

            throw new InvalidOperationException("Cannot determine location type from provided inputs.");
        }
    }
}
