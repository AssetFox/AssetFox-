using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings
{
    public static class LocationItemMapper
    {
        public static Location ToDomain(this LocationEntity entity)
        {
            if (entity == null)
            {
                throw new NullReferenceException("LocationEntity object is null");
            }

            if (entity is LinearLocationEntity linearLocationEntity)
            {
                return new LinearLocation(
                    linearLocationEntity.Route.ToDomain(),
                    linearLocationEntity.UniqueIdentifier,
                    linearLocationEntity.Start,
                    linearLocationEntity.End);
            }

            return new SectionLocation(entity.UniqueIdentifier);
        }

        public static LocationEntity ToEntity(this Location domain, Guid locationId)
        {
            if (domain == null)
            {
                throw new NullReferenceException("Location object is null");
            }

            if (domain is LinearLocation linearLocationDomain)
            {
                var routeId = Guid.NewGuid();

                return new LinearLocationEntity
                {
                    Id = locationId,
                    Start = linearLocationDomain.Start,
                    End = linearLocationDomain.End,
                    RouteId = routeId,
                    Route = linearLocationDomain.Route.ToEntity(routeId)
                };
            }

            return new SectionLocationEntity
            {
                Id = locationId,
                UniqueIdentifier = domain.UniqueIdentifier
            };
        }
    }
}
