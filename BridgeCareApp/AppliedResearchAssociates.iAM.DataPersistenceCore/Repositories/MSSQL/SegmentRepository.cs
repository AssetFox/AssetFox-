using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Interfaces;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SegmentRepository : MSSQLRepository<Segment, SegmentEntity>, ISegmentDataRepository
    {
        public SegmentRepository(IAMContext context) : base(context)
        {
        }

        public void AddNetworkSegments(IEnumerable<Segment> segments, Guid networkId) => context.Segments.AddRange(segments.Select(d => d.ToEntity(networkId)));
    }
}
