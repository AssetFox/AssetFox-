using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class AssetSummaryDetailValueMapper
    {
        public static AssetSummaryDetailValueEntityIntId ToNumericEntity(
            Guid assetSummaryDetailId,
            KeyValuePair<string, double> assetSummaryDetailValue,
            Dictionary<string, Guid> attributeIdLookupDictionary)
        {
            var attributeId = attributeIdLookupDictionary[assetSummaryDetailValue.Key];
            var entity = new AssetSummaryDetailValueEntityIntId
            {
                AssetSummaryDetailId = assetSummaryDetailId,
                Discriminator = AssetDetailValueDiscriminators.Number,
                AttributeId = attributeId,
                NumericValue = assetSummaryDetailValue.Value,
            };
            return entity;
        }

        public static AssetSummaryDetailValueEntityIntId ToTextEntity(
            Guid assetSummaryDetailId,
            KeyValuePair<string, string> keyValuePair,
            Dictionary<string, Guid> attributeIdLookupDictionary)
        {
            var attributeId = attributeIdLookupDictionary[keyValuePair.Key];
            var entity = new AssetSummaryDetailValueEntityIntId
            {
                AssetSummaryDetailId = assetSummaryDetailId,
                Discriminator = AssetDetailValueDiscriminators.Text,
                AttributeId = attributeId,
                TextValue = keyValuePair.Value,
            };
            return entity;
        }

        internal static void AddToDictionaries(
            ICollection<AssetSummaryDetailValueEntityIntId> assetSummaryDetailValues,
            Dictionary<string, double> valuePerNumericAttribute,
            Dictionary<string, string> valuePerTextAttribute,
            Dictionary<Guid, string> attributeNameLookup)
        {
            foreach (var summary in assetSummaryDetailValues)
            {
                AddToDictionary(summary, valuePerNumericAttribute, valuePerTextAttribute, attributeNameLookup);
            }
            FillAreaAttributeValue(valuePerNumericAttribute);
        }

        public static void FillAreaAttributeValue(Dictionary<string, double> valuePerNumericAttribute)
        {
            var areaKey = Network.DefaultSpatialWeightingIdentifier;
            var deckAreaKey = AttributeNameConstants.DeckArea;
            if (valuePerNumericAttribute.ContainsKey(deckAreaKey))
            {
                valuePerNumericAttribute[areaKey] = valuePerNumericAttribute[deckAreaKey];
            }
        }

        public static void AddToDictionary(
            AssetSummaryDetailValueEntityIntId summary,
            Dictionary<string, double> valuePerNumericAttribute,
            Dictionary<string, string> valuePerTextAttribute,
            Dictionary<Guid, string> attributeNameLookup
            )
        {
            var attributeName = attributeNameLookup[summary.AttributeId];
            switch (summary.Discriminator)
            {
            case AssetDetailValueDiscriminators.Number:
                if (summary.NumericValue.HasValue)
                {
                    valuePerNumericAttribute[attributeName] = summary.NumericValue.Value;
                }
                break;
            case AssetDetailValueDiscriminators.Text:
                valuePerTextAttribute[attributeName] = summary.TextValue;
                break;
            }
        }

        public static List<AssetSummaryDetailValueEntityIntId> ToNumericEntityList(
            Guid assetSummaryDetailId,
            Dictionary<string, double> assetSummaryDetailValues,
            Dictionary<string, Guid> attributeIdLookup)
        {
            var entities = new List<AssetSummaryDetailValueEntityIntId>();
            var areaKey = Network.DefaultSpatialWeightingIdentifier;
            var deckAreaKey = AttributeNameConstants.DeckArea;
            var containsAreaKey = assetSummaryDetailValues.ContainsKey(areaKey);
            var containsDeckAreaKey = assetSummaryDetailValues.ContainsKey(deckAreaKey);
            if (containsAreaKey && !containsDeckAreaKey)
            {
                var message = $"Unable to save simulation results. We have a value for {areaKey} but no value for {deckAreaKey}.";
                throw new Exception(message);
            }
            else if (containsDeckAreaKey && !containsAreaKey)
            {
                var message = $"Unable to save simulation results. We have a value for {deckAreaKey} but no value for {areaKey}.";
                throw new Exception(message);
            }
            else if (containsDeckAreaKey && assetSummaryDetailValues[deckAreaKey] != assetSummaryDetailValues[areaKey])
            {
                var message = $"Unable to save simulation results. We expect the value for {deckAreaKey} to match the value for {areaKey}. But the value for {deckAreaKey} is {assetSummaryDetailValues[deckAreaKey]} and the value for {areaKey} is {assetSummaryDetailValues[areaKey]},";
                throw new Exception(message);
            }
            foreach (var keyValuePair in assetSummaryDetailValues)
            {
                if (attributeIdLookup.ContainsKey(keyValuePair.Key))
                {
                    var entity = ToNumericEntity(assetSummaryDetailId, keyValuePair, attributeIdLookup);
                    entities.Add(entity);
                }
                else if (keyValuePair.Key != areaKey)
                {
                    var message = $"Unable to save simulation results. We have a value for {keyValuePair.Key}, but no corresponding attribute.";
                    throw new Exception(message);
                }
            }
            return entities;

        }

        public static List<AssetSummaryDetailValueEntityIntId> ToTextEntityList(
            Guid assetSummaryDetailId,
            Dictionary<string, string> assetSummaryDetailValues,
            Dictionary<string, Guid> attributeIdLookupDictionary)
        {
            var entities = new List<AssetSummaryDetailValueEntityIntId>();
            foreach (var keyValuePair in assetSummaryDetailValues)
            {
                var entity = ToTextEntity(assetSummaryDetailId, keyValuePair, attributeIdLookupDictionary);
                entities.Add(entity);
            }
            return entities;
        }

    }
}
