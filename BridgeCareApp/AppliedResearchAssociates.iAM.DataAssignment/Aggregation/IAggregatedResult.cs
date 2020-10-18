using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;

namespace AppliedResearchAssociates.iAM.DataAssignment.Aggregation
{
    public interface IAggregatedResult
    {
        Guid Id { get; }
        MaintainableAsset MaintainableAsset { get; }
    }
}
