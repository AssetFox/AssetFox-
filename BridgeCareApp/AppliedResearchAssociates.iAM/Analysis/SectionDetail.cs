using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Domains;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class SectionDetail : SectionSummaryDetail
    {
        public SectionDetail(Section section) : base(section)
        {
        }

        [JsonConstructor]
        public SectionDetail(double area, string facilityName, string sectionName) : base(area, facilityName, sectionName)
        {
        }

        public TreatmentCause TreatmentCause { get; set; }

        public List<TreatmentConsiderationDetail> TreatmentConsiderations { get; } = new List<TreatmentConsiderationDetail>();

        public bool TreatmentFundingIgnoresSpendingLimit { get; set; }

        public string TreatmentName { get; set; }

        public List<TreatmentOptionDetail> TreatmentOptions { get; } = new List<TreatmentOptionDetail>();

        public List<TreatmentRejectionDetail> TreatmentRejections { get; } = new List<TreatmentRejectionDetail>();

        public List<TreatmentSchedulingCollisionDetail> TreatmentSchedulingCollisions { get; } = new List<TreatmentSchedulingCollisionDetail>();

        public TreatmentStatus TreatmentStatus { get; set; }

        internal SectionDetail(SectionDetail original) : base(original)
        {
            TreatmentName = original.TreatmentName;
            TreatmentCause = original.TreatmentCause;
            TreatmentStatus = original.TreatmentStatus;
            TreatmentFundingIgnoresSpendingLimit = original.TreatmentFundingIgnoresSpendingLimit;

            TreatmentRejections.AddRange(original.TreatmentRejections.Select(_ => new TreatmentRejectionDetail(_)));
            TreatmentOptions.AddRange(original.TreatmentOptions.Select(_ => new TreatmentOptionDetail(_)));
            TreatmentConsiderations.AddRange(original.TreatmentConsiderations.Select(_ => new TreatmentConsiderationDetail(_)));
            TreatmentSchedulingCollisions.AddRange(original.TreatmentSchedulingCollisions.Select(_ => new TreatmentSchedulingCollisionDetail(_)));
        }
    }
}
