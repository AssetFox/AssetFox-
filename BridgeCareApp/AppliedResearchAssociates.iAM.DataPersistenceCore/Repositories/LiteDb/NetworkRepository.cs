using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.LiteDb.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Mappings;

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
    }
}
