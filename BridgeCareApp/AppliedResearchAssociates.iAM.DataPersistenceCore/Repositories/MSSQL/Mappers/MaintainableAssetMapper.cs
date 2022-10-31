using System;
using System.Linq;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Analysis;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class MaintainableAssetMapper
    {
        public static MaintainableAsset ToDomain(this MaintainableAssetEntity entity, string encryptionKey)
        {
            var maintainableAsset = new MaintainableAsset(
                entity.Id, entity.NetworkId, entity.MaintainableAssetLocation.ToDomain(), entity.SpatialWeighting);

            if (entity.AssignedData != null && entity.AssignedData.Any())
            {
                if (entity.AssignedData.Any(a => a.Discriminator == "NumericAttributeDatum"))
                {
                    var numericAttributeData = entity.AssignedData
                        .Where(e => e.Discriminator == "NumericAttributeDatum")
                        .Select(e => e.ToDomain(encryptionKey));

                    maintainableAsset.AssignAttributeDataFromDataSource(numericAttributeData);
                }

                if (entity.AssignedData.Any(a => a.Discriminator == "TextAttributeDatum"))
                {
                    var textAttributeData = entity.AssignedData
                        .Where(e => e.Discriminator == "TextAttributeDatum")
                        .Select(e => e.ToDomain(encryptionKey));

                    maintainableAsset.AssignAttributeDataFromDataSource(textAttributeData);
                }
            }

            return maintainableAsset;
        }

        public static MaintainableAssetEntity ToEntity(this MaintainableAsset domain, Guid networkId) =>
            new MaintainableAssetEntity
            {
                Id = domain.Id,
                NetworkId = networkId,
                SpatialWeighting = domain.SpatialWeighting
            };

        public static MaintainableAssetEntity ToEntity(this AnalysisMaintainableAsset asset, Guid networkId) =>
            new MaintainableAssetEntity
            {
                Id = asset.Id,
                NetworkId = networkId,
                SpatialWeighting = asset.SpatialWeighting.Expression,
                AssetName = asset.AssetName
            };
    }
}
