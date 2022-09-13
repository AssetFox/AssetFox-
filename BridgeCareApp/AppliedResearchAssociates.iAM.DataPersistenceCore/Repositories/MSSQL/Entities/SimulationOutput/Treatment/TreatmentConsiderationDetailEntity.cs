using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class TreatmentConsiderationDetailEntity
    {
        public Guid Id { get; set; }

        public Guid AssetDetailId { get; set; }

        public virtual AssetDetailEntity AssetDetail { get; set; }

        public int? BudgetPriorityLevel { get; set; }

        public ICollection<BudgetUsageDetailEntity> BudgetUsageDetails { get; set; } = new HashSet<BudgetUsageDetailEntity>();

        public ICollection<CashFlowConsiderationDetailEntity> CashFlowConsiderationDetails { get; set; } = new HashSet<CashFlowConsiderationDetailEntity>();

        public string TreatmentName { get; set; }
    }
}
