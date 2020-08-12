using System;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class TreatmentOptionDetail : ITreatmentStatistics
    {
        public TreatmentOptionDetail(string treatmentName, double costPerUnitArea, double benefit, double? remainingLife)
        {
            if (string.IsNullOrWhiteSpace(treatmentName))
            {
                throw new ArgumentException("Treatment name is blank.", nameof(treatmentName));
            }

            TreatmentName = treatmentName;
            CostPerUnitArea = costPerUnitArea;
            Benefit = benefit;
            RemainingLife = remainingLife;
        }

        public double Benefit { get; }

        public double CostPerUnitArea { get; }

        public double? RemainingLife { get; }

        public string TreatmentName { get; }

        internal TreatmentOptionDetail(TreatmentOptionDetail original)
        {
            TreatmentName = original.TreatmentName;
            CostPerUnitArea = original.CostPerUnitArea;
            Benefit = original.Benefit;
            RemainingLife = original.RemainingLife;
        }
    }
}
