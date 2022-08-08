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
        public static AssetSummaryDetailValueEntity ToNumericEntity(KeyValuePair<string, double> assetSummaryDetailValue, Dictionary<string, Guid> attributeIdLookupDictionary)
        {
            var id = Guid.NewGuid();
            var attributeId = attributeIdLookupDictionary[assetSummaryDetailValue.Key];
            var entity = new AssetSummaryDetailValueEntity
            {
                Id = id,
                Discriminator = "Numeric",
                AttributeId = attributeId,
                NumericValue = assetSummaryDetailValue.Value,
            };
            return entity;
        }

        public static AssetSummaryDetailValueEntity ToTextEntity(
            KeyValuePair<string, string> keyValuePair,
            Dictionary<string, Guid> attributeIdLookupDictionary)
        {
            var id = Guid.NewGuid();
            var attributeId = attributeIdLookupDictionary[keyValuePair.Key];
            var entity = new AssetSummaryDetailValueEntity
            {
                Id = id,
                Discriminator = "Text",
                AttributeId = attributeId,
                TextValue = keyValuePair.Value,
            };
            return entity;
        }

        public static List<AssetSummaryDetailValueEntity> ToNumericEntityList(
            Dictionary<string, double> assetSummaryDetailValues,
            Dictionary<string, Guid> attributeIdLookupDictionary)
        {
            var entities = new List<AssetSummaryDetailValueEntity>();
            foreach (var keyValuePair in assetSummaryDetailValues)
            {
                var entity = ToNumericEntity(keyValuePair, attributeIdLookupDictionary);
                entities.Add(entity);
            }
            return entities;
        }

        public static List<AssetSummaryDetailValueEntity> ToTextEntityList(
            Dictionary<string, string> assetSummaryDetailValues,
            Dictionary<string, Guid> attributeIdLookupDictionary)
        {
            var entities = new List<AssetSummaryDetailValueEntity>();
            foreach (var keyValuePair in assetSummaryDetailValues)
            {
                var entity = ToTextEntity(keyValuePair, attributeIdLookupDictionary);
                entities.Add(entity);
            }
            return entities;
        }

    }
}
