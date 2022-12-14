using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class AssetSummaryDetailValueEntityIntId: SimulationOutputValueEntityIntId
    {
        public Guid AssetSummaryDetailId { get; set; }

        public virtual AssetSummaryDetailEntity AssetSummaryDetail { get; set; }
    }
}
