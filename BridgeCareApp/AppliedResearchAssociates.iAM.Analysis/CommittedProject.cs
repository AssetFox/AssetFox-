using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis;

public sealed class CommittedProject : Treatment
{
    public CommittedProject(AnalysisMaintainableAsset asset, int year)
    {
        Asset = asset ?? throw new ArgumentNullException(nameof(asset));
        Year = year;
    }

    public AnalysisMaintainableAsset Asset { get; }

    public Budget Budget { get; set; }

    public double Cost { get; set; }

    public ProjectSourceDTO ProjectSource { get; set; }

    /// <remarks>
    ///     This property isn't used by the analysis engine. It probably shouldn't exist among the
    ///     types in this module.
    /// </remarks>
    public DateTime LastModifiedDate { get; set; }

    public override IReadOnlyDictionary<NumberAttribute, double> PerformanceCurveAdjustmentFactors => TemplateTreatment.PerformanceCurveAdjustmentFactors;

    public override int ShadowForAnyTreatment => TemplateTreatment.ShadowForAnyTreatment;

    public override int ShadowForSameTreatment => TemplateTreatment.ShadowForSameTreatment;

    public SelectableTreatment TemplateTreatment { get; set; }

    /// <remarks>
    ///     This property isn't used by the analysis engine. It probably shouldn't exist among the
    ///     types in this module.
    /// </remarks>
    public TreatmentCategory treatmentCategory { get; set; }

    public int Year { get; }

    public override ValidationResultBag GetDirectValidationResults()
    {
        var results = base.GetDirectValidationResults();

        if (Budget == null)
        {
            results.Add(ValidationStatus.Error, "Budget is unset.", this, nameof(Budget));
        }

        if (Cost < 0)
        {
            results.Add(ValidationStatus.Error, "Cost is less than zero.", this, nameof(Cost));
        }

        return results;
    }

    internal override bool CanUseBudget(Budget budget) => budget == Budget;

    internal override IReadOnlyCollection<ConsequenceApplicator> GetConsequenceApplicators(AssetContext scope)
        => ShouldApplyConsequences ? TemplateTreatment.GetConsequenceApplicators(scope) : Array.Empty<ConsequenceApplicator>();

    internal override double GetCost(AssetContext scope, bool shouldApplyMultipleFeasibleCosts) => Cost;

    internal override IEnumerable<TreatmentScheduling> GetSchedulings() => Enumerable.Empty<TreatmentScheduling>();

    internal bool ShouldApplyConsequences { get; set; } = true;
}
