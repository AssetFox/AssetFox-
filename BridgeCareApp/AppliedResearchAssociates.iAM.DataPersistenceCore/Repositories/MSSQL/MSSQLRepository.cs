using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public abstract class MSSQLRepository<T>
        : IRepository<T> where T : class
    {
        protected IAMContext context;
        public MSSQLRepository(IAMContext context)
        {
            this.context = context;
        }
        // If we ll use entity framework for MS SQL, then we ll get `context` to do operation on the database
        public virtual T Add(T entity)
        {
            return context.Add(entity).Entity;
        }

        public virtual IEnumerable<T> All()
        {
            return context.Set<T>().ToList();
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return context.Set<T>()
                .AsQueryable()
                .Where(predicate)
                .ToList();
        }

        public virtual T Get(Guid id)
        {
            return context.Find<T>(id);
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }

        public virtual T Update(T entity)
        {
            return context.Update(entity).Entity;
        }

        public virtual List<T> AddAll(List<T> entities)
        {
            context.AddRange(entities);
            return entities;
        }
    }
}
