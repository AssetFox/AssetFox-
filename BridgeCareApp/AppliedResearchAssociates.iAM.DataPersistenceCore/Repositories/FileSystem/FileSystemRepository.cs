using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.FileSystem
{
    public abstract class FileSystemRepository<TDomain> : IRepository<TDomain> where TDomain : class
    {
        public virtual void Add(TDomain datum)
        {
            throw new NotImplementedException();
        }

        public virtual void AddAll(IEnumerable<TDomain> data, params object[] args)
        {
            throw new NotImplementedException();
        }

        public virtual TDomain Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<TDomain> All()
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<TDomain> Find(params object[] args)
        {
            throw new NotImplementedException();
        }

        public virtual void Update(TDomain datum)
        {
            throw new NotImplementedException();
        }

        public virtual void Delete(TDomain datum)
        {
            throw new NotImplementedException();
        }
    }
}
