using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class InvestmentPlanDTO : BaseDTO
    {
        public int FirstYearOfAnalysisPeriod { get; set; }
        public double InflationRatePercentage { get; set; }
        public decimal MinimumProjectCostLimit { get; set; }
        public int NumberOfYearsInAnalysisPeriod { get; set; }
    }
}
