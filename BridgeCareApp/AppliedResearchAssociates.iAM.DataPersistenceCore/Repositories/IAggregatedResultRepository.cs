using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAggregatedResultRepository
    {
        int CreateAggregatedResults<T>(List<AggregatedResult<T>> aggregatedResults);
        int DeleteAggregatedResults(Guid networkId, List<Guid> metaDataAttributeIds, List<Guid> networkAttributeIds);
        IEnumerable<IAggregatedResult> GetAggregatedResults(Guid networkId);
    }
}
