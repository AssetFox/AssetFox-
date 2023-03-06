using System;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    public sealed class TreatmentOptionDetail
    {
        public TreatmentOptionDetail(string treatmentName, double cost, double cumulativeBenefit, double? remainingLife, double instantaneousBenefit)
        {
            if (string.IsNullOrWhiteSpace(treatmentName))
            {
                throw new ArgumentException("Treatment name is blank.", nameof(treatmentName));
            }

            TreatmentName = treatmentName;
            Cost = cost;
            CumulativeBenefit = cumulativeBenefit;
            RemainingLife = remainingLife;
            InstantaneousBenefit = instantaneousBenefit;
        }

        public double CumulativeBenefit { get; }

        public double Cost { get; }

        public double InstantaneousBenefit { get; }

        public double? RemainingLife { get; }

        public string TreatmentName { get; }

        internal TreatmentOptionDetail(TreatmentOptionDetail original)
        {
            TreatmentName = original.TreatmentName;
            Cost = original.Cost;
            CumulativeBenefit = original.CumulativeBenefit;
            RemainingLife = original.RemainingLife;
            InstantaneousBenefit = original.InstantaneousBenefit;
        }
    }
}
