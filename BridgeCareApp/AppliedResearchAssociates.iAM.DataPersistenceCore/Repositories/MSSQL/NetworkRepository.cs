using System;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.MSSQL.Mappings;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class NetworkRepository : MSSQLRepository<Network>
    {
        public NetworkRepository(IAMContext context) : base(context) { }

        public override void Add(Network network) => context.Networks.Add(network.ToEntity());

        public override Network Get(Guid id)
        {
            if (!context.Networks.Any(n => n.Id == id))
            {
                throw new RowNotInTableException($"Cannot find network with the given id: {id}");
            }

            var entity = context.Networks
                .Include(n => n.MaintainableAssets)
                .ThenInclude(m => m.Location)
                .Include(n => n.MaintainableAssets)
                .ThenInclude(m => m.AttributeData)
                .ThenInclude(a => a.Attribute)
                .Include(n => n.MaintainableAssets)
                .ThenInclude(m => m.AttributeData)
                .ThenInclude(a => a.Location)
                .Single(n => n.Id == id);

            return entity.ToDomain();
        }
    }
}
