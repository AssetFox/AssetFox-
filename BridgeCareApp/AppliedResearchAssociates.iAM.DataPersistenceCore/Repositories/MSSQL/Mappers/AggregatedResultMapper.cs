using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

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

        public static List<IAggregatedResult> ToDomain(this List<AggregatedResultEntity> entities)
        {
            var aggregatedResults = new List<IAggregatedResult>();

            if (entities.Any(_ => _.Discriminator == "NumericAggregatedResult"))
            {
                var aggregatedData = entities.Where(_ => _.Discriminator == "NumericAggregatedResult")
                    .Select(_ => (_.Attribute.ToDomain(), (_.Year, _.NumericValue ?? 0)));

                aggregatedResults.Add(new AggregatedResult<double>(Guid.NewGuid(),
                    entities.First().MaintainableAsset.ToDomain(), aggregatedData));
            }

            if (entities.Any(_ => _.Discriminator == "TextAggregatedResult"))
            {
                var aggregatedData = entities.Where(_ => _.Discriminator == "TextAggregatedResult")
                    .Select(_ => (_.Attribute.ToDomain(), (_.Year, _.TextValue ?? "")));

                aggregatedResults.Add(new AggregatedResult<string>(Guid.NewGuid(),
                    entities.First().MaintainableAsset.ToDomain(), aggregatedData));
            }

            return aggregatedResults;
        }
    }
}
