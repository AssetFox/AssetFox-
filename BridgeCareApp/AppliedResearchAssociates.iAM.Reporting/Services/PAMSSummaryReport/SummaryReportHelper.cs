using System.Collections;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using System.Linq;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport;

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

        public void BuildKeyCashFlowFundingDetails(SimulationYearDetail yearData, AssetDetail section, string crs, Dictionary<string, List<TreatmentConsiderationDetail>> keyCashFlowFundingDetails)
        {
            if (section.TreatmentStatus != TreatmentStatus.Applied)
            {                
                var fundingSection = section.TreatmentCause == TreatmentCause.SelectedTreatment
                                   && section.AppliedTreatment.ToLower() != BAMSConstants.NoTreatment
                                   ? section : null;
                if (fundingSection != null)
                {
                    if (!keyCashFlowFundingDetails.ContainsKey(crs))
                    {
                        keyCashFlowFundingDetails.Add(crs, fundingSection.TreatmentConsiderations ?? new());
                    }
                    else
                    {
                        keyCashFlowFundingDetails[crs].AddRange(fundingSection.TreatmentConsiderations);
                    }
                }
            }
        }
    }
}
