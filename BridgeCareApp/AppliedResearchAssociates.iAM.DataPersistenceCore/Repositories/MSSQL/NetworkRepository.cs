using System;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Extensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class NetworkRepository : MSSQLRepository, INetworkRepository
    {
        public NetworkRepository(IAMContext context) : base(context) { }

        public void CreateNetwork(Network network)
        {
            // prevent EF from attempting to create the network's child entities (create them separately as part of a bulk insert)
            Context.Networks.Add(new NetworkEntity
            {
                Id = network.Id,
                Name = network.Name
            });

            // convert maintainable asset and all child domains to entities
            var maintainableAssetEntities = network.MaintainableAssets.Select(_ => _.ToEntity(network.Id)).ToList();

            // bulk insert maintainable assets
            Context.BulkInsert(maintainableAssetEntities);

            // bulk insert maintainable asset locations
            Context.BulkInsert(maintainableAssetEntities.Select(_ => _.MaintainableAssetLocation).ToList());

            Context.SaveChanges();
        }

        public Network GetNetworkWithAssetsAndLocations(Guid id)
        {
            if (!Context.Networks.Any(n => n.Id == id))
            {
                throw new RowNotInTableException($"Cannot find network with the given id: {id}");
            }

            var entity = Context.Networks
                .Include(n => n.MaintainableAssets)
                .ThenInclude(m => m.MaintainableAssetLocation)
                .Single(n => n.Id == id);

            return entity.ToDomain();
        }
    }
}
