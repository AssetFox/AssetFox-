﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.SimulationOutput
{
    public class AssetDetailValueEntity: SimulationOutputValueEntity
    {
        public Guid AssetDetailId { get; set; }
        public virtual AssetDetailEntity AssetDetail { get; set; }
    }
}
