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
                NumericValue = assetSummaryDetailValue.Value,
            };
            return entity;
        }

        public static AssetDetailValueEntity ToTextEntity(
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
                TextValue = assetSummaryDetailValue.Value,
            };
            return entity;
        }

        public static List<AssetDetailValueEntity> ToNumericEntityList(
            Dictionary<string, double> assetSummaryDetailValues,
            Dictionary<string, Guid> attributeIdLookup)
        {
            var entities = new List<AssetDetailValueEntity>();
            foreach (var keyValuePair in assetSummaryDetailValues)
            {
                var entity = ToNumericEntity(keyValuePair, attributeIdLookup);
                entities.Add(entity);
            }
            return entities;
        }

        public static List<AssetDetailValueEntity> ToTextEntityList(
            Dictionary<string, string> assetSummaryDetailValues,
            Dictionary<string, Guid> attributeIdLookup)
        {
            var entities = new List<AssetDetailValueEntity>();
            foreach (var keyValuePair in assetSummaryDetailValues)
            {
                var entity = ToTextEntity(keyValuePair, attributeIdLookup);
                entities.Add(entity);
            }
            return entities;
        }
    }
}
