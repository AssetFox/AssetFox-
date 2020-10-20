using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataPersistenceCore.LiteDb.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb
{
    public class AggregatedResultsRepository : LiteDbRepository, IAggregatedResultRepository
    {
        public AggregatedResultsRepository(ILiteDbContext context) : base(context) { }

        public int CreateAggregatedResults<T>(List<AggregatedResult<T>> domainAggregatedResults)
        {
            var aggregatedResultCollection = Context.Database.GetCollection<IAggregatedResultEntity>("AGGREGATED_RESULTS");
            return aggregatedResultCollection.InsertBulk(domainAggregatedResults.Select(_ => _.ToEntity()));
        }

        public int DeleteAggregatedResults(Guid networkId, List<Guid> metaDataAttributeIds, List<Guid> networkAttributeIds)
        {
            var aggregatedResultsCollection = Context.Database.GetCollection<IAggregatedResultEntity>("AGGREGATED_RESULTS");

            var items = aggregatedResultsCollection
                .Include(_ => _.MaintainableAssetEntity)
                .Find(_ => _.MaintainableAssetEntity.NetworkId == networkId);

            Context.Database.BeginTrans();
            foreach (var item in items)
            {
                aggregatedResultsCollection.Delete(item.Id);
            }
            Context.Database.Commit();
            return items.Count();
        }

        public IEnumerable<IAggregatedResult> GetAggregatedResults(Guid networkId)
        {
            var aggregatedResultsCollection = Context.Database.GetCollection<IAggregatedResultEntity>("AGGREGATED_RESULTS");
            var result = aggregatedResultsCollection
                .Include(_ => _.MaintainableAssetEntity)
                .Include(_ => _.MaintainableAssetEntity.LocationEntity)
                .Find(_ => _.MaintainableAssetEntity.NetworkId == networkId).ToList();
            
            var numericAggregatedResultEntities = result.Where(_ => _ is AggregatedResultEntity<double>).Select(_ => _.ToDomain<double>());
            var textAggregatedResultEntities = result.Where(_ => _ is AggregatedResultEntity<string>).Select(_ => _.ToDomain<string>());

            return numericAggregatedResultEntities.Concat(textAggregatedResultEntities);
        }
    }
}
