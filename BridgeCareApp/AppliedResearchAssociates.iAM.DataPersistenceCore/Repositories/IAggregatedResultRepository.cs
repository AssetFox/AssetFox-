using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.Analysis;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAggregatedResultRepository
    {
        void AddAggregatedResults(List<IAggregatedResult> aggregatedResults);

        IEnumerable<IAggregatedResult> GetAggregatedResults(Guid networkId);

        void CreateAggregatedResults<T>(
            Dictionary<(Guid maintainableAssetId, Guid attributeId), AttributeValueHistory<T>>
                attributeValueHistoryPerMaintainableAssetIdAttributeIdTuple);
    }
}
