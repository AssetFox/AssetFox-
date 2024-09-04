using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport
{
    public class SummaryReportHelper
    {
        public bool BridgeFundingBOF(AssetSummaryDetail section)
        {
            var functionalClass = "";
            if (section.ValuePerTextAttribute["FUNC_CLASS"].Length >= 2)
            {
                functionalClass = section.ValuePerTextAttribute["FUNC_CLASS"].Substring(0, 2);
            }
            var NBISlen = section.ValuePerTextAttribute["NBISLEN"];

            return
                NBISlen is "Y" &&
                functionalClass is "08" or "09" or "18" or "19";
        }

        public bool BridgeFundingNHPP(AssetSummaryDetail section)
        {
            if (string.IsNullOrEmpty(section.ValuePerTextAttribute["FEDAID"])) return false;
            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var functionalClass = "";
            if (section.ValuePerTextAttribute["FUNC_CLASS"].Length >= 2)
            {
                functionalClass = section.ValuePerTextAttribute["FUNC_CLASS"].Substring(0, 2);
            }

            return
                (fedAid is "1" && functionalClass is "01" or "02" or "03" or "06" or "07" or "11" or "12" or "14" or "16" or "17") ||
                (fedAid is "0" && functionalClass is "99");
        }

        public bool BridgeFundingSTP(AssetSummaryDetail section)
        {
            if (string.IsNullOrEmpty(section.ValuePerTextAttribute["FEDAID"])) return false;
            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);

            return fedAid is "1" or "2";
        }

        public bool BridgeFundingBRIP(AssetSummaryDetail section)
        {
            var functionalClass = "";
            if (section.ValuePerTextAttribute["FUNC_CLASS"].Length >= 2)
            {
                functionalClass = section.ValuePerTextAttribute["FUNC_CLASS"].Substring(0, 2);
            }
            var NBISlen = section.ValuePerTextAttribute["NBISLEN"];

            return
                NBISlen is "Y" &&
                functionalClass is "01" or "02";
        }

        public bool BridgeFundingState(AssetSummaryDetail section)
        {
            var internetReport = section.ValuePerTextAttribute["INTERNET_REPORT"];

            return internetReport is "State" or "Local";
        }

        public bool BridgeFundingNotApplicable(AssetSummaryDetail section)
        {
            if (string.IsNullOrEmpty(section.ValuePerTextAttribute["FEDAID"])) return false;
            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var functionalClass = "";
            if (section.ValuePerTextAttribute["FUNC_CLASS"].Length >= 2)
            {
                functionalClass = section.ValuePerTextAttribute["FUNC_CLASS"].Substring(0, 2);
            }

            return
                (fedAid is "0" && functionalClass is "01" or "02" or "03" or "06" or "07" or "11" or "12" or "14" or "16" or "17") ||
                functionalClass is "NN";
        }

        public static TreatmentCategory GetCategory(TreatmentCategory treatmentCategory) => treatmentCategory == TreatmentCategory.Reconstruction ?
                                                                                             TreatmentCategory.Replacement :
                                                                                             treatmentCategory;       
    }
}
