using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public abstract class MSSQLRepository<TDomain> : IRepository<TDomain> where TDomain : class
    {
        protected IAMContext context;

        public MSSQLRepository(IAMContext context)
        {
            this.context = context;
        }

        public virtual void Add(TDomain datum)
        {
            context.Add(datum);
        }

        public virtual void AddAll(IEnumerable<TDomain> data, params object[] args)
        {
            context.AddRange(data);
        }

        public virtual TDomain Get(Guid id)
        {
            return context.Find<TDomain>(id);
        }

        public virtual IEnumerable<TDomain> All()
        {
            return context.Set<TDomain>();
        }

        public virtual IEnumerable<TDomain> Find(params object[] args)
        {
            if (!args.Any())
            {
                throw new ArgumentNullException("No clause(s) found for query");
            }

            var result = context.Set<TDomain>()
                .AsQueryable();

            foreach (var predicate in args)
            {
                var convertedPredicate = (Expression<Func<TDomain, bool>>)Convert.ChangeType(predicate,
                    typeof(Expression<Func<TDomain, bool>>));

                result = result.Where(convertedPredicate);
            }

            return result;
        }

        public virtual void Update(TDomain datum)
        {
            context.Update(datum);
        }

        public virtual void Delete(TDomain datum)
        {
            context.Entry(datum).State = EntityState.Deleted;
        }
    }
}
