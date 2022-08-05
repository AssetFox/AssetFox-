using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class AssetSummaryDetailMapper
    {
        public static void MapAssetSummaryDetailProperties(AssetSummaryDetail domain, AssetSummaryDetailEntity entity, Guid simulationOutputId)
        {

            entity.AssetName = domain.AssetName;
            entity.SimulationOutputId = simulationOutputId; 
            // be a subclass of this one.
            entity.Id = Guid.NewGuid();
        }
    }
}
