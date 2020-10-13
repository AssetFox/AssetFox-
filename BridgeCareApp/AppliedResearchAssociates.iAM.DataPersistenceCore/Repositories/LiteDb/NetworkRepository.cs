using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.LiteDb.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;
using LiteDB;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb
{
    public class NetworkRepository : LiteDbRepository<NetworkEntity, Network>, INetworkRepository
    {
        public NetworkRepository(LiteDbContext context) : base(context)
        {

        }
        public override Network Get(Guid id)
        {
            return Context.Database.GetCollection<NetworkEntity>("NETWORKS")
                .Include(_ => _.MaintainableAssetEntities.ToList())
                .Include(_ => _.MaintainableAssetEntities.Select(_ => _.LocationEntity))
                .FindById(id)
                .ToDomain();
        }

        public override void Add(Network datum)
        {
            var locationCollection = Context.Database.GetCollection<LocationEntity>("LOCATIONS");
            locationCollection.InsertBulk(datum.MaintainableAssets.Select(_ => _.Location.ToEntity()));

            var maintainableAssetCollection = Context.Database.GetCollection<MaintainableAssetEntity>("MAINTAINABLE_ASSETS");
            maintainableAssetCollection.InsertBulk(datum.MaintainableAssets.Select(_ => _.ToEntity()));


            var networkCollection = Context.Database.GetCollection<NetworkEntity>("NETWORKS");
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
