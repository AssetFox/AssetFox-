using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using DataMinerAttribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataAssignment.Aggregation
{
    public class AggregatedResult<T> : IAggregatedResult
    {
        public AggregatedResult(Guid id, MaintainableAsset maintainableAsset, IEnumerable<(DataMinerAttribute attribute, (int year, T value))> aggregatedData)
        {
            Id = id;
            MaintainableAsset = maintainableAsset;
            AggregatedData = aggregatedData;
        }

        public Guid Id { get; }
        public MaintainableAsset MaintainableAsset { get; }
        public IEnumerable<(DataMinerAttribute attribute, (int year, T value) yearValuePair)> AggregatedData { get; }
    }
}
