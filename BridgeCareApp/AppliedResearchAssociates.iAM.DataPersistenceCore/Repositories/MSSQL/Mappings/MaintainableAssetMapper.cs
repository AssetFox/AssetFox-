using System;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class MaintainableAssetMapper
    {
        public static MaintainableAsset ToDomain(this MaintainableAssetEntity entity)
        {
            if (entity == null)
            {
                throw new NullReferenceException("Cannot map null MaintainableAsset entity to MaintainableAsset domain");
            }

            if (entity.MaintainableAssetLocation == null)
            {
                throw new NullReferenceException("MaintainableAsset entity is missing related MaintainableAssetLocation entity");
            }

            var maintainableAsset = new MaintainableAsset(entity.Id, entity.NetworkId, entity.MaintainableAssetLocation.ToDomain());

            if (entity.AssignedData != null && entity.AssignedData.Any())
            {
                if (entity.AssignedData.Any(a => a.Discriminator == "NumericAttributeDatum"))
                {
                    var numericAttributeData = entity.AssignedData
                        .Where(e => e.Discriminator == "NumericAttributeDatum")
                        .Select(e => e.ToDomain());

                    maintainableAsset.AssignAttributeDataFromDataSource(numericAttributeData);
                }

                if (entity.AssignedData.Any(a => a.Discriminator == "TextAttributeDatum"))
                {
                    var textAttributeData = entity.AssignedData
                        .Where(e => e.Discriminator == "TextAttributeDatum")
                        .Select(e => e.ToDomain());

                    maintainableAsset.AssignAttributeDataFromDataSource(textAttributeData);
                }
            }

            return maintainableAsset;
        }

        public static MaintainableAssetEntity ToEntity(this MaintainableAsset domain, Guid networkId)
        {
            if (domain == null)
            {
                throw new NullReferenceException("Cannot map null MaintainableAsset domain to MaintainableAsset entity");
            }

            var locationEntity = domain.Location.ToEntity(domain.Id, "MaintainableAssetEntity");

            return new MaintainableAssetEntity
            {
                Id = domain.Id,
                NetworkId = networkId,
                MaintainableAssetLocation = (MaintainableAssetLocationEntity)locationEntity
            };
        }
    }
}
