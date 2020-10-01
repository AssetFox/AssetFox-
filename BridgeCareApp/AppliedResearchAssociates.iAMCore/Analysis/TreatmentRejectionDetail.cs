using System;

namespace AppliedResearchAssociates.iAMCore.Analysis
{
    public sealed class TreatmentRejectionDetail
    {
        public TreatmentRejectionDetail(string treatmentName, TreatmentRejectionReason treatmentRejectionReason)
        {
            TreatmentName = treatmentName ?? throw new ArgumentNullException(nameof(treatmentName));
            TreatmentRejectionReason = treatmentRejectionReason;
        }

        public string TreatmentName { get; }

        public TreatmentRejectionReason TreatmentRejectionReason { get; }

        internal TreatmentRejectionDetail(TreatmentRejectionDetail original)
        {
            TreatmentName = original.TreatmentName;
            TreatmentRejectionReason = original.TreatmentRejectionReason;
        }
    }
}
