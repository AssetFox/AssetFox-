using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.DistrictTotals
{
    public static class DistrictTotalsSectionDetailPredicates
    {
        public static bool IsTurnpike(SectionDetail section)
        {
            var ownerCode = section.ValuePerTextAttribute["OWNER_CODE"];
            bool r = ownerCode.Trim() == "31";
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

        public static bool IsNumberedDistrictMpmsTable(SectionDetail section, int districtNumber)
        {
            var committed = IsCommittedProject(section);
            var district = IsDistrictNotTurnpike(section, districtNumber);
            return district && committed;
        }


        public static bool IsNumberedDistrictBamsTable(SectionDetail section, int districtNumber)
        {
            var ownerCode = section.ValuePerTextAttribute["OWNER_CODE"];
            var committed = IsCommittedProject(section);
            var district = IsDistrictNotTurnpike(section, districtNumber);
            return district && !committed;
        }

        public static bool IsCommittedTurnpike(SectionDetail section)
        {
            bool committed = IsCommittedProject(section);
            bool turnpike = IsTurnpike(section);
            return committed && turnpike;
        }


        public static bool IsTurnpikeButNotCommitted(SectionDetail section)
        {
            bool committed = IsCommittedProject(section);
            bool turnpike = IsTurnpike(section);
            return turnpike && !committed;
        }
    }
}
