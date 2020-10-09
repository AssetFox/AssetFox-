using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataPersistenceCore.LiteDb.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Repositories
{
    public class AggregatedResultsRepository<T> : LiteDbRepository<AggregatedResultEntity<T>, AggregatedResult<T>>
    {
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
