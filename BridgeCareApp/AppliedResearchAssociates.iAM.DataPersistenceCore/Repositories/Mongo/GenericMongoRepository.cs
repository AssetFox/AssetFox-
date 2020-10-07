using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Mongo
{
    public class GenericMongoRepository<TDomain, TEntity>
        : IRepository<TDomain>
    {
        /*public GenericMongoRepository(MongoDriverContext context)
        {
            //this.context = context;
        }*/

        public virtual TDomain Add(TDomain entity)
        {
            throw new NotImplementedException();
        }

        public List<TDomain> AddAll(List<TDomain> entity)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<TDomain> All()
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<TDomain> Find(Expression<Func<TDomain, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public virtual TDomain Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }

        public virtual TDomain Update(TDomain entity)
        {
            throw new NotImplementedException();
        }
    }
}
