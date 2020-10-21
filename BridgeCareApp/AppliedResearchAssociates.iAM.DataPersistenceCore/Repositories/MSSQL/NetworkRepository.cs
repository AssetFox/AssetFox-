using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class NetworkRepository : MSSQLRepository<Network>
    {
        public NetworkRepository(IAMContext context) : base(context) { }

        public override Guid Add(Network network) => context.Networks.Add(network.ToEntity()).Entity.Id;

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

        public override IEnumerable<Network> All()
        {
            if (context.Networks.Count() == 0)
            {
                throw new RowNotInTableException($"Cannot find networks in the database");
            }

            // consumer of this call will only need the network information. Not the maintainable assest information
            var entities = context.Networks.ToList();

            var networks = new List<Network>();
            foreach (var entity in entities)
            {
                networks.Add(entity.ToDomain());
            }
            return networks;
        }
    }
}
