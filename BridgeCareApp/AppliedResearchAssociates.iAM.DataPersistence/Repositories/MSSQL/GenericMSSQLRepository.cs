using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistence.Repositories.MSSQL
{
    public abstract class GenericMSSQLRepository<T>
        : IRepository<T> where T : class
    {
        //protected DbContext context;
        public GenericMSSQLRepository(BridgeCareContext context)
        {
            //this.context = context;
        }
        // If we ll use entity framework for MS SQL, then we ll get `context` to do operation on the database
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

        public virtual T Get(Guid id)
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
