using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SegmentRepository : MSSQLRepository<SegmentEntity>
    {
        public SegmentRepository(IAMContext context) : base(context)
        {
        }

        public override SegmentEntity Add(SegmentEntity entity)
        {
            return base.Add(entity);
        }

        public override IEnumerable<SegmentEntity> All()
        {
            return base.All();
        }

        public override IEnumerable<SegmentEntity> Find(Expression<Func<SegmentEntity, bool>> predicate)
        {
            return base.Find(predicate);
        }

        public override SegmentEntity Get(Guid id)
        {
            return base.Get(id);
        }

        public override SegmentEntity Update(SegmentEntity entity)
        {
            return base.Update(entity);
        }

        public void AddAll(List<SegmentEntity> segments)
        {
            context.AddRange(segments);
            //return true;
        }
    }
}
