using System;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb
{
    public class NetworkRepository : LiteDbRepository<NetworkEntity, Network>, INetworkRepository
    {
        protected override NetworkEntity ToEntity(Network domainModel)
        {
            throw new NotImplementedException();
        }

        protected override Network ToDomain(NetworkEntity dataEntity)
        {
            throw new NotImplementedException();
        }
    }
}
