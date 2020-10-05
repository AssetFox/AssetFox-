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
                throw new NullReferenceException("Cannot map null Location entity to Location domain");
            }

            if (entity.Discriminator == "LinearLocation")
            {
                return new LinearLocation(
                    entity.Route.ToDomain(),
                    entity.UniqueIdentifier,
                    entity.Start.Value,
                    entity.End.Value);
            }

            return new SectionLocation(entity.UniqueIdentifier);
        }

        public static LocationEntity ToEntity(this Location domain, Guid locationId)
        {
            if (domain == null)
            {
                throw new NullReferenceException("Cannot map null Location domain to Location entity");
            }

            if (domain is LinearLocation linearLocationDomain)
            {
                var entity = new LocationEntity
                {
                    Id = locationId,
                    Start = linearLocationDomain.Start,
                    End = linearLocationDomain.End,
                    Discriminator = "LinearLocation"
                };

                if (linearLocationDomain.Route != null)
                {
                    var routeId = Guid.NewGuid();
                    entity.RouteId = routeId;
                    entity.Route = linearLocationDomain.Route.ToEntity(routeId);
                }

                return entity;
            }

            return new LocationEntity
            {
                Id = locationId,
                UniqueIdentifier = domain.UniqueIdentifier,
                Discriminator = "SectionLocation"
            };
        }
    }
}
