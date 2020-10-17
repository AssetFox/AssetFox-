using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataPersistenceCore.LiteDb.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb
{
    public class AggregatedResultsRepository<T> : LiteDbRepository<AggregatedResultEntity<T>, AggregatedResult<T>>, IAggregatedResultRepository, IRepository<AggregatedResult<T>>
    {
        public AggregatedResultsRepository(ILiteDbContext context) : base(context) { }

        public int AddAggregatedResults<U>(IEnumerable<AggregatedResult<U>> domainAggregatedResults)
        {
            var aggregatedResultCollection = Context.Database.GetCollection<IAggregatedResultEntity>("AGGREGATED_RESULTS");
            return aggregatedResultCollection.InsertBulk(domainAggregatedResults.Select(_ => _.ToEntity()));
        }

        public int DeleteAggregatedResults(Guid networkId)
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
            return 0;
        }

        public IEnumerable<IAggregatedResultEntity> GetAggregatedResultEntities(Guid networkId)
        {
            var aggregatedResultsCollection = Context.Database.GetCollection<IAggregatedResultEntity>("AGGREGATED_RESULTS");
            var result = aggregatedResultsCollection
                .Include(_ => _.MaintainableAssetEntity)
                .Find(_ => _.MaintainableAssetEntity.NetworkId == networkId).ToList();
            return result.Select(_ => _.ToDomain());
        }

        protected override AggregatedResult<T> ToDomain(AggregatedResultEntity<T> dataEntity)
        {
            throw new System.NotImplementedException();
        }

        protected override AggregatedResultEntity<T> ToEntity(AggregatedResult<T> domainModel)
        {
            throw new System.NotImplementedException();
        }
    }
}
