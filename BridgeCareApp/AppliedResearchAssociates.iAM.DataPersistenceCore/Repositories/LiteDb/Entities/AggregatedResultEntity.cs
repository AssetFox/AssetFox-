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
        public Guid Id { get; set; }

        public IEnumerable<AttributeYearValueEntity<T>> AggregatedData { get; set; }

        [BsonRef("MAINTAINABLE_ASSETS")]
        public MaintainableAssetEntity MaintainableAssetEntity { get; set; }
    }

    /// <summary>
    /// Used for LiteDB management of Tuples. LiteDB does not support BSON Tuple structures.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AttributeYearValueEntity<T>
    {
        public AttributeYearValueEntity()
        {
        }

        public AttributeEntity AttributeEntity { get; set; }

        public int Year { get; set; }

        public T Value { get; set; }
    }
}
