using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class InvestmentPlanDTO : BaseDTO
    {
        public int FirstYearOfAnalysisPeriod { get; set; }

        public double InflationRatePercentage { get; set; }

        public decimal MinimumProjectCostLimit { get; set; }

        public int NumberOfYearsInAnalysisPeriod { get; set; }
    }
}
