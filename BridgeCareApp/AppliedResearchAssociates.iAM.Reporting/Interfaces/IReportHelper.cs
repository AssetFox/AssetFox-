using System.Collections;
using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace AppliedResearchAssociates.iAM.Reporting.Interfaces
{
    public interface IReportHelper
    {
        T CheckAndGetValue<T>(IDictionary itemsArray, string itemName);

        string FullFunctionalClassDescription(string functionalClassAbbreviation);

        bool BridgeFundingBOF(AssetDetail section);

        bool BridgeFundingNHPP(AssetDetail section);

        bool BridgeFundingSTP(AssetDetail section);

        bool BridgeFundingBRIP(AssetDetail section);

        bool BridgeFundingState(AssetDetail section);

        bool BridgeFundingNotApplicable(AssetDetail section);
    }
}
