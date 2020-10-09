using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities
{
    public class NetworkEntity
    {
        public NetworkEntity()
        {

        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<MaintainableAssetEntity> MaintainableAssetEntities { get; set; }
    }
}
