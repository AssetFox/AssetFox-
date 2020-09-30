using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings.Domain_to_Entity
{
    public static class NetworkDomainToNetworkEntity
    {
        public static NetworkEntity CreateFromDomain(Network network) =>
            new NetworkEntity
            {
                Id = network.Guid,
                Name = network.Name
            };

        public static NetworkEntity UpdateFromDomain(NetworkEntity networkEntity, Network network)
        {
            networkEntity.Name = network.Name;
            return networkEntity;
        }
    }
}
