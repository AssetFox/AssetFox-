using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings
{
    public static class LocationEntityToLocation
    {
        public static Location CreateFromEntity(LocationEntity locationEntity)
        {
            if (locationEntity is LinearLocationEntity linearLocationEntity)
            {
                var route = linearLocationEntity.Route is DirectionalRouteEntity directionalRouteEntity
                    ? new DirectionalRoute(directionalRouteEntity.UniqueIdentifier, directionalRouteEntity.Direction)
                    : (Route)(linearLocationEntity.Route is RouteEntity simpleRouteEntity
                        ? new SimpleRoute(simpleRouteEntity.UniqueIdentifier)
                        : throw new InvalidOperationException());
                return new LinearLocation(route, linearLocationEntity.UniqueIdentifier, linearLocationEntity.Start, linearLocationEntity.End);
            }
            else
            {
                return locationEntity is SectionLocationEntity sectionLocationEntity
                    ? new SectionLocation(sectionLocationEntity.UniqueIdentifier)
                    : throw new InvalidOperationException("Entity is not of an expressable type.");
            }
        }
    }
}
