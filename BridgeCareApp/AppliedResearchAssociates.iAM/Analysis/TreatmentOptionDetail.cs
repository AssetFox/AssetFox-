using System;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class TreatmentOptionDetail
    {
        public TreatmentOptionDetail(string treatmentName, double cost, double benefit, double? remainingLife)
        {
            if (string.IsNullOrWhiteSpace(treatmentName))
            {
                throw new ArgumentException("Treatment name is blank.", nameof(treatmentName));
            }

            TreatmentName = treatmentName;
            Cost = cost;
            Benefit = benefit;
            RemainingLife = remainingLife;
        }

        public double Benefit { get; }

        public double Cost { get; }

        public double? RemainingLife { get; }

        public string TreatmentName { get; }

        internal TreatmentOptionDetail(TreatmentOptionDetail original)
        {
            TreatmentName = original.TreatmentName;
            Cost = original.Cost;
            Benefit = original.Benefit;
            RemainingLife = original.RemainingLife;
        }
    }
}
