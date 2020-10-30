using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAggregatedResultRepository
    {
        int CreateAggregatedResults(List<IAggregatedResult> aggregatedResults);

        IEnumerable<IAggregatedResult> GetAggregatedResults(Guid networkId);
    }
}
