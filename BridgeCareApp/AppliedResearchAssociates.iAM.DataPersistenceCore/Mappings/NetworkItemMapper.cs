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
        public static Network ToDomain(this NetworkEntity entity) =>
            entity == null
                ? new Network(new List<Segment>(), Guid.NewGuid())
                : new Network(
                    entity.SegmentEntities == null
                        ? new List<Segment>()
                        : entity.SegmentEntities.Select(e => e.ToDomain()).ToList(),
                    entity.Id,
                    entity.Name);

        public static NetworkEntity ToEntity(this Network domain)
        {
            if (domain == null)
            {
                return new NetworkEntity {Id = Guid.NewGuid(), Name = ""};
            }
            
            return new NetworkEntity {Id = domain.Id, Name = domain.Name};
        }
        
        public static void UpdateEntity(this NetworkEntity entity, Network domain) => entity.Name = domain.Name;
    }
}
