using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Data.Aggregation;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAggregatedResultRepository
    {
        void AddAggregatedResults(List<IAggregatedResult> aggregatedResults);

        IEnumerable<IAggregatedResult> GetAggregatedResults(Guid networkId);

        void CreateAggregatedResults<T>(
            Dictionary<(Guid maintainableAssetId, Guid attributeId), IAttributeValueHistory<T>>
                attributeValueHistoryPerMaintainableAssetIdAttributeIdTuple);

        List<AggregatedResultDTO> GetAggregatedResultsForAttributeNames(Guid networkId, List<string> attributeNames);
        List<AggregatedResultDTO> GetAggregatedResultsForMaintainableAsset(Guid assetId, List<Guid> attributeIds);
        List<AggregatedSelectValuesResultDTO> GetAggregatedResultsForAttributeNames(List<string> attributeNames);
    }
}
