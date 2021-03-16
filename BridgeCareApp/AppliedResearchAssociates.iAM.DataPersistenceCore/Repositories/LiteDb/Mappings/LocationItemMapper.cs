using System;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Mappings
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
                    entity.Id,
                    entity.RouteEntity.ToDomain(),
                    entity.LocationIdentifier,
                    entity.Start ?? 0,
                    entity.End ?? 0);
            }

            if (entity.Discriminator == "SectionLocation")
            {
                return new SectionLocation(entity.Id, entity.LocationIdentifier);
            }

            throw new InvalidOperationException("Cannot determine Location entity type");
        }

        public static LocationEntity ToEntity(this Location domain)
        {
            if (domain == null)
            {
                throw new NullReferenceException("Cannot map null Location domain to Location entity");
            }

            if (string.IsNullOrEmpty(domain.LocationIdentifier))
            {
                throw new InvalidOperationException("Location has no unique identifier");
            }

            if (domain is LinearLocation linearLocationDomain)
            {
                var entity = new LocationEntity
                {
                    Id = domain.Id,
                    Start = linearLocationDomain.Start,
                    End = linearLocationDomain.End,
                    Discriminator = "LinearLocation"
                };

                if (linearLocationDomain.Route != null)
                {
                    entity.RouteEntity = linearLocationDomain.Route.ToEntity();
                }

                return entity;
            }

            return new LocationEntity
            {
                Id = domain.Id,
                LocationIdentifier = domain.LocationIdentifier,
                Discriminator = "SectionLocation"
            };
        }
    }
}
