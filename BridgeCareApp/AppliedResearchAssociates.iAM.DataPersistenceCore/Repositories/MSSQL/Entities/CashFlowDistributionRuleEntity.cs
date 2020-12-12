using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CashFlowDistributionRuleEntity : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid CashFlowRuleId { get; set; }

        public int DurationInYears { get; set; }

        public decimal CostCeiling { get; set; }

        public string YearlyPercentages { get; set; }

        public virtual CashFlowRuleEntity CashFlowRule { get; set; }
    }
}
