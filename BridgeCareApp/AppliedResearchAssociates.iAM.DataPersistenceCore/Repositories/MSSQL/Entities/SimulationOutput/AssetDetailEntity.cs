using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class AssetDetailEntity: BaseEntity
    {
        // This is the entity that was formerly a subclass.
        public Guid Id { get; set; }

        public string AssetName { get; set; }

        public Guid SimulationYearDetailId { get; set; }

        public virtual SimulationYearDetailEntity SimulationYearDetail { get; set; }

        public string AppliedTreatment { get; set; }

        public int TreatmentCause { get; set; }

        public ICollection<AssetDetailValueEntity> AssetDetailValues { get; set; } = new HashSet<AssetDetailValueEntity>();

        public ICollection<TreatmentConsiderationDetailEntity> TreatmentConsiderationDetails { get; set; } = new HashSet<TreatmentConsiderationDetailEntity>();

        public bool TreatmentFundingIgnoresSpendingLimit { get; set; }

        public virtual ICollection<AssetSummaryDetailValueEntity> AssetSummaryDetailValues { get; set; } = new HashSet<AssetSummaryDetailValueEntity>();

        public ICollection<TreatmentOptionDetailEntity> TreatmentOptionDetails { get; set; } = new HashSet<TreatmentOptionDetailEntity>();

        public ICollection<TreatmentRejectionDetailEntity> TreatmentRejectionDetails { get; set; } = new HashSet<TreatmentRejectionDetailEntity>();

        public ICollection<TreatmentSchedulingCollisionDetailEntity> TreatmentSchedulingCollisionDetails { get; set; } = new HashSet<TreatmentSchedulingCollisionDetailEntity>();

        public int TreatmentStatus { get; set; }

    }
}
