using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb
{
    public class NetworkRepository : LiteDbRepository<NetworkEntity, Network>, INetworkRepository
    {
        protected override NetworkEntity ToDataEntity(Network domainModel)
        {
            throw new NotImplementedException();
        }

        protected override Network ToDomainModel(NetworkEntity dataEntity)
        {
            throw new NotImplementedException();
        }
    }
}
