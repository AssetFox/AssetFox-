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
    public class NetworkRepository : MSSQLRepository<Network>, INetworkDataRepository
    {
        public NetworkRepository(IAMContext context) : base(context)
        {
        }

        public Network GetNetworkWithNoAttributeData(Guid id)
        {
            if (!context.Networks.Any(n => n.Id == id))
            {
                throw new RowNotInTableException($"Cannot find network with the given id: {id}");
            }
            // if there is no id match, it throws an exception that sequence contains no element.
            var entity = context.Networks
                .Include(n => n.MaintainableAssetEntities)
                .ThenInclude(s => s.Location)
                .Single(n => n.Id == id);

            return entity.ToDomain();
        }

        public void AddNetworkWithoutAnyData(Network network) => context.Networks.Add(network.ToEntity());

        public override Network Update(Network network)
        {
            // mapping from domain to entity
            var networkEntity = network.ToEntity();
            networkEntity.MaintainableAssetEntities = new List<MaintainableAssetEntity>();
            var id = network.Id;
            foreach (var segment in network.MaintainableAssets)
            {
                networkEntity.MaintainableAssetEntities.Add(segment.ToEntity(id));
            }

            context.Networks.Update(networkEntity);
            return base.Update(network);
        }

        public Network GetNetworkWithAllData(Guid id)
        {
            if (!context.Networks.Any(n => n.Id == id))
            {
                throw new RowNotInTableException($"Cannot find network with the given id: {id}");
            }

            var entity = context.Networks
                .Include(n => n.Segments)
                .ThenInclude(s => s.Location)
                .Include(n => n.Segments)
                .ThenInclude(s => s.AttributeData)
                .ThenInclude(a => a.Attribute)
                .Single(n => n.Id == id);

            return entity.ToDomain();
        }
    }
}
