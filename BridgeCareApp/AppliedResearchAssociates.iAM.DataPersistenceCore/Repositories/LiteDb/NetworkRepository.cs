using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.LiteDb.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using MSSQLEntities = AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb
{
    public class NetworkRepository : LiteDbRepository, INetworkRepository
    {
        public NetworkRepository(ILiteDbContext context) : base(context) { }

        public void CreateNetwork(DataAssignment.Networking.Network datum)
        {
            var locationCollection = Context.Database.GetCollection<LocationEntity>("LOCATIONS");
            locationCollection.InsertBulk(datum.MaintainableAssets.Select(_ => _.Location.ToEntity()));

            var maintainableAssetCollection = Context.Database.GetCollection<MaintainableAssetEntity>("MAINTAINABLE_ASSETS");
            maintainableAssetCollection.InsertBulk(datum.MaintainableAssets.Select(_ => _.ToEntity()));

            var networkCollection = Context.Database.GetCollection<NetworkEntity>("NETWORKS");
            networkCollection.Insert(datum.ToEntity());
        }

        public void CreateNetwork(Network network) => throw new NotImplementedException();
        public Task<List<NetworkDTO>> Networks() => throw new NotImplementedException();
        public List<DataAssignment.Networking.Network> GetAllNetworks() => throw new NotImplementedException();
        public MSSQLEntities.NetworkEntity GetPennDotNetwork() => throw new NotImplementedException();
        public bool CheckPennDotNetworkHasData() => throw new NotImplementedException();
        public Network GetSimulationAnalysisNetwork(Guid networkId, Explorer explorer, bool areFacilitiesRequired = true) => throw new NotImplementedException();
        public void DeleteNetworkData() => throw new NotImplementedException();
    }
}
