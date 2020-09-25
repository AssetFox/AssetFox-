using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class RouteEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid LinearLocationId { get; set; }

        [ForeignKey("LinearLocationId")]
        public virtual LinearLocationEntity LinearLocation { get; set; }
    }
}
