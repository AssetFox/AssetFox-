using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class AssetSummaryDetailEntity
    {
        public AssetSummaryDetailEntity()
        {
            AssetSummaryDetailValues = new HashSet<AssetSummaryDetailValueEntity>();
        }

        public Guid Id { get; set; }

        public Guid MaintainableAssetId { get; set; }

        public virtual MaintainableAssetEntity MaintainableAsset { get; set; }

        public virtual SimulationOutputEntity SimulationOutput { get; set; }

        public Guid SimulationOutputId { get; set; }

        public virtual ICollection<AssetSummaryDetailValueEntity> AssetSummaryDetailValues { get; set; }
        public virtual ICollection<AssetSummaryDetailValueEntityIntId> AssetSummaryDetailValuesIntId { get; set; }

    }
}
