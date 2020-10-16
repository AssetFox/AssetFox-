using System;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.LiteDb.Mappings
{
    public static class MaintainableAssetMapper
    {
        public static MaintainableAsset ToDomain(this MaintainableAssetEntity entity)
        {
            if (entity == null)
            {
                throw new NullReferenceException("Cannot map null MaintainableAsset entity to MaintainableAsset domain");
            }

            if (entity.LocationEntity == null)
            {
                throw new NullReferenceException("MaintainableAsset entity is missing related Location entity");
            }

            var maintainableAsset = new MaintainableAsset(Guid.Parse(entity.Id), Guid.Parse(entity.NetworkId), entity.LocationEntity.ToDomain());

            if (entity.AttributeDatumEntities != null && entity.AttributeDatumEntities.Any())
            {
                if (entity.AttributeDatumEntities.Any(a => a.Discriminator == "NumericAttributeDatum"))
                {
                    var numericAttributeData = entity.AttributeDatumEntities
                        .Where(e => e.Discriminator == "NumericAttributeDatum")
                        .Select(e => e.ToDomain<double>());

                    maintainableAsset.AssignAttributeData(numericAttributeData);
                }

                if (entity.AttributeDatumEntities.Any(a => a.Discriminator == "TextAttributeDatum"))
                {
                    var textAttributeData = entity.AttributeDatumEntities
                        .Where(e => e.Discriminator == "TextAttributeDatum")
                        .Select(e => e.ToDomain<string>());

                    maintainableAsset.AssignAttributeData(textAttributeData);
                }
            }

            return maintainableAsset;
        }

        public static MaintainableAssetEntity ToEntity(this MaintainableAsset domain)
        {
            if (domain == null)
            {
                throw new NullReferenceException("Cannot map null MaintainableAsset domain to MaintainableAsset entity");
            }

            var locationEntity = domain.Location.ToEntity();

            return new MaintainableAssetEntity
            {
                Id = domain.Id.ToString(),
                AttributeDatumEntities = domain.AssignedData.Select(_ => _.ToEntity()).ToList(),
                NetworkId = domain.NetworkId.ToString(),
                LocationEntity = locationEntity
            };
        }
    }
}
