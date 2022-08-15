﻿using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
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
                    var entity = ToNumericEntity(keyValuePair, attributeIdLookup);
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
            var areaKey = Network.DefaultSpatialWeightingIdentifier;
            var deckAreaKey = AttributeNameConstants.DeckArea;
            if (valuePerNumericAttribute.ContainsKey(deckAreaKey))
            {
                valuePerNumericAttribute[areaKey] = valuePerNumericAttribute[deckAreaKey];
            }
        }
    }
}
