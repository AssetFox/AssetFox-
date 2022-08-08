using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class AssetDetailValueMapper
    {
        public static AssetDetailValueEntity ToNumericEntity(
            Guid maintainableAssetId,
            KeyValuePair<string, double> assetSummaryDetailValue,
            Dictionary<string, Guid> attributeIdLookupDictionary)
        {
            var id = Guid.NewGuid();
            var attributeId = attributeIdLookupDictionary[assetSummaryDetailValue.Key];
            var entity = new AssetDetailValueEntity
            {
                Id = id,
                Discriminator = "Numeric",
                AttributeId = attributeId,
                MaintainableAssetId = maintainableAssetId,
                NumericValue = assetSummaryDetailValue.Value,
            };
            return entity;
        }

        public static AssetDetailValueEntity ToTextEntity(
            Guid maintainableAssetId,
            KeyValuePair<string, string> assetSummaryDetailValue,
            Dictionary<string, Guid> attributeIdLookupDictionary)
        {
            var attributeId = attributeIdLookupDictionary[assetSummaryDetailValue.Key];
            var id = Guid.NewGuid();
            var entity = new AssetDetailValueEntity
            {
                Id = id,
                Discriminator = "Text",
                AttributeId = attributeId,
                MaintainableAssetId = maintainableAssetId,
                TextValue = assetSummaryDetailValue.Value,
            };
            return entity;
        }

        public static List<AssetDetailValueEntity> ToNumericEntityList(
            Guid maintainableAssetId,
            Dictionary<string, double> assetSummaryDetailValues,
            Dictionary<string, Guid> attributeIdLookup)
        {
            var entities = new List<AssetDetailValueEntity>();
            foreach (var keyValuePair in assetSummaryDetailValues)
            {
                var entity = ToNumericEntity(maintainableAssetId, keyValuePair, attributeIdLookup);
                entities.Add(entity);
            }
            return entities;
        }

        public static List<AssetDetailValueEntity> ToTextEntityList(
            Guid maintainableAssetId,
            Dictionary<string, string> assetSummaryDetailValues,
            Dictionary<string, Guid> attributeIdLookup)
        {
            var entities = new List<AssetDetailValueEntity>();
            foreach (var keyValuePair in assetSummaryDetailValues)
            {
                var entity = ToTextEntity(maintainableAssetId, keyValuePair, attributeIdLookup);
                entities.Add(entity);
            }
            return entities;
        }
    }
}
