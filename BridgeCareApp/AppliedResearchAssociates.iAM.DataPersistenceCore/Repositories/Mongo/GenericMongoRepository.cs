using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Mongo
{
    public class GenericMongoRepository<T>
        : IRepository<T> where T : class
    {
        /*public GenericMongoRepository(MongoDriverContext context)
        {
            //this.context = context;
        }*/
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
