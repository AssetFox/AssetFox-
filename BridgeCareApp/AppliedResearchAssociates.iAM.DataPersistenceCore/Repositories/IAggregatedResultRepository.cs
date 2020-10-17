using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAggregatedResultRepository
    {
        int AddAggregatedResults<T>(IEnumerable<AggregatedResult<T>> domainAggregatedResults);
        int DeleteAggregatedResults(Guid networkId);
        IEnumerable<IAggregatedResultEntity> GetAggregatedResultEntities(Guid networkId);
    }
}
