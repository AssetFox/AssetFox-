using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities
{
    public class AttributeDatumEntity<T> : IAttributeDatumEntity
    {
        public AttributeDatumEntity()
        {

        }
        public string Id { get; set; }
        public LocationEntity LocationEntity { get; set; }
        public AttributeEntity AttributeEntity { get; set; }
        public T Value { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Discriminator { get; set; }
    }
}
