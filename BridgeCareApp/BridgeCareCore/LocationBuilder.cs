using System;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace BridgeCareCore
{
    public static class LocationBuilder
    {
        public static Location CreateLocation(
            string uniqueIdentifier,
            double? start = null,
            double? end = null,
            Direction? direction = null,
            string wellKnownText = null)
        {
            if (uniqueIdentifier != null && start != null && end != null && direction == null)
            {
                // Linear route data with no defined direction
                return new LinearLocation(new SimpleRoute(uniqueIdentifier), uniqueIdentifier, start.Value, end.Value);
            }
            else if (start != null & end != null && direction != null && uniqueIdentifier != null)
            {
                // Linear route data with a defined direction
                return new LinearLocation(new DirectionalRoute(uniqueIdentifier, direction.Value), uniqueIdentifier, start.Value, end.Value);
            }
            else if (uniqueIdentifier != null && wellKnownText != null && start == null && end == null)
            {
                return new GisLocation(wellKnownText, uniqueIdentifier);
            }
            else if (start == null && end == null && wellKnownText == null && uniqueIdentifier != null)
            {
                return new SectionLocation(uniqueIdentifier);
            }
            else
            {
                throw new InvalidOperationException("Cannot determine location type from provided inputs.");
            }
        }
    }
}
