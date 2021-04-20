using System;
using AppliedResearchAssociates.iAM.DataMiner;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract
{
    public abstract class LocationEntity : BaseEntity
    {
        protected LocationEntity() {}

        protected LocationEntity(Guid id, string discriminator, string locationIdentifier)
        {
            Id = id;
            Discriminator = discriminator;
            LocationIdentifier = locationIdentifier;
        }

        public Guid Id { get; set; }

        public string Discriminator { get; set; }

        public string LocationIdentifier { get; set; }

        public double? Start { get; set; }

        public double? End { get; set; }

        public Direction? Direction { get; set; }
    }
}
