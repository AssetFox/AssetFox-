using System;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class RouteItemMapper
    {
        public static Route ToDomain(this RouteEntity entity)
        {
            if (entity == null)
            {
                throw new NullReferenceException("Cannot map null Route entity to Route domain");
            };

            if (entity.Discriminator == "DirectionalRoute")
            {
                return new DirectionalRoute(entity.UniqueIdentifier, entity.Direction.Value);
            }

            return new SimpleRoute(entity.UniqueIdentifier);
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
                    Id = Guid.NewGuid(),
                    Direction = directionalRouteDomain.Direction,
                    UniqueIdentifier = directionalRouteDomain.UniqueIdentifier,
                    Discriminator = "DirectionalRoute"
                };
            }

            return new RouteEntity
            {
                Id = Guid.NewGuid(),
                UniqueIdentifier = domain.UniqueIdentifier,
                Discriminator = "SimpleRoute"
            };
        }
    }
}
