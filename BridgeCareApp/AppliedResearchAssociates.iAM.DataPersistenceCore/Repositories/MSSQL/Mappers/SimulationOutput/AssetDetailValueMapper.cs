using System;
using System.Collections.Generic;
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
                Discriminator = AssetDetailValueDiscriminators.Number,
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
                Discriminator = AssetDetailValueDiscriminators.Text,
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
            {// Wjwjwj The "if" is a temporary hack which should be deleted prior to PR completion.
                if (attributeIdLookup.ContainsKey(keyValuePair.Key))
                {
                    var entity = ToNumericEntity(keyValuePair, attributeIdLookup);
                    entities.Add(entity);
                }
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

        public static void AddToDictionaries(ICollection<AssetDetailValueEntity> entityCollection, Dictionary<string, string> valuePerTextAttribute, Dictionary<string, double> valuePerNumericAttribute)
        {
            {
                foreach (var entity in entityCollection)
                {
                    var attributeName = entity.Attribute.Name;
                    // WjJake -- how should we handle unexpected cases, i.e. invalid discriminator, or discriminator is "number" but the numeric value is null?
                    switch (entity.Discriminator)
                    {
                    case AssetDetailValueDiscriminators.Number:
                        if (entity.NumericValue.HasValue)
                        {
                            valuePerNumericAttribute[attributeName] = entity.NumericValue.Value;

                        }
                        break;
                    case AssetDetailValueDiscriminators.Text:
                        valuePerTextAttribute[attributeName] = entity.TextValue;
                        break;
                    }
                }
            }
        }
    }
}
