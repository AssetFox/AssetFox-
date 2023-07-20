using System;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

/// <summary>
/// Results of applying a specific treatment.  Note that is does NOT mean that the treatment was applied.
/// The applied treatment is part of the AssetDetail object.
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
    /// The benefit of applying this treatment as calculated by the difference between the do nothing option and
    /// the resulting value of the benefit attribute if the treatment was applied.
    /// </summary>
    public double Benefit { get; }

    /// <summary>
    /// The change in condition immediately after the treatment is applied.
    /// </summary>
    public double ConditionChange { get; }

    /// <summary>
    /// The cost of the treatment for the specific asset
    /// </summary>
    public double Cost { get; }

    /// <summary>
    /// The remaining lift of the asset if the treatment is applied
    /// </summary>
    public double? RemainingLife { get; }

    /// <summary>
    /// The name of the treatment whose application characteristics are given in this object.
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
