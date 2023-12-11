using System;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

internal sealed class TreatmentOption
{
    public TreatmentOption(AssetContext context, Treatment candidateTreatment, double cost, double benefit, double? remainingLife, double conditionChange)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        CandidateTreatment = candidateTreatment ?? throw new ArgumentNullException(nameof(candidateTreatment));
        Cost = cost;
        Benefit = benefit;
        RemainingLife = remainingLife;
        ConditionChange = conditionChange;
    }

    public double Benefit { get; }

    public Treatment CandidateTreatment { get; }

    public double ConditionChange { get; }

    public AssetContext Context { get; }

    public double Cost { get; }

    public TreatmentOptionDetail Detail => new TreatmentOptionDetail(CandidateTreatment.Name, Cost, Benefit, RemainingLife, ConditionChange);

    public double? RemainingLife { get; }
}
