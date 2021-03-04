using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class CashFlowDistributionRuleDTO : BaseDTO
    {
        public int DurationInYears { get; set; }

        public decimal CostCeiling { get; set; }

        public string YearlyPercentages { get; set; }
    }
}
