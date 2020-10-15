using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAggregatedResultRepository
    {
        int AddAggregatedResults<T>(IEnumerable<AggregatedResult<T>> domainAggregatedResults, Guid networkId);
        int DeleteAggregatedResults(Guid networkId);
    }
}
