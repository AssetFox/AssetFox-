using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class SectionDetail
    {
        public SectionDetail(Section section)
        {
            if (section is null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            FacilityName = section.Facility.Name;
            SectionName = section.Name;
        }

        public string FacilityName { get; }

        public string NameOfUnfundedScheduledTreatment { get; set; }

        public string SectionName { get; }

        public List<TreatmentConsiderationDetail> TreatmentConsiderations { get; } = new List<TreatmentConsiderationDetail>();

        public string TreatmentName { get; set; }

        public List<TreatmentOptionDetail> TreatmentOptions { get; } = new List<TreatmentOptionDetail>();

        public List<TreatmentSchedulingCollisionDetail> TreatmentSchedulingCollisions { get; } = new List<TreatmentSchedulingCollisionDetail>();

        public TreatmentCause TreatmentCause { get; set; }

        public TreatmentStatus TreatmentStatus { get; set; }

        public Dictionary<string, double> ValuePerNumberAttribute { get; } = new Dictionary<string, double>();

        public Dictionary<string, string> ValuePerTextAttribute { get; } = new Dictionary<string, string>();
    }
}
