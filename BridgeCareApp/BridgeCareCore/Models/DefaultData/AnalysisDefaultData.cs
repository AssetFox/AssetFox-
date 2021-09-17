using AppliedResearchAssociates.iAM.DataAccess;

namespace BridgeCareCore.Models.DefaultData
{
    public class AnalysisDefaultData
    {
        public string weighting { get; set; }

        public OptimizationStrategy optimizationStrategy { get; set; }

        public string benefitAttribute { get; set; }

        public int benefitLimit { get; set; }
    }
}
