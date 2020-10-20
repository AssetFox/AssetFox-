using System;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.LiteDb.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb
{
    public class NetworkRepository : LiteDbRepository, INetworkRepository
    {
        public NetworkRepository(ILiteDbContext context) : base(context) { }

        public void CreateNetwork(Network datum)
        {
            var locationCollection = Context.Database.GetCollection<LocationEntity>("LOCATIONS");
            locationCollection.InsertBulk(datum.MaintainableAssets.Select(_ => _.Location.ToEntity()));

            var maintainableAssetCollection = Context.Database.GetCollection<MaintainableAssetEntity>("MAINTAINABLE_ASSETS");
            maintainableAssetCollection.InsertBulk(datum.MaintainableAssets.Select(_ => _.ToEntity()));

            var networkCollection = Context.Database.GetCollection<NetworkEntity>("NETWORKS");
            networkCollection.Insert(datum.ToEntity());
        }

        public Network GetNetworkWithAssetsAndLocations(Guid id) =>
            Context.Database.GetCollection<NetworkEntity>("NETWORKS")
                .Include(_ => _.MaintainableAssetEntities.ToList())
                .Include(_ => _.MaintainableAssetEntities.Select(_ => _.LocationEntity))
                .FindById(id)
                .ToDomain();
    }
}
