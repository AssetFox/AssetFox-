using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAggregatedResultRepository
    {
        int CreateAggregatedResults<T>(IEnumerable<AggregatedResult<T>> aggregatedResults);
        int DeleteAggregatedResults(Guid networkId);
        IEnumerable<IAggregatedResult> GetAggregatedResults(Guid networkId);
    }
}
