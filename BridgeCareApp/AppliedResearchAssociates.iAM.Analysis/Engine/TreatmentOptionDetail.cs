using System;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

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

    public double Benefit { get; }

    public double ConditionChange { get; }

    public double Cost { get; }

    public double? RemainingLife { get; }

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
