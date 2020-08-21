using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class SectionDetail : ISection
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

        public double Area { get; set; }

        public string FacilityName { get; }

        public string SectionName { get; }

        public TreatmentCause TreatmentCause { get; set; }

        public List<TreatmentConsiderationDetail> TreatmentConsiderations { get; } = new List<TreatmentConsiderationDetail>();

        public bool TreatmentFundingIgnoresSpendingLimit { get; set; }

        public string TreatmentName { get; set; }

        public List<TreatmentOptionDetail> TreatmentOptions { get; } = new List<TreatmentOptionDetail>();

        public List<TreatmentSchedulingCollisionDetail> TreatmentSchedulingCollisions { get; } = new List<TreatmentSchedulingCollisionDetail>();

        public TreatmentStatus TreatmentStatus { get; set; }

        public Dictionary<string, double> ValuePerNumericAttribute { get; } = new Dictionary<string, double>();

        public Dictionary<string, string> ValuePerTextAttribute { get; } = new Dictionary<string, string>();

        double ISection.GetAttributeValue(string attributeName) => ValuePerNumericAttribute[attributeName];

        internal SectionDetail(SectionDetail original)
        {
            FacilityName = original.FacilityName;
            SectionName = original.SectionName;
            Area = original.Area;

            TreatmentName = original.TreatmentName;
            TreatmentCause = original.TreatmentCause;
            TreatmentStatus = original.TreatmentStatus;
            TreatmentFundingIgnoresSpendingLimit = original.TreatmentFundingIgnoresSpendingLimit;

            ValuePerNumericAttribute.CopyFrom(original.ValuePerNumericAttribute);
            ValuePerTextAttribute.CopyFrom(original.ValuePerTextAttribute);

            TreatmentOptions.AddRange(original.TreatmentOptions.Select(_ => new TreatmentOptionDetail(_)));
            TreatmentConsiderations.AddRange(original.TreatmentConsiderations.Select(_ => new TreatmentConsiderationDetail(_)));
            TreatmentSchedulingCollisions.AddRange(original.TreatmentSchedulingCollisions.Select(_ => new TreatmentSchedulingCollisionDetail(_)));
        }
    }
}
