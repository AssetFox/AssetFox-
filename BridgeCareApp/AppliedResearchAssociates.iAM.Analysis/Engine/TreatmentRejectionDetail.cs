using System;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    /// <summary>
    /// .
    /// </summary>
    public sealed class TreatmentRejectionDetail
    {
        public TreatmentRejectionDetail(string treatmentName, TreatmentRejectionReason treatmentRejectionReason, double potentialConditionChange)
        {
            TreatmentName = treatmentName ?? throw new ArgumentNullException(nameof(treatmentName));
            TreatmentRejectionReason = treatmentRejectionReason;
            PotentialConditionChange = potentialConditionChange;
        }

        /// <summary>
        /// .
        /// </summary>
        public double PotentialConditionChange { get; }

        /// <summary>
        /// .
        /// </summary>
        public string TreatmentName { get; }

        /// <summary>
        /// .
        /// </summary>
        public TreatmentRejectionReason TreatmentRejectionReason { get; }

        internal TreatmentRejectionDetail(TreatmentRejectionDetail original)
        {
            TreatmentName = original.TreatmentName;
            TreatmentRejectionReason = original.TreatmentRejectionReason;
            PotentialConditionChange = original.PotentialConditionChange;
        }
    }
}
