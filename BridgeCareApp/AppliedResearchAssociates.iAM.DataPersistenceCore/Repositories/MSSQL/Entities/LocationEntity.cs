using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public abstract class LocationEntity
    {
        protected LocationEntity(Guid id, string discriminator, string uniqueIdentifier)
        {
            Id = id;
            Discriminator = discriminator;
            UniqueIdentifier = uniqueIdentifier;
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Discriminator { get; set; }

        [Required]
        public string UniqueIdentifier { get; set; }

        public double? Start { get; set; }

        public double? End { get; set; }

        public Direction? Direction { get; set; }
    }
}
