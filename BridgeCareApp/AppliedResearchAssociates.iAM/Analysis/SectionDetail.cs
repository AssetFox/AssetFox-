using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class SectionDetail
    {
        public SectionDetail(string sectionName, string facilityName)
        {
            SectionName = sectionName ?? throw new ArgumentNullException(nameof(sectionName));
            FacilityName = facilityName ?? throw new ArgumentNullException(nameof(facilityName));
        }

        public string FacilityName { get; }

        public string NameOfUnfundedScheduledTreatment { get; set; }

        public string SectionName { get; }

        public List<TreatmentConsiderationDetail> TreatmentConsiderations { get; } = new List<TreatmentConsiderationDetail>();

        public string TreatmentName { get; set; }

        public List<TreatmentOptionDetail> TreatmentOptions { get; } = new List<TreatmentOptionDetail>();

        public List<TreatmentSchedulingCollisionDetail> TreatmentSchedulingCollisions { get; } = new List<TreatmentSchedulingCollisionDetail>();

        public TreatmentSource TreatmentSource { get; set; }

        public TreatmentStatus TreatmentStatus { get; set; }

        public Dictionary<string, double> ValuePerNumberAttribute { get; } = new Dictionary<string, double>();

        public Dictionary<string, string> ValuePerTextAttribute { get; } = new Dictionary<string, string>();
    }
}
