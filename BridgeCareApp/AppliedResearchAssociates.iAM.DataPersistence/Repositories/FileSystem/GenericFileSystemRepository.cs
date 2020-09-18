using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistence.Repositories.FileSystem
{
    public abstract class GenericFileSystemRepository<T>
        : IRepository<T> where T : class
    {
        public virtual T Add(T entity)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<T> All()
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public T Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }

        public virtual T Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
