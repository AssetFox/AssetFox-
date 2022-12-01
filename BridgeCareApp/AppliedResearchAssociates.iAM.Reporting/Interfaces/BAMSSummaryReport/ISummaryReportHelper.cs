using System.Collections;
using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport
{
    public interface ISummaryReportHelper
    {
        T checkAndGetValue<T>(IDictionary itemsArray, string itemName);

        bool BridgeFunding183(AssetDetail section);
        bool BridgeFunding185(AssetDetail section);
        bool BridgeFunding581(AssetDetail section);
        bool BridgeFundingBOF(AssetDetail section);
        bool BridgeFundingNHPP(AssetDetail section);
        bool BridgeFundingSTP(AssetDetail section);


        bool BridgeFundingBRIP(AssetDetail section);
        bool BridgeFundingState(AssetDetail section);
        bool BridgeFundingNA(AssetDetail section);


        bool BridgeFunding183(AssetSummaryDetail section);
        bool BridgeFunding185(AssetSummaryDetail section);
        bool BridgeFunding581(AssetSummaryDetail section);
        bool BridgeFundingBOF(AssetSummaryDetail section);
        bool BridgeFundingNHPP(AssetSummaryDetail section);
        bool BridgeFundingSTP(AssetSummaryDetail section);


        bool BridgeFundingBRIP(AssetSummaryDetail section);
        bool BridgeFundingState(AssetSummaryDetail section);
        bool BridgeFundingNA(AssetSummaryDetail section);

        string FullFunctionalClassDescription(string functionalClassAbbreviation);
    }
}
