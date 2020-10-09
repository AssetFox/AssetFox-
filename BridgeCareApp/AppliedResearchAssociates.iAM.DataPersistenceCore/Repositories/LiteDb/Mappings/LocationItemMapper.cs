using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.LiteDb.Mappings
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
                    entity.Start ?? 0,
                    entity.End ?? 0);
            }

            if (entity.Discriminator == "SectionLocation")
            {
                return new SectionLocation(entity.UniqueIdentifier);
            }

            throw new InvalidOperationException("Cannot determine Location entity type");
        }

        public static LocationEntity ToEntity(this Location domain)
        {
            if (domain == null)
            {
                throw new NullReferenceException("Cannot map null Location domain to Location entity");
            }

            if (string.IsNullOrEmpty(domain.UniqueIdentifier))
            {
                throw new InvalidOperationException("Location has no unique identifier");
            }

            if (domain is LinearLocation linearLocationDomain)
            {
                var entity = new LocationEntity
                {
                    Id = Guid.NewGuid(),
                    Start = linearLocationDomain.Start,
                    End = linearLocationDomain.End,
                    Discriminator = "LinearLocation"
                };

                if (linearLocationDomain.Route != null)
                {
                    var routeEntity = linearLocationDomain.Route.ToEntity();

                    entity.RouteId = routeEntity.Id;
                    entity.Route = routeEntity;
                }

                return entity;
            }

            return new LocationEntity
            {
                Id = Guid.NewGuid(),
                UniqueIdentifier = domain.UniqueIdentifier,
                Discriminator = "SectionLocation"
            };
        }
    }
}
