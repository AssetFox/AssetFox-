using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace AppliedResearchAssociates.iAM.Reporting.Interfaces.PAMSSummaryReport
{
    public interface ISummaryReportHelper
    {
        bool BridgeFunding183(AssetDetail section);
        bool BridgeFunding185(AssetDetail section);
        bool BridgeFunding581(AssetDetail section);
        bool BridgeFundingBOF(AssetDetail section);
        bool BridgeFundingNHPP(AssetDetail section);
        bool BridgeFundingSTP(AssetDetail section);

        bool BridgeFunding183(AssetSummaryDetail section);
        bool BridgeFunding185(AssetSummaryDetail section);
        bool BridgeFunding581(AssetSummaryDetail section);
        bool BridgeFundingBOF(AssetSummaryDetail section);
        bool BridgeFundingNHPP(AssetSummaryDetail section);
        bool BridgeFundingSTP(AssetSummaryDetail section);

        string FullFunctionalClassDescription(string functionalClassAbbreviation);
    }
}
