using System;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    /// <summary>
    /// The reason a specific treatment was rejected
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
        /// The condition change if the treatment was applied
        /// </summary>
        public double PotentialConditionChange { get; }

        /// <summary>
        /// The name of the treatment
        /// </summary>
        public string TreatmentName { get; }

        /// <summary>
        /// The reason the treatment was rejected
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
