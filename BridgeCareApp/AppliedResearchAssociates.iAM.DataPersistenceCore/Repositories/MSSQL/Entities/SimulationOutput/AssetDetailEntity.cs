using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class AssetDetailEntity: AssetSummaryDetailEntity
    {
        public Guid SimulationYearDetailId { get; set; }

        public virtual SimulationYearDetailEntity SimulationYearDetail { get; set; }

        public string AppliedTreatment { get; set; }

        public TreatmentCause TreatmentCause { get; set; }

        public ICollection<TreatmentConsiderationDetailEntity> TreatmentConsiderations { get; } = new HashSet<TreatmentConsiderationDetailEntity>();

        public bool TreatmentFundingIgnoresSpendingLimit { get; set; }

        public ICollection<TreatmentOptionDetailEntity> TreatmentOptions { get; } = new HashSet<TreatmentOptionDetailEntity>();

        public ICollection<TreatmentRejectionDetailEntity> TreatmentRejections { get; } = new HashSet<TreatmentRejectionDetailEntity>();

        public ICollection<TreatmentSchedulingCollisionDetailEntity> TreatmentSchedulingCollisions { get; } = new HashSet<TreatmentSchedulingCollisionDetailEntity>();

        public TreatmentStatus TreatmentStatus { get; set; }

    }
}
