using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.Data.Networking;

namespace AppliedResearchAssociates.iAM.Data.Aggregation
{
    public interface IAggregatedResult
    {
        Guid Id { get; }
        MaintainableAsset MaintainableAsset { get; }
    }
}
