using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SegmentRepository : MSSQLRepository<Segment>
    {
        public SegmentRepository(IAMContext context) : base(context)
        {
        }

        public override Segment Add(Segment segment)
        {
            return base.Add(segment);
        }

        public override IEnumerable<Segment> All()
        {
            return base.All();
        }

        public override List<Segment> AddAll(List<Segment> segments)
        {
            // TODO: mapping from segments to segmentEntity

            var segmentEntities = new List<SegmentEntity>();
            context.Segments.AddRange(segmentEntities);
            return segments;
        }
    }
}
