using System;
using System.ComponentModel.DataAnnotations;
using AppliedResearchAssociates.iAM.DataMiner;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class RouteEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Discriminator { get; set; }

        public string UniqueIdentifier { get; set; }

        public Direction? Direction { get; set; }

        public virtual LocationEntity Location { get; set; }
    }
}
