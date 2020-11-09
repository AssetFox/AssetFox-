using System;
using System.ComponentModel.DataAnnotations;
using AppliedResearchAssociates.iAM.DataMiner;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract
{
    public abstract class LocationEntity
    {
        protected LocationEntity(Guid id, string discriminator, string locationIdentifier)
        {
            Id = id;
            Discriminator = discriminator;
            LocationIdentifier = locationIdentifier;
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Discriminator { get; set; }

        [Required]
        public string LocationIdentifier { get; set; }

        public double? Start { get; set; }

        public double? End { get; set; }

        public Direction? Direction { get; set; }
    }
}
