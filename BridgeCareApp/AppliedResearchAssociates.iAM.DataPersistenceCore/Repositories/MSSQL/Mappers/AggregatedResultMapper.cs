using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Data.Aggregation;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class AggregatedResultMapper
    {
        public static IEnumerable<AggregatedResultEntity> ToEntity(this IAggregatedResult domain) =>
            domain switch
            {
                AggregatedResult<double> numericAggregatedResult => numericAggregatedResult.AggregatedData.Select(_ =>
                    new AggregatedResultEntity
                    {
                        Id = Guid.NewGuid(),
                        MaintainableAssetId = numericAggregatedResult.MaintainableAsset.Id,
                        AttributeId = _.attribute.Id,
                        Discriminator = "NumericAggregatedResult",
                        Year = _.yearValuePair.year,
                        NumericValue = Convert.ToDouble(_.yearValuePair.value),
                    }),
                AggregatedResult<string> textAggregatedResult => textAggregatedResult.AggregatedData.Select(_ =>
                    new AggregatedResultEntity
                    {
                        Id = Guid.NewGuid(),
                        MaintainableAssetId = textAggregatedResult.MaintainableAsset.Id,
                        AttributeId = _.attribute.Id,
                        Discriminator = "TextAggregatedResult",
                        Year = _.yearValuePair.year,
                        TextValue = _.yearValuePair.value.ToString(),
                    }),
                _ => throw new InvalidOperationException(
                    "Unable to determine Value data type for AttributeDatum entity")
            };

        public static List<IAggregatedResult> ToDomain(this List<AggregatedResultEntity> entities, string encryptionKey)
        {
            var aggregatedResults = new List<IAggregatedResult>();

            if (entities.Any(_ => _.Discriminator == DataPersistenceConstants.AggregatedResultNumericDiscriminator))
            {
                var aggregatedData = entities.Where(_ =>
                        _.Discriminator == DataPersistenceConstants.AggregatedResultNumericDiscriminator)
                    .Select(_ => (_.Attribute.ToDomain(encryptionKey), (_.Year, _.NumericValue ?? 0)));

                aggregatedResults.Add(new AggregatedResult<double>(Guid.NewGuid(),
                    entities.First().MaintainableAsset.ToDomain(encryptionKey), aggregatedData));
            }

            if (entities.Any(_ => _.Discriminator == DataPersistenceConstants.AggregatedResultTextDiscriminator))
            {
                var aggregatedData = entities.Where(_ =>
                        _.Discriminator == DataPersistenceConstants.AggregatedResultTextDiscriminator)
                    .Select(_ => (_.Attribute.ToDomain(encryptionKey), (_.Year, _.TextValue ?? "")));

                aggregatedResults.Add(new AggregatedResult<string>(Guid.NewGuid(),
                    entities.First().MaintainableAsset.ToDomain(encryptionKey), aggregatedData));
            }

            return aggregatedResults;
        }

        public static List<AggregatedResultEntity> ToEntity<T>(this IAttributeValueHistory<T> domain, Guid maintainableAssetId, Guid attributeId)
        {
            using var historyEnumerator = domain.GetEnumerator();
            historyEnumerator.Reset();

            var entities = new List<AggregatedResultEntity>();

            while (historyEnumerator.MoveNext())
            {
                entities.Add(new AggregatedResultEntity
                {
                    Id = Guid.NewGuid(),
                    MaintainableAssetId = maintainableAssetId,
                    AttributeId = attributeId,
                    Year = historyEnumerator.Current.Key,
                    TextValue = typeof(T) == typeof(string) ? historyEnumerator.Current.Value.ToString() : null,
                    NumericValue = typeof(T) == typeof(double) ? Convert.ToDouble(historyEnumerator.Current.Value) : (double?)null,
                    Discriminator = typeof(T) == typeof(double)
                        ? DataPersistenceConstants.AggregatedResultNumericDiscriminator
                        : DataPersistenceConstants.AggregatedResultTextDiscriminator
                });
            }

            if (!entities.Any())
            {
                entities.Add(new AggregatedResultEntity
                {
                    Id = Guid.NewGuid(),
                    MaintainableAssetId = maintainableAssetId,
                    AttributeId = attributeId,
                    Year = 0,
                    TextValue = typeof(T) == typeof(string) && domain.MostRecentValue != null ? domain.MostRecentValue.ToString() : null,
                    NumericValue = typeof(T) == typeof(double) && domain.MostRecentValue != null ? Convert.ToDouble(domain.MostRecentValue) : (double?)null,
                    Discriminator = typeof(T) == typeof(double)
                        ? DataPersistenceConstants.AggregatedResultNumericDiscriminator
                        : DataPersistenceConstants.AggregatedResultTextDiscriminator
                });
            }

            return entities;
        }

        public static AggregatedResultDTO ToDto(this AggregatedResultEntity entity)
        {
            var attribute = AttributeMapper.ToAbbreviatedDto(entity.Attribute);
            return new AggregatedResultDTO
            {
                MaintainableAssetId = entity.MaintainableAssetId,
                TextValue = entity.TextValue,
                NumericValue = entity.NumericValue,
                Year = entity.Year,
                Discriminator = entity.Discriminator,
                Attribute = attribute,
            };
        }
    }
}
