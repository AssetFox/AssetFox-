using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb
{
    public class AttributeDatumRepository<T> : LiteDbRepository<IAttributeDatum, IAttributeDatum>, IAttributeDatumRepository
    {
        public void AddAttributeData(IEnumerable<IAttributeDatum> domainAttributeData, Guid maintainableAssetId)
        {
            throw new NotImplementedException();
        }

        protected override IAttributeDatum ToDomain(IAttributeDatum dataEntity)
        {
            throw new NotImplementedException();
        }

        protected override IAttributeDatum ToEntity(IAttributeDatum domainModel)
        {
            throw new NotImplementedException();
        }
    }
}
