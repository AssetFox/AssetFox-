using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities
{
    public class RouteEntity
    {
        public RouteEntity()
        {

        }
        public string Discriminator { get; set; }
        public Direction Direction { get; set; }

        public string UniqueIdentifier { get; set; }
    }
}
