using System;
using System.Collections.Generic;
using LiteDB;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities
{
    public class MaintainableAssetEntity
    {
        public MaintainableAssetEntity()
        {
        }
        [BsonId]
        public Guid Id { get; set; }

        public List<IAttributeDatumEntity> AttributeDatumEntities { get; set; }

        [BsonRef("LOCATIONS")]
        public LocationEntity LocationEntity { get; set; }
    }
}
