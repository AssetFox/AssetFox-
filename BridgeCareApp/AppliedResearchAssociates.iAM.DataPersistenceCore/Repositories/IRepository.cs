using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IRepository<T>
    {
        T Add(T entity);
        T Update(T entity);
        T Get(Guid id);
        IEnumerable<T> All();
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        void SaveChanges(); // It can go in a separate interface
    }
}
