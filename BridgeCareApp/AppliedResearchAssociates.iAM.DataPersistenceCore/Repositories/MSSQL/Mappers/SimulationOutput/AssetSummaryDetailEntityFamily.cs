using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AssetSummaryDetailEntityFamily
    {
        public List<AssetSummaryDetailValueEntityIntId> AssetSummaryDetailValues { get; set; } = new List<AssetSummaryDetailValueEntityIntId>();
        public List<AssetSummaryDetailEntity> AssetSummaryDetails { get; set; } = new List<AssetSummaryDetailEntity>();
    }
}
