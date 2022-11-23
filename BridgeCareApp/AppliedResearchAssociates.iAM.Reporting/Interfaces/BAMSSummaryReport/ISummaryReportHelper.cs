using System.Collections;
using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport
{
    public interface ISummaryReportHelper
    {
        T checkAndGetValue<T>(IDictionary itemsArray, string itemName);

        bool BridgeFundingBOF(AssetDetail section);
        bool BridgeFundingNHPP(AssetDetail section);
        bool BridgeFundingSTP(AssetDetail section);
        bool BridgeFundingBRIP(AssetDetail section);
        bool BridgeFundingState(AssetDetail section);
        bool BridgeFundingNotApplicable(AssetDetail section);

        bool BridgeFundingBOF(AssetSummaryDetail section);
        bool BridgeFundingNHPP(AssetSummaryDetail section);
        bool BridgeFundingSTP(AssetSummaryDetail section);
        bool BridgeFundingBRIP(AssetSummaryDetail section);
        bool BridgeFundingState(AssetSummaryDetail section);
        bool BridgeFundingNotApplicable(AssetSummaryDetail section);

        string FullFunctionalClassDescription(string functionalClassAbbreviation);
    }
}
