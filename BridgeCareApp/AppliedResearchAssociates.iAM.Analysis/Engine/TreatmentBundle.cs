using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

internal sealed class TreatmentBundle : Treatment
{
    public TreatmentBundle(IEnumerable<Treatment> bundledTreatments)
    {
        BundledTreatments = bundledTreatments?.ToList() ?? throw new ArgumentNullException(nameof(bundledTreatments));

        // These initializations assume that each bundled treatment doesn't change its relevant
        // properties afterward. This is a safe assumption, because this type is only used inside
        // the analysis engine.

        PerformanceCurveAdjustmentFactors = bundledTreatments
            .SelectMany(t => t.PerformanceCurveAdjustmentFactors)
            .GroupBy(kv => kv.Key)
            .ToDictionary(g => g.Key, g => g.Max(kv => kv.Value));

        ShadowForAnyTreatment = bundledTreatments.Max(t => t.ShadowForAnyTreatment);
    }

    public IReadOnlyList<Treatment> BundledTreatments { get; }

    public override IReadOnlyDictionary<NumberAttribute, double> PerformanceCurveAdjustmentFactors { get; }

    public override int ShadowForAnyTreatment { get; }

    public override int ShadowForSameTreatment => throw new NotSupportedException("A treatment bundle does not have just one same-treatment shadow.");

    internal override bool CanUseBudget(Budget budget) => throw new NotImplementedException();

    internal override IReadOnlyCollection<Action> GetConsequenceActions(AssetContext scope) => throw new NotImplementedException();

    internal override double GetCost(AssetContext scope, bool shouldApplyMultipleFeasibleCosts) => throw new NotImplementedException();

    internal override IEnumerable<TreatmentScheduling> GetSchedulings() => throw new NotImplementedException();
}
