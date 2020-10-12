using System;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.LiteDb.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;
using LiteDB;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb
{
    public class NetworkRepository : LiteDbRepository<NetworkEntity, Network>, INetworkRepository
    {
        public override Network Get(Guid id)
        {
            using var db = new LiteDatabase(@"C:\Users\cbecker\Desktop\MyData.db");
            return db.GetCollection<NetworkEntity>("NETWORKS").FindById(id).ToDomain();
        }

        public override void Add(Network datum)
        {
            using var db = new LiteDatabase(@"C:\Users\cbecker\Desktop\MyData.db");
            var networkCollection = db.GetCollection<NetworkEntity>("NETWORKS");
            networkCollection.Insert(datum.ToEntity());
        }

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
