using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Interfaces
{
    public interface ISegmentDataRepository
    {
        void AddNetworkSegments(IEnumerable<Segment> segments, Guid networkId);
    }
}
