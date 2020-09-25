using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistence.Repositories.MSSQL.Entities
{
    public class LinearLocationEntity : LocationEntity
    {
        public double Start { get; set; }
        public double End { get; set; }

        public virtual RouteEntity Route { get; set; }
    }
}
