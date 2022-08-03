using System.Collections;
using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace AppliedResearchAssociates.iAM.Reporting.Interfaces.PAMSSummaryReport
{
    public interface ISummaryReportHelper
    {
        T checkAndGetValue<T>(IDictionary itemsArray, string itemName);
    }
}
