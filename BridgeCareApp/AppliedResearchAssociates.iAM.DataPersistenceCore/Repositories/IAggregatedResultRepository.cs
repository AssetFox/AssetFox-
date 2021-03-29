using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAggregatedResultRepository
    {
        int CreateAggregatedResults(List<IAggregatedResult> aggregatedResults);

        IEnumerable<IAggregatedResult> GetAggregatedResults(Guid networkId);

        void CreateAggregatedResults<T>(
            Dictionary<(Guid maintainableAssetId, Guid attributeId), AttributeValueHistory<T>>
                attributeValueHistoryPerMaintainableAssetIdAttributeIdTuple);
    }
}
