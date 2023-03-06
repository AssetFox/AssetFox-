using System;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    public sealed class TreatmentRejectionDetail
    {
        public TreatmentRejectionDetail(string treatmentName, TreatmentRejectionReason treatmentRejectionReason, double potentialBenefitImprovement)
        {
            TreatmentName = treatmentName ?? throw new ArgumentNullException(nameof(treatmentName));
            TreatmentRejectionReason = treatmentRejectionReason;
            PotentialInstantaneousBenefit = potentialBenefitImprovement;
        }

        public double PotentialInstantaneousBenefit { get; }

        public string TreatmentName { get; }

        public TreatmentRejectionReason TreatmentRejectionReason { get; }

        internal TreatmentRejectionDetail(TreatmentRejectionDetail original)
        {
            TreatmentName = original.TreatmentName;
            TreatmentRejectionReason = original.TreatmentRejectionReason;
            PotentialInstantaneousBenefit = original.PotentialInstantaneousBenefit;
        }
    }
}
