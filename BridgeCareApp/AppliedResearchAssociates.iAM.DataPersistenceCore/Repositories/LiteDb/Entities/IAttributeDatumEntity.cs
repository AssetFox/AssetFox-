using System;

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
