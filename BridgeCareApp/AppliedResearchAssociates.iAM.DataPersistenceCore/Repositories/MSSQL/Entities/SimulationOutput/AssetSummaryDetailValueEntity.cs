using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.SimulationOutput
{
    public class AssetSummaryDetailValueEntity: SimulationOutputValueEntity
    {
        public Guid AssetSummaryDetailId { get; set; }

        public virtual AssetSummaryDetailEntity AssetSummary { get; set; }
    }
}
