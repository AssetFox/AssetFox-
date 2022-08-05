using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class AssetSummaryDetailValueMapper
    {
        public static AssetSummaryDetailValueEntity ToNumericEntity(Guid maintainableAssetId, KeyValuePair<string, double> assetSummaryDetailValue, Dictionary<string, Guid> attributeIdLookupDictionary)
        {
            var id = Guid.NewGuid();
            var attributeId = attributeIdLookupDictionary[assetSummaryDetailValue.Key];
            var entity = new AssetSummaryDetailValueEntity
            {
                Id = id,
                Discriminator = "Numeric",
                AttributeId = attributeId,
                MaintainableAssetId = maintainableAssetId,
                NumericValue = assetSummaryDetailValue.Value,
            };
            return entity;
        }

        public static List<AssetSummaryDetailValueEntity> ToNumericEntityList(Guid maintainableAssetId, Dictionary<string, double> assetSummaryDetailValues, Dictionary<string, Guid> attributeIdLookupDictionary)
        {
            var entities = new List<AssetSummaryDetailValueEntity>();
            foreach (var keyValuePair in assetSummaryDetailValues)
            {
                var entity = ToNumericEntity(maintainableAssetId, keyValuePair, attributeIdLookupDictionary);
                entities.Add(entity);
            }
            return entities;
        }
    }
}
