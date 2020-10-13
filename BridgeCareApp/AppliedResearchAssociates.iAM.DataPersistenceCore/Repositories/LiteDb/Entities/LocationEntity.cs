using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner;
using LiteDB;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities
{
    public class LocationEntity
    {
        public LocationEntity()
        {

        }
        [BsonId]
        public Guid Id { get; set; }
        public string Discriminator { get; set; }

        public string UniqueIdentifier { get; set; }

        public double? Start { get; set; }

        public double? End { get; set; }

        public RouteEntity RouteEntity { get; set; }


    }
}
