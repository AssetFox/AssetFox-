using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSAuditReport
{
    public class BridgeDataModel
    {
        public double BRKey { get; set; }

        public SimulationYearDetail SimulationYearDetail { get; set; }

        public AssetDetail Section { get; set; }
    }
}
