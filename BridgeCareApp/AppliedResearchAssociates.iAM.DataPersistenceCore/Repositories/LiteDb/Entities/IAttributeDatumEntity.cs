using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities
{
    public interface IAttributeDatumEntity
    {
        string Discriminator { get; set; }
        LocationEntity LocationEntity { get; set; }
        AttributeEntity AttributeEntity { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
