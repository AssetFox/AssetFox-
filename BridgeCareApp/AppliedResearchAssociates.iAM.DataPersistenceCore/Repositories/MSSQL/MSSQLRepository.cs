using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public abstract class MSSQLRepository<TDomain, TEntity>
        : IRepository<TDomain> where TDomain : class
    {
        protected IAMContext context;
        public MSSQLRepository(IAMContext context)
        {
            this.context = context;
        }

        public virtual IEnumerable<TDomain> All()
        {
            return context.Set<TDomain>().ToList();
        }

        public virtual IEnumerable<TDomain> Find(Expression<Func<TDomain, bool>> predicate)
        {
            return context.Set<TDomain>()
                .AsQueryable()
                .Where(predicate)
                .ToList();
        }

        public virtual TDomain Get(Guid id)
        {
            return context.Find<TDomain>(id);
        }

        public virtual TDomain Update(TDomain entity)
        {
            return context.Update(entity).Entity;
        }

        public virtual List<TDomain> AddAll(List<TDomain> entities)
        {
            context.AddRange(entities);
            return entities;
        }
    }
}
