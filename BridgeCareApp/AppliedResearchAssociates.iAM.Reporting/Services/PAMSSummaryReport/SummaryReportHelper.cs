using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis.Engine;

using AppliedResearchAssociates.iAM.Reporting.Interfaces.PAMSSummaryReport;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport
{
    public class SummaryReportHelper : ISummaryReportHelper
    {
        public T checkAndGetValue<T>(object itemsArray, string itemName)
        {
            var itemValue = default(T);
            if (itemsArray == null) { return itemValue; }
            if (string.IsNullOrEmpty(itemName) || string.IsNullOrWhiteSpace(itemName)) { return itemValue; }

            var dynamicObject = itemsArray as dynamic;
            if (dynamicObject.ContainsKey(itemName)) { itemValue = dynamicObject[itemName]; }

            //return value
            return itemValue;
        }
    }
}
