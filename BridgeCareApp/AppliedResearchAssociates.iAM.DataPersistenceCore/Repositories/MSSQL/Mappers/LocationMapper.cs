using System;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class LocationMapper
    {
        public static Location ToDomain(this LocationEntity entity)
        {
            if (entity == null)
            {
                throw new NullReferenceException("Cannot map null Location entity to Location domain");
            }

            if (entity.Discriminator == DataPersistenceConstants.LinearLocation)
            {
                Route route;
                if (entity.Direction.HasValue)
                {
                    route = new DirectionalRoute(entity.LocationIdentifier, entity.Direction.Value);
                }
                else
                {
                    route = new SimpleRoute(entity.LocationIdentifier);
                }
                return new LinearLocation(
                    entity.Id,
                    route,
                    entity.LocationIdentifier,
                    entity.Start ?? 0,
                    entity.End ?? 0);
            }

            if (entity.Discriminator == DataPersistenceConstants.SectionLocation)
            {
                return new SectionLocation(entity.Id, entity.LocationIdentifier);
            }

            throw new InvalidOperationException("Cannot determine Location entity type");
        }

        public static LocationEntity ToEntity(this Location domain, Guid parentId, Type parentEntityType)
        {
            if (domain == null)
            {
                throw new NullReferenceException("Cannot map null Location domain to Location entity");
            }

            if (string.IsNullOrEmpty(domain.LocationIdentifier))
            {
                throw new InvalidOperationException("Location has no unique identifier");
            }

            LocationEntity entity;

            if (parentEntityType == typeof(MaintainableAssetEntity))
            {
                entity = new MaintainableAssetLocationEntity(domain.Id, DataPersistenceConstants.SectionLocation, domain.LocationIdentifier)
                {
                    MaintainableAssetId = parentId
                };
                UpdateLinearLocationFields(domain, entity);
                return entity;
            }

            if (parentEntityType == typeof(AttributeDatumEntity))
            {
                entity = new AttributeDatumLocationEntity(domain.Id, DataPersistenceConstants.SectionLocation, domain.LocationIdentifier)
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
            entity.Discriminator = DataPersistenceConstants.LinearLocation;

            if (linearLocationDomain.Route is DirectionalRoute directionalRoute)
            {
                entity.Direction = directionalRoute.Direction;
            }
        }

        public static LocationEntity CreateMaintainableAssetLocation(this MaintainableAssetEntity entity) =>
            new MaintainableAssetLocationEntity(Guid.NewGuid(), DataPersistenceConstants.SectionLocation, $"{entity.FacilityName}-{entity.SectionName}")
            {
                MaintainableAssetId = entity.Id
            };
    }
}
