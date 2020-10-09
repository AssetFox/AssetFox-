using System;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.LiteDb.Mappings
{
    public static class AggregatedResultMapper
    {
        public static AggregatedResultEntity<T> ToEntity<T>(this AggregatedResult<T> aggregatedResult, Network network)
        {
            if (aggregatedResult == null || !aggregatedResult.AggregatedData.Any())
            {
                throw new NullReferenceException("Cannot map null AggregatedResult domains to AggregatedResult entities");
            }
            return new AggregatedResultEntity<T>()
            {
                MaintainableAssetEntity = aggregatedResult.MaintainableAsset.ToEntity(network.Id),
                AggregatedData = aggregatedResult.AggregatedData.Select(_ => (_.attribute.ToEntity(), _.yearValuePair))
            };
        }
    }
}
