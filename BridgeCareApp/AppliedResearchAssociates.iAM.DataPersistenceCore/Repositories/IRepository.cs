using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IRepository<TDomain>
    {
        List<TDomain> AddAll(List<TDomain> data);
        TDomain Update(TDomain datum);
        TDomain Get(Guid id);
        IEnumerable<TDomain> All();
        IEnumerable<TDomain> Find(Expression<Func<TDomain, bool>> predicate);
    }
}
