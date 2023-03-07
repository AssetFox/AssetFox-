using System;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    public sealed class TreatmentRejectionDetail
    {
        public TreatmentRejectionDetail(string treatmentName, TreatmentRejectionReason treatmentRejectionReason, double potentialConditionChange)
        {
            TreatmentName = treatmentName ?? throw new ArgumentNullException(nameof(treatmentName));
            TreatmentRejectionReason = treatmentRejectionReason;
            PotentialConditionChange = potentialConditionChange;
        }

        public double PotentialConditionChange { get; }

        public string TreatmentName { get; }

        public TreatmentRejectionReason TreatmentRejectionReason { get; }

        internal TreatmentRejectionDetail(TreatmentRejectionDetail original)
        {
            TreatmentName = original.TreatmentName;
            TreatmentRejectionReason = original.TreatmentRejectionReason;
            PotentialConditionChange = original.PotentialConditionChange;
        }
    }
}
