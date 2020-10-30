using System;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.LiteDb.Mappings
{
    public static class AggregatedResultMapper
    {
        public static IAggregatedResultEntity ToEntity(this IAggregatedResult aggregatedResult)
        {
            if (aggregatedResult == null)
            {
                throw new NullReferenceException("Cannot map null aggregated result value.");
            }

            if (aggregatedResult is AggregatedResult<double> numericAggregatedResult)
            {
                return new AggregatedResultEntity<double>
                {
                    AggregatedData = numericAggregatedResult.AggregatedData
                    .Select(_ => new AttributeYearValueEntity<double>()
                    {
                        AttributeEntity = _.attribute.ToEntity(),
                        Value = _.yearValuePair.value,
                        Year = _.yearValuePair.year
                    }),
                    Id = numericAggregatedResult.Id,
                    MaintainableAssetEntity = numericAggregatedResult.MaintainableAsset.ToEntity()
                };
            }

            if (aggregatedResult is AggregatedResult<string> textAggregatedResult)
            {
                return new AggregatedResultEntity<string>
                {
                    AggregatedData = textAggregatedResult.AggregatedData
                    .Select(_ => new AttributeYearValueEntity<string>()
                    {
                        AttributeEntity = _.attribute.ToEntity(),
                        Value = _.yearValuePair.value,
                        Year = _.yearValuePair.year
                    }),
                    Id = textAggregatedResult.Id,
                    MaintainableAssetEntity = textAggregatedResult.MaintainableAsset.ToEntity()
                };
            }

            throw new InvalidOperationException("Unable to determine aggregated result type.");
        }

        public static IAggregatedResult ToDomain<T>(this IAggregatedResultEntity aggregatedResultEntity)
        {
            if (aggregatedResultEntity == null)
            {
                throw new NullReferenceException("Cannot map null aggregated result entity.");
            }

            if (aggregatedResultEntity is AggregatedResultEntity<double> numericAggregatedResultEntity)
            {
                var tuple = numericAggregatedResultEntity.AggregatedData.Select(_ => (_.AttributeEntity.ToDomain(), (_.Year, _.Value)));
                return new AggregatedResult<double>(numericAggregatedResultEntity.Id, numericAggregatedResultEntity.MaintainableAssetEntity.ToDomain(), tuple);
            }

            if (aggregatedResultEntity is AggregatedResultEntity<string> textAggregatedResultEntity)
            {
                var tuple = textAggregatedResultEntity.AggregatedData.Select(_ => (_.AttributeEntity.ToDomain(), (_.Year, _.Value)));
                return new AggregatedResult<string>(textAggregatedResultEntity.Id, textAggregatedResultEntity.MaintainableAssetEntity.ToDomain(), tuple);
            }

            throw new InvalidOperationException("Cannot determine aggregated result entity type");
        }
    }
}
