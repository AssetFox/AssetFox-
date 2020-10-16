using System;
using System.Collections.Generic;
using System.Text;
using LiteDB;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities
{
    public interface IAggregatedResultEntity
    {

        [BsonId]
        string Id { get; set; }


        [BsonRef("MAINTAINABLE_ASSETS")]
        public MaintainableAssetEntity MaintainableAssetEntity { get; set; }
    }
}
