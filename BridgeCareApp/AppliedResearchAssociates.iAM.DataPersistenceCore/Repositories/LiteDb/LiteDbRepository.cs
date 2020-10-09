using System;
using System.Collections.Generic;
using LiteDB;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb
{
    public abstract class LiteDbRepository<TEntity, TDomain> : IRepository<TDomain>
    {
        public virtual void Add(TDomain datum)
        {
            using (var db = new LiteDatabase(@"C:\Users\cbecker\Desktop\MyData.db"))
            {
                var networkCollection = db.GetCollection<TDomain>("NETWORKS");
                networkCollection.Insert(datum);
            }
        }

        public virtual void AddAll(IEnumerable<TDomain> data, params object[] args)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<TDomain> All()
        {
            throw new NotImplementedException();
        }

        public virtual void Delete(TDomain datum)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<TDomain> Find(params object[] args)
        {
            throw new NotImplementedException();
        }

        public virtual TDomain Get(Guid id)
        {
            using (var db = new LiteDatabase(@"C:\Users\cbecker\Desktop\MyData.db"))
            {
                var networkCollection = db.GetCollection<TDomain>("NETWORKS");
                var testReturn = networkCollection.FindById(id);
                return testReturn;
            }
        }

        public virtual void Update(TDomain datum)
        {
            throw new NotImplementedException();
        }

        protected abstract TEntity ToEntity(TDomain domainModel);
        protected abstract TDomain ToDomain(TEntity dataEntity);
    }
}
