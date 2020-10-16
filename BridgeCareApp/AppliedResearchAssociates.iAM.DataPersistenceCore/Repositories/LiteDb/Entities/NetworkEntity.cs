using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using LiteDB;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities
{
    public class NetworkEntity
    {
        public NetworkEntity()
        {

        }

        public string Id { get; set; }
        public string Name { get; set; }

        [BsonRef("MAINTAINABLE_ASSETS")]
        public ICollection<MaintainableAssetEntity> MaintainableAssetEntities { get; set; }
    }
}
