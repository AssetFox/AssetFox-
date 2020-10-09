using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities
{
    public interface IAttributeDatumEntity
    {
        string Discriminator { get; set; }
    }
}
