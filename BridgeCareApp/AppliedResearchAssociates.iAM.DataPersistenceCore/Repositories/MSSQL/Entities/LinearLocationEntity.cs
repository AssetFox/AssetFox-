using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class LinearLocationEntity : LocationEntity
    {
        public double Start { get; set; }
        public double End { get; set; }

        public Guid RouteId { get; set; }
        [ForeignKey("RouteId")]
        public virtual RouteEntity Route { get; set; }
    }
}
