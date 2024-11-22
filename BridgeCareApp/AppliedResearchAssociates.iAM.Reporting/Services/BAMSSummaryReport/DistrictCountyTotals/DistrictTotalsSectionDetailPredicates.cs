using System.Diagnostics.Eventing.Reader;
using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.DistrictCountyTotals
{
    public static class DistrictTotalsSectionDetailPredicates
    {
        public static bool IsTurnpike(AssetDetail section)
        {
            var ownerCode = section.ValuePerTextAttribute["OWNER_CODE"];
            var returnValue = ownerCode.Trim() == "31";
            return returnValue;
        }

        public static bool IsCommittedProject(AssetDetail section)
        {
            var returnValue = section.TreatmentCause == TreatmentCause.CommittedProject;
            return returnValue;
        }

        public static bool IsDistrictNotTurnpike(AssetDetail section, int districtNumber)
        {
            var actualDistrict = section.ValuePerTextAttribute["DISTRICT"];
            var isTurnpike = IsTurnpike(section);
            var returnValue = !isTurnpike && int.TryParse(actualDistrict, out var sectionDistrict) && sectionDistrict == districtNumber;
            return returnValue;
        }

        public static bool IsNumberedDistrictMpmsTable(AssetDetail section, int districtNumber)
        {
            var committed = IsCommittedProject(section);
            var district = IsDistrictNotTurnpike(section, districtNumber);
            return district && committed;
        }

        public static bool IsCounty(AssetDetail section, string county)
        {
            var actualCounty = section.ValuePerTextAttribute["COUNTY"];
            var returnValue = actualCounty.ToUpper() == county.ToUpper();
            return returnValue;
        }

        public static bool IsNumberedDistrictBamsTable(AssetDetail section, int districtNumber)
        {
            var ownerCode = section.ValuePerTextAttribute["OWNER_CODE"];
            var committed = IsCommittedProject(section);
            var district = IsDistrictNotTurnpike(section, districtNumber);
            return district && !committed;
        }

        public static bool IsCommittedTurnpike(AssetDetail section)
        {
            bool committed = IsCommittedProject(section);
            bool turnpike = IsTurnpike(section);
            return committed && turnpike;
        }


        public static bool IsTurnpikeButNotCommitted(AssetDetail section)
        {
            bool committed = IsCommittedProject(section);
            bool turnpike = IsTurnpike(section);
            return turnpike && !committed;
        }
    }
}
