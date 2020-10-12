using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities
{
    public class MaintainableAssetEntity
    {
        public MaintainableAssetEntity()
        {

        }
        public Guid Id { get; set; }
        public Guid NetworkId { get; set; }
        public List<IAttributeDatumEntity> AttributeDatumEntities { get; set; }
        public LocationEntity LocationEntity {get; set;}
    }
}
