using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace BridgeCareCore.Models.DefaultData
{
    public class AnalysisDefaultData
    {
        public string Weighting { get; set; }

        public OptimizationStrategy OptimizationStrategy { get; set; }

        public string BenefitAttribute { get; set; }

        public int BenefitLimit { get; set; }

        public SpendingStrategy SpendingStrategy { get; set; }
    }
}
