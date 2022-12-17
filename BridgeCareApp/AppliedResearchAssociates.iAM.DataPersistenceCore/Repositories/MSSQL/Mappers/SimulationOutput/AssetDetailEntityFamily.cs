using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AssetDetailEntityFamily
    {
        public List<AssetDetailEntity> AssetDetails { get; set; } = new List<AssetDetailEntity>();
        public List<AssetDetailValueEntityIntId> AssetDetailValues { get; set; } = new List<AssetDetailValueEntityIntId>();
        public List<TreatmentOptionDetailEntity> TreatmentOptionDetails { get; set; } = new List<TreatmentOptionDetailEntity>();
        public List<TreatmentRejectionDetailEntity> TreatmentRejectionDetails { get; set; } = new List<TreatmentRejectionDetailEntity>();
        public List<TreatmentSchedulingCollisionDetailEntity> TreatmentSchedulingCollisionDetails { get; set; } = new List<TreatmentSchedulingCollisionDetailEntity>();
        public List<TreatmentConsiderationDetailEntity> TreatmentConsiderationDetails { get; set; } = new List<TreatmentConsiderationDetailEntity>();
        public List<BudgetUsageDetailEntity> BudgetUsageDetails { get; set; } = new List<BudgetUsageDetailEntity>();
        public List<CashFlowConsiderationDetailEntity> CashFlowConsiderationDetails { get; set; } = new List<CashFlowConsiderationDetailEntity>();
    }
}
