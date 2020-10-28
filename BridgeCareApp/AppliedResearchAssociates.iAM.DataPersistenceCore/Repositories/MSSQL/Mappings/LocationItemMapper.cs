using System;
using System.Dynamic;
using System.Runtime.InteropServices.ComTypes;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
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
                Route route;
                if (entity.Direction.HasValue)
                {
                    route = new DirectionalRoute(entity.UniqueIdentifier, entity.Direction.Value);
                }
                else
                {
                    route = new SimpleRoute(entity.UniqueIdentifier);
                }
                return new LinearLocation(
                    entity.Id,
                    route,
                    entity.UniqueIdentifier,
                    entity.Start ?? 0,
                    entity.End ?? 0);
            }

            if (entity.Discriminator == "SectionLocation")
            {
                return new SectionLocation(entity.Id, entity.UniqueIdentifier);
            }

            throw new InvalidOperationException("Cannot determine Location entity type");
        }

        public static LocationEntity ToEntity(this Location domain, Guid parentId, string parentEntityType)
        {
            if (domain == null)
            {
                throw new NullReferenceException("Cannot map null Location domain to Location entity");
            }

            if (string.IsNullOrEmpty(domain.UniqueIdentifier))
            {
                throw new InvalidOperationException("Location has no unique identifier");
            }

            LocationEntity entity;

            if (parentEntityType == "MaintainableAssetEntity")
            {
                entity = new MaintainableAssetLocationEntity(domain.Id, "SectionLocation", domain.UniqueIdentifier)
                {
                    MaintainableAssetId = parentId
                };
                UpdateLinearLocationFields(domain, entity);
                return entity;
            }

            if (parentEntityType == "AttributeDatumEntity")
            {
                entity = new AttributeDatumLocationEntity(domain.Id, "SectionLocation", domain.UniqueIdentifier)
                {
                    AttributeDatumId = parentId
                };
                UpdateLinearLocationFields(domain, entity);
                return entity;
            }

            throw new NullReferenceException("Could not determine Location entity type");
        }

        private static void UpdateLinearLocationFields(Location domain, LocationEntity entity)
        {
            if (!(domain is LinearLocation linearLocationDomain))
            {
                return;
            }

            entity.Start = linearLocationDomain.Start;
            entity.End = linearLocationDomain.End;
            entity.Discriminator = "LinearLocation";

            if (linearLocationDomain.Route != null && linearLocationDomain.Route is DirectionalRoute directionalRoute)
            {
                entity.Direction = directionalRoute.Direction;
            }
        }
    }
}
