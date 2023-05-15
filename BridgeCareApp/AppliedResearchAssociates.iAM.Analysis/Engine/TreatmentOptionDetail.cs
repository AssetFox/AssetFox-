using System;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    /// <summary>
    /// .
    /// </summary>
    public sealed class TreatmentOptionDetail
    {
        public TreatmentOptionDetail(string treatmentName, double cost, double benefit, double? remainingLife, double conditionChange)
        {
            if (string.IsNullOrWhiteSpace(treatmentName))
            {
                throw new ArgumentException("Treatment name is blank.", nameof(treatmentName));
            }

            TreatmentName = treatmentName;
            Cost = cost;
            Benefit = benefit;
            RemainingLife = remainingLife;
            ConditionChange = conditionChange;
        }

        /// <summary>
        /// .
        /// </summary>
        public double Benefit { get; }

        /// <summary>
        /// .
        /// </summary>
        public double ConditionChange { get; }

        /// <summary>
        /// .
        /// </summary>
        public double Cost { get; }

        /// <summary>
        /// .
        /// </summary>
        public double? RemainingLife { get; }

        /// <summary>
        /// .
        /// </summary>
        public string TreatmentName { get; }

        internal TreatmentOptionDetail(TreatmentOptionDetail original)
        {
            TreatmentName = original.TreatmentName;
            Cost = original.Cost;
            Benefit = original.Benefit;
            RemainingLife = original.RemainingLife;
            ConditionChange = original.ConditionChange;
        }
    }
}
