using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IRepository<TDomain>
    {
        void Add(TDomain datum);

        void AddAll(IEnumerable<TDomain> data, params object[] args);

        TDomain Get(Guid id);

        IEnumerable<TDomain> All();

        IEnumerable<TDomain> Find(params object[] args);

        void Update(TDomain datum);

        void Delete(TDomain datum);
    }
}
