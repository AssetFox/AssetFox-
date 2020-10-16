using System;
using System.Collections.Generic;
using LiteDB;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities
{
    public class AggregatedResultEntity<T> : IAggregatedResultEntity
    {
        public AggregatedResultEntity()
        {
        }

        [BsonId]
        public string Id { get; set; }

        public IEnumerable<GarbageEntity<T>> AggregatedData { get; set; }

        [BsonRef("MAINTAINABLE_ASSETS")]
        public MaintainableAssetEntity MaintainableAssetEntity { get; set; }
    }

    /// <summary>
    /// Used for LiteDB management of Tuples. LiteDB does not support BSON Tuple structures.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GarbageEntity<T>
    {
        public GarbageEntity()
        {
        }

        public AttributeEntity AttributeEntity { get; set; }

        public int Year { get; set; }

        public T Value { get; set; }
    }
}
