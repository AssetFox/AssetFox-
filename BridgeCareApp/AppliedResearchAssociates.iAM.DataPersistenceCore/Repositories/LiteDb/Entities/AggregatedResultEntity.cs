using System;
using System.Collections.Generic;
using System.Text;
using LiteDB;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities
{
    public class AggregatedResultEntity<T>
    {
        public AggregatedResultEntity()
        {

        }
        [BsonId]
        public Guid Id { get; set; }
        public IEnumerable<GarbageEntity<T>> AggregatedData { get; set; }
        [BsonRef("MAINTAINABLE_ASSETS")]
        public MaintainableAssetEntity MaintainableAssetEntity { get; set; }
    }
}
