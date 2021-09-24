using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace BridgeCareCore.Interfaces.SummaryReport
{
    public interface ISummaryReportHelper
    {
        bool BridgeFunding183(SectionDetail section);
        bool BridgeFunding185(SectionDetail section);
        bool BridgeFunding581(SectionDetail section);
        bool BridgeFundingBOF(SectionDetail section);
        bool BridgeFundingNHPP(SectionDetail section);
        bool BridgeFundingSTP(SectionDetail section);

        bool BridgeFunding183(SectionSummaryDetail section);
        bool BridgeFunding185(SectionSummaryDetail section);
        bool BridgeFunding581(SectionSummaryDetail section);
        bool BridgeFundingBOF(SectionSummaryDetail section);
        bool BridgeFundingNHPP(SectionSummaryDetail section);
        bool BridgeFundingSTP(SectionSummaryDetail section);

        string FullFunctionalClassDescription(string functionalClassAbbreviation);
    }
}
