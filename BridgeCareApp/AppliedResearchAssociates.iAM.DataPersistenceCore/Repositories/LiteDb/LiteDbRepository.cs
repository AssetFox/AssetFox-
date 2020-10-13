﻿using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb
{
    public abstract class LiteDbRepository<TEntity, TDomain> : IRepository<TDomain>
    {
        protected LiteDbContext Context { get; }

        public LiteDbRepository(LiteDbContext context)
        {
            Context = context;
        }
        public virtual void Add(TDomain datum)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public virtual void Update(TDomain datum)
        {
            throw new NotImplementedException();
        }

        protected abstract TEntity ToEntity(TDomain domainModel);

        protected abstract TDomain ToDomain(TEntity dataEntity);
    }
}
