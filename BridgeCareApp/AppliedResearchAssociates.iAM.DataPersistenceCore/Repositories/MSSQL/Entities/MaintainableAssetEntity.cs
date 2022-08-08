using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class MaintainableAssetEntity : BaseEntity
    {
        public MaintainableAssetEntity()
        {
            AggregatedResults = new HashSet<AggregatedResultEntity>();
            AssetDetails = new HashSet<AssetDetailEntity>();
            AssignedData = new HashSet<AttributeDatumEntity>();
            AssetSummaryDetails = new HashSet<AssetSummaryDetailEntity>();
        }

        public Guid Id { get; set; }

        public Guid NetworkId { get; set; }

        public string AssetName { get; set; }

        public string SpatialWeighting { get; set; }

        public virtual NetworkEntity Network { get; set; }

        public virtual MaintainableAssetLocationEntity MaintainableAssetLocation { get; set; }

        public virtual ICollection<AggregatedResultEntity> AggregatedResults { get; set; }

        public virtual ICollection<AttributeDatumEntity> AssignedData { get; set; }

        public virtual ICollection<CommittedProjectEntity> CommittedProjects { get; set; }

        public virtual ICollection<AssetDetailEntity> AssetDetails { get; set; }

        public virtual ICollection<AssetSummaryDetailEntity> AssetSummaryDetails { get; set; }
    }
}
