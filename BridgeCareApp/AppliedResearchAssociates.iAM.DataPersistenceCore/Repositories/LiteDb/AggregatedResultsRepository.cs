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

        public int AddAggregatedResults<T>(IEnumerable<AggregatedResult<T>> domainAggregatedResults, Guid networkId)
        {
            var aggregatedResultCollection = Context.Database.GetCollection<AggregatedResultEntity<T>>("AGGREGATED_RESULTS");
            var test = domainAggregatedResults.Select(_ => _.ToEntity());
            return aggregatedResultCollection.InsertBulk(test);
        }

        public int DeleteAggregatedResults(Guid networkId)
        {
            var value = Context.Database.GetCollection<AggregatedResultEntity<T>>("AGGREGATED_RESULTS")
                .Include(_ => _.MaintainableAssetEntity)
                .DeleteMany(_ => _.MaintainableAssetEntity.NetworkId == networkId);
            return value;
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
