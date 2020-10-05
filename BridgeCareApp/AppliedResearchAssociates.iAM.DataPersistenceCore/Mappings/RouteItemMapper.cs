using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings
{
    public static class RouteItemMapper
    {
        public static Route ToDomain(this RouteEntity entity)
        {
            if (entity == null)
            {
                throw new NullReferenceException("RouteEntity object is null");
            };

            if (entity is DirectionalRouteEntity directionalRouteEntity)
            {
                return new DirectionalRoute(directionalRouteEntity.UniqueIdentifier, directionalRouteEntity.Direction);
            }

            return new SimpleRoute(entity.UniqueIdentifier);
        }

        public static RouteEntity ToEntity(this Route domain, Guid routeId)
        {
            if (domain == null)
            {
                throw new NullReferenceException("Route object is null");
            }

            if (domain is DirectionalRoute directionalRouteDomain)
            {
                return new DirectionalRouteEntity
                {
                    Id = routeId,
                    Direction = directionalRouteDomain.Direction,
                    UniqueIdentifier = directionalRouteDomain.UniqueIdentifier
                };
            }

            return new RouteEntity
            {
                Id = routeId,
                UniqueIdentifier = domain.UniqueIdentifier
            };
        }
    }
}
