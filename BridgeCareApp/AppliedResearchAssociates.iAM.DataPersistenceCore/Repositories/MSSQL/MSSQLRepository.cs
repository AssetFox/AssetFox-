using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public abstract class MSSQLRepository<T>
        : IRepository<T> where T : class
    {
        protected DbContext context;
        public MSSQLRepository(IAMContext context)
        {
            this.context = context;
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
            context.SaveChanges();
        }

        public virtual T Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
