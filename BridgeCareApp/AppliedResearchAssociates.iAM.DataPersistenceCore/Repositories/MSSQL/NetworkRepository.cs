using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class NetworkRepository : MSSQLRepository<Network, NetworkEntity>, INetworkDataRepository
    {
        public NetworkRepository(IAMContext context) : base(context)
        {
        }

        public Network GetNetworkWithNoAttributeData(Guid id)
        {
            var entity = context.Networks
                .Include(n => n.SegmentEntities)
                .Include(n => n.SegmentEntities.Select(s => s.Location).FirstOrDefault())
                .Single(n => n.Id == id);

            return entity.ToDomain();
        }

        public void AddNetworkWithoutAnyData(Network network) => context.Networks.Add(network.ToEntity());

        public override Network Update(Network network)
        {
             //var networkEntity = context.Networks.Find(network.Id);
            // mapping from domain to entity
            var networkEntity = network.ToEntity();
            var id = network.Id;
            foreach (var segment in network.Segments)
            {
                networkEntity.SegmentEntities.Add(segment.ToEntity(id));
            }

            context.Networks.Update(networkEntity);
            return base.Update(network);
        }
    }
}
