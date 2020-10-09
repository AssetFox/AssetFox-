using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataPersistenceCore.LiteDb.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.LiteDb.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;
using LiteDB;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Repositories
{
    public class AggregatedResultsRepository<T> : LiteDbRepository<AggregatedResultEntity<T>, AggregatedResult<T>>
    {
        public override void AddAll(IEnumerable<AggregatedResult<T>> data, params object[] args)
        {
            using (var db = new LiteDatabase(@"C:\Users\cbecker\Desktop\MyData.db"))
            {
                var aggregatedResultsCollection = db.GetCollection<AggregatedResultEntity<T>>("AGGREGATED_RESULTS");
                aggregatedResultsCollection.InsertBulk(data.Select(_ => _.ToEntity()));
            }
        }
        protected override AggregatedResult<T> ToDomain(AggregatedResultEntity<T> dataEntity)
        {
            throw new NotImplementedException();
        }

        protected override AggregatedResultEntity<T> ToEntity(AggregatedResult<T> domainModel)
        {
            throw new NotImplementedException();
        }
    }
}
