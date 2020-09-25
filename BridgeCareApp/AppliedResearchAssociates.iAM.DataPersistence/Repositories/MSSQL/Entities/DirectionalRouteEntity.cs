using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataPersistence.Repositories.MSSQL.Entities
{
    public class DirectionalRouteEntity : RouteEntity
    {
        public Direction Direction { get; set; }
    }
}
