using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataAssignment.Aggregation
{
    public class AggregatedResult<T>
    {
        public AggregatedResult(MaintainableAsset maintainableAsset, IEnumerable<(Attribute attribute, (int year, T value))> aggregatedData)
        {
            MaintainableAsset = maintainableAsset;
            AggregatedData = aggregatedData;
        }

        public MaintainableAsset MaintainableAsset { get; }
        public IEnumerable<(Attribute attribute, (int year, T value) yearValuePair)> AggregatedData { get; }
    }
}
