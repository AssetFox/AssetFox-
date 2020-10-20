using System;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.MSSQL.Mappings;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class NetworkRepository : MSSQLRepository, INetworkRepository
    {
        public NetworkRepository(IAMContext context) : base(context) { }

        public void CreateNetwork(Network network)
        {
            Context.Networks.Add(network.ToEntity());
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
                .ThenInclude(m => m.Location)
                .Single(n => n.Id == id);

            return entity.ToDomain();
        }
    }
}
