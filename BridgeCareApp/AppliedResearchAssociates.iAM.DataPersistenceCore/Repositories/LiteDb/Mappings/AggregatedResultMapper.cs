﻿using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.LiteDb.Mappings
{
    public static class AggregatedResultMapper
    {
        public static AggregatedResultEntity<T> ToEntity<T>(this AggregatedResult<T> aggregatedResult)
        {
            if (aggregatedResult == null || !aggregatedResult.AggregatedData.Any())
            {
                throw new NullReferenceException("Cannot map null AggregatedResult domains to AggregatedResult entities");
            }
            return new AggregatedResultEntity<T>()
            {
                Id = aggregatedResult.Id.ToString(),
                MaintainableAssetEntity = aggregatedResult.MaintainableAsset.ToEntity(),

                // LiteDB doesn't support tuples...so that's nice. :(
                //AggregatedData = aggregatedResult.AggregatedData.Select(_ => (_.attribute.ToEntity(), _.yearValuePair))
                AggregatedData = aggregatedResult.AggregatedData.Select(_ => new GarbageEntity<T>()
                {
                    AttributeEntity = _.attribute.ToEntity(),
                    Value = _.yearValuePair.value,
                    Year = _.yearValuePair.year
                })
            };
        }

        public static AggregatedResult<T> ToDomain<T>(this AggregatedResultEntity<T> aggregatedResultEntity)
        {
            if (aggregatedResultEntity == null || !aggregatedResultEntity.AggregatedData.Any())
            {
                throw new NullReferenceException("Cannot map null AggregatedResult domains to AggregatedResult entities");
            }
            return new AggregatedResult<T>(
                Guid.Parse(aggregatedResultEntity.Id),
                aggregatedResultEntity.MaintainableAssetEntity.ToDomain(),
                aggregatedResultEntity.AggregatedData.Select(_ => (_.AttributeEntity.ToDomain(), (_.Year, _.Value))));
        }
    }
}
