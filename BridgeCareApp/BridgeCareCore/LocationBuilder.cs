using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
            if(uniqueIdentifier != null && start != null && end != null && direction == null)
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
            else if(start == null && end == null && wellKnownText == null && uniqueIdentifier != null)
            {
                return new SectionLocation(uniqueIdentifier);
            }
            else
            {
                throw new InvalidOperationException("Cannot determine location type from provided inputs.");
            }
            
        }

        public static Location CreateFromEntity(LocationEntity locationEntity)
        {
            if (locationEntity is LinearLocationEntity linearLocationEntity)
            {
                Route route;
                if (linearLocationEntity.Route is DirectionalRouteEntity directionalRouteEntity)
                {
                    route = new DirectionalRoute(directionalRouteEntity.UniqueIdentifier, directionalRouteEntity.Direction);
                }
                else if (linearLocationEntity.Route is RouteEntity simpleRouteEntity)
                {
                    route = new SimpleRoute(simpleRouteEntity.UniqueIdentifier);
                }
                else
                {
                    throw new InvalidOperationException();
                }
                return new LinearLocation(route, linearLocationEntity.UniqueIdentifier, linearLocationEntity.Start, linearLocationEntity.End);
            }
            else if(locationEntity is SectionLocationEntity sectionLocationEntity)
            {
                return new SectionLocation(sectionLocationEntity.UniqueIdentifier);
            }
            else
            {
                throw new InvalidOperationException("Entity is not of an expressable type.");
            }
        }
    }
}
