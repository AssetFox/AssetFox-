using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract
{
    public class BaseCashFlowDistributionRuleEntity : BaseEntity
    {
        public Guid Id { get; set; }

        public int DurationInYears { get; set; }

        public decimal CostCeiling { get; set; }

        public string YearlyPercentages { get; set; }
    }
}
