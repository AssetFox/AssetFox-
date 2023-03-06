using System;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    internal sealed class TreatmentOption
    {
        public TreatmentOption(AssetContext context, SelectableTreatment candidateTreatment, double cost, double cumulativeBenefit, double? remainingLife, double instantaneousBenefit)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            CandidateTreatment = candidateTreatment ?? throw new ArgumentNullException(nameof(candidateTreatment));
            Cost = cost;
            CumulativeBenefit = cumulativeBenefit;
            RemainingLife = remainingLife;
            InstantaneousBenefit = instantaneousBenefit;
        }

        public double CumulativeBenefit { get; }

        public SelectableTreatment CandidateTreatment { get; }

        public AssetContext Context { get; }

        public double Cost { get; }

        public TreatmentOptionDetail Detail => new TreatmentOptionDetail(CandidateTreatment.Name, Cost, CumulativeBenefit, RemainingLife, InstantaneousBenefit);

        public double InstantaneousBenefit { get; }

        public double? RemainingLife { get; }
    }
}
