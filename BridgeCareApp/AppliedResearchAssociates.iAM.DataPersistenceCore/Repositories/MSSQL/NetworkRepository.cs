using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class NetworkRepository : MSSQLRepository<Network, NetworkEntity>, INetworkDataRepository
    {
        public NetworkRepository(IAMContext context) : base(context)
        {
        }

        public Network GetNetworkWithNoAttributeData(Guid id)
        {
            var entity = context.Networks.Find(id);

            // mapping from entity to network domain object
            var segments = new List<Segment>();
            foreach (var segmentEntity in entity.SegmentEntities)
            {
                var segment = new Segment(LocationEntityToLocation.CreateFromEntity(segmentEntity.LocationEntity));
                segments.Add(segment);
            }

            var network = new Network(segments, id);
            return network;
        }

        public Network AddNetworkWithoutAnyData(Network network)
        {
            // this mapping can go in a service
            var networkEntity = new NetworkEntity { Id = network.Guid, Name = network.Name };
            context.Networks.Add(networkEntity);
            return network;

        }

        public override Network Update(Network network)
        {
             var networkEntity = context.Networks.Find(network.Guid);
            // mapping from domain to entity

            context.Networks.Update(networkEntity);
            return base.Update(network);
        }

        protected override NetworkEntity ToDataEntity(Network domainModel)
        {
            // I think the mapping will go here and the controller will call this method passing in the domain object
            // Eg. NetworkRepository.ToDataEntity(networkDomainObject)
            throw new NotImplementedException();
        }

        protected override Network ToDomainModel(NetworkEntity dataEntity)
        {
            throw new NotImplementedException();
        }
    }
}
