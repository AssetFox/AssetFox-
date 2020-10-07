using System;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings
{
    public static class MaintainableAssetItemMapper
    {
        public static MaintainableAsset ToDomain(this MaintainableAssetEntity entity)
        {
            if (entity == null)
            {
                throw new NullReferenceException("Cannot map null Segment entity to Segment domain");
            }

            if (entity.Location == null)
            {
                throw new NullReferenceException("Segment entity is missing related Location entity");
            }

            var segment = new Segment(entity.Id, entity.Location.ToDomain());

            if (entity.AttributeData != null && entity.AttributeData.Any())
            {
                if (entity.AttributeData.Any(a => a.Discriminator == "NumericAttributeDatum"))
                {
                    var numericAttributeData = entity.AttributeData
                        .Where(e => e.Discriminator == "NumericAttributeDatum")
                        .Select(e => e.ToDomain());

                    segment.AssignAttributeData(numericAttributeData);
                }

                if (entity.AttributeData.Any(a => a.Discriminator == "TextAttributeDatum"))
                {
                    var textAttributeData = entity.AttributeData
                        .Where(e => e.Discriminator == "TextAttributeDatum")
                        .Select(e => e.ToDomain());

                    segment.AssignAttributeData(textAttributeData);
                }
            }

            return new MaintainableAsset(entity.Location.ToDomain(), entity.Id);
        }

        public static MaintainableAssetEntity ToEntity(this MaintainableAsset domain, Guid networkId)
        {
            var locationEntity = domain.Location.ToEntity();

            var maintainableAssetEntity = new MaintainableAssetEntity
            {
                Id = domain.Id,
                NetworkId = networkId,
                UniqueIdentifier = locationEntity.UniqueIdentifier,
                LocationId = locationEntity.Id,
                Location = locationEntity
            };

            return maintainableAssetEntity;
        }
    }
}
