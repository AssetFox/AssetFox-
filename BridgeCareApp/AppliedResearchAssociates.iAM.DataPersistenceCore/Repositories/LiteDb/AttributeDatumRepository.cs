using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;
using LiteDB;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb
{
    public class AttributeDatumRepository<T> : LiteDbRepository<IAttributeDatum, IAttributeDatum>, IAttributeDatumRepository
    {
        public bool DeleteAssignedDataFromNetwork(Guid networkId)
        {
            throw new NotImplementedException();
        }
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
