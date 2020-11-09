using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using EFCore.BulkExtensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class NetworkRepository : MSSQLRepository, INetworkRepository
    {
        public NetworkRepository(IAMContext context) : base(context) { }

        public void CreateNetwork(DataAssignment.Networking.Network network)
        {
            // prevent EF from attempting to create the network's child entities (create them
            // separately as part of a bulk insert)
            Context.Networks.Add(new NetworkEntity
            {
                Id = network.Id,
                Name = network.Name
            });

            // convert maintainable assets and all child domains to entities
            var maintainableAssetEntities = network.MaintainableAssets.Select(_ => _.ToEntity(network.Id)).ToList();

            // bulk insert maintainable assets
            Context.BulkInsert(maintainableAssetEntities);

            // bulk insert maintainable asset locations
            Context.BulkInsert(maintainableAssetEntities.Select(_ => _.MaintainableAssetLocation).ToList());

            Context.SaveChanges();
        }

        public IEnumerable<DataAssignment.Networking.Network> GetAllNetworks()
        {
            if (Context.Networks.Count() == 0)
            {
                throw new RowNotInTableException($"Cannot find networks in the database");
            }

            // consumer of this call will only need the network information. Not the maintainable assest information
            return Context.Networks.Select(_ => _.ToDomain()).ToList();
        }
    }
}
