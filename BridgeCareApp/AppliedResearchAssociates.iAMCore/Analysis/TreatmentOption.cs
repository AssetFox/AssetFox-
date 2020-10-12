using System;

namespace AppliedResearchAssociates.iAMCore.Analysis
{
    internal sealed class TreatmentOption
    {
        public TreatmentOption(SectionContext context, SelectableTreatment candidateTreatment, double cost, double benefit, double? remainingLife)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            CandidateTreatment = candidateTreatment ?? throw new ArgumentNullException(nameof(candidateTreatment));
            Cost = cost;
            Benefit = benefit;
            RemainingLife = remainingLife;
        }

        public double Benefit { get; }

        public SelectableTreatment CandidateTreatment { get; }

        public SectionContext Context { get; }

        public double Cost { get; }

        public TreatmentOptionDetail Detail => new TreatmentOptionDetail(CandidateTreatment.Name, Cost, Benefit, RemainingLife);

        public double? RemainingLife { get; }
    }
}
