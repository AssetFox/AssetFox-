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
        public Guid Id { get; set; }
        public LocationEntity Location { get; set; }
        public AttributeEntity Attribute { get; set; }
        public T Value { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Discriminator { get; set; }
    }
}
