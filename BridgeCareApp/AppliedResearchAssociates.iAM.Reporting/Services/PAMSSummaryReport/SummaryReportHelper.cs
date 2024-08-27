using System.Collections;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport
{
    public class SummaryReportHelper
    {
        public T checkAndGetValue<T>(IDictionary itemsArray, string itemName)
        {
            var itemValue = default(T);

            if (itemsArray == null) { return itemValue; }
            if (string.IsNullOrEmpty(itemName) || string.IsNullOrWhiteSpace(itemName)) { return itemValue; }

            if (itemsArray.Contains(itemName)) { itemValue = (T)itemsArray[itemName]; }

            //return value
            return itemValue;
        }

        public static TreatmentCategory GetCategory(TreatmentCategory treatmentCategory) => treatmentCategory == TreatmentCategory.Replacement ?
                                                                                             TreatmentCategory.Reconstruction :
                                                                                             treatmentCategory;
    }
}
