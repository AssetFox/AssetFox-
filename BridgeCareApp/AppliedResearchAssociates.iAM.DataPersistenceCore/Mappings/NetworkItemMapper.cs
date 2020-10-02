using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings
{
    public static class NetworkItemMapper
    {
        public static NetworkEntity CreateFromDomain(Network network)
        {
            if (network == null)
            {
                return new NetworkEntity
                {
                    Id = Guid.NewGuid(),
                    Name = ""
                };
            }
            
            return new NetworkEntity
            {
                Id = network.Id,
                Name = network.Name
            };
        }
        
        public static NetworkEntity UpdateFromDomain(this NetworkEntity networkEntity, Network network)
        {
            networkEntity.Name = network.Name;
            return networkEntity;
        }

        /*public static Network CreateFromEntity(this NetworkEntity entity)
        {
            return entity == null
                ? new Network(new List<Segment>(), Id.NewGuid())
                : new Network(
                    entity.SegmentEntities,
                    entity.Id,
                    entity.Name
                );
        }*/
    }
}
