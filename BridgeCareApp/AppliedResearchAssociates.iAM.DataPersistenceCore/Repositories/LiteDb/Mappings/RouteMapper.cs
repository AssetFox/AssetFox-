using System;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.LiteDb.Mappings
{
    public static class RouteMapper
    {
        public static Route ToDomain(this RouteEntity entity)
        {
            if (entity == null)
            {
                throw new NullReferenceException("Cannot map null Route entity to Route domain");
            };

            if (entity.Discriminator == "DirectionalRoute")
            {
                return new DirectionalRoute(entity.LocationIdentifier, entity.Direction);
            }

            return new SimpleRoute(entity.LocationIdentifier);
        }

        public static RouteEntity ToEntity(this Route domain)
        {
            if (domain == null)
            {
                throw new NullReferenceException("Cannot map null Route domain to Route entity");
            }

            if (domain is DirectionalRoute directionalRouteDomain)
            {
                return new RouteEntity
                {
                    Direction = directionalRouteDomain.Direction,
                    LocationIdentifier = directionalRouteDomain.LocationIdentifier,
                    Discriminator = "DirectionalRoute"
                };
            }

            return new RouteEntity
            {
                LocationIdentifier = domain.LocationIdentifier,
                Discriminator = "SimpleRoute"
            };
        }
    }
}
