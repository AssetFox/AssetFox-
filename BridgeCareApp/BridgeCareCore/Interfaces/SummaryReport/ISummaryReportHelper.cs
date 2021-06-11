using AppliedResearchAssociates.iAM.Analysis;

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
        string FullFunctionalClassDescription(string functionalClassAbbreviation);
    }
}
