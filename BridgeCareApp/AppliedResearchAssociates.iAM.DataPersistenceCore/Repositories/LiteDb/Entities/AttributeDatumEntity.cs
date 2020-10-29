using System;
using LiteDB;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities
{
    public class AttributeDatumEntity<T> : IAttributeDatumEntity
    {
        public AttributeDatumEntity()
        {
        }

        [BsonId]
        public Guid Id { get; set; }

        public LocationEntity LocationEntity { get; set; }

        public AttributeEntity AttributeEntity { get; set; }

        public T Value { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Discriminator { get; set; }
    }
}
