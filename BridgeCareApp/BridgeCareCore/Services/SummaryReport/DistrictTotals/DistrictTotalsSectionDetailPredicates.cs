using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;

namespace BridgeCareCore.Services.SummaryReport.DistrictTotals
{
    public static class DistrictTotalsSectionDetailPredicates
    {
        private static bool IsTurnpike(SectionDetail section)
        {
            var ownerCode = section.ValuePerTextAttribute["OWNER_CODE"];
            bool r = ownerCode.Trim() == "01";
            return r;
        }

        public static bool IsCommittedProject(SectionDetail section)
        {
            bool r = section.TreatmentCause == TreatmentCause.CommittedProject;
            return r;
        }

        public static bool IsDistrictNotTurnpike(SectionDetail section, int districtNumber)
        {
            var actualDistrict = section.ValuePerTextAttribute["DISTRICT"];
            bool isTurnpike = IsTurnpike(section);
            bool r = !isTurnpike && int.TryParse(actualDistrict, out var sectionDistrict) && sectionDistrict == districtNumber;
            return r;
        }

        public static bool IsNumberedDistrictTopTable(SectionDetail section, int districtNumber)
        {
            var committed = IsCommittedProject(section);
            var district = IsDistrictNotTurnpike(section, districtNumber);
            return committed && district;
        }

        public static bool IsCommittedTurnpike(SectionDetail section)
        {
            bool committed = IsCommittedProject(section);
            bool turnpike = IsTurnpike(section);
            return committed && turnpike;
        }
    }
}
