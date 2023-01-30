using System.Collections;
using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport
{
    public interface ISummaryReportHelper
    {
        bool BridgeFundingBOF(AssetSummaryDetail section);
        bool BridgeFundingNHPP(AssetSummaryDetail section);
        bool BridgeFundingSTP(AssetSummaryDetail section);
        bool BridgeFundingBRIP(AssetSummaryDetail section);
        bool BridgeFundingState(AssetSummaryDetail section);
        bool BridgeFundingNotApplicable(AssetSummaryDetail section);        
    }
}
