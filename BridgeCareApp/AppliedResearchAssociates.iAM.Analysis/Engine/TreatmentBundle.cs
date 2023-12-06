using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

internal sealed class TreatmentBundle : Treatment
{
    private readonly IReadOnlyList<ITreatmentScheduling> TreatmentSchedulings;

    public TreatmentBundle(IEnumerable<SelectableTreatment> bundledTreatments)
    {
        BundledTreatments = bundledTreatments?.ToList() ?? throw new ArgumentNullException(nameof(bundledTreatments));

        // These initializations assume that each bundled treatment doesn't change its relevant
        // properties afterward. This is a safe assumption, because this type is only used inside
        // the analysis engine.

        Name = "<bundle>"; // TODO

        PerformanceCurveAdjustmentFactors = bundledTreatments
            .SelectMany(t => t.PerformanceCurveAdjustmentFactors)
            .GroupBy(kv => kv.Key)
            .ToDictionary(g => g.Key, g => g.Max(kv => kv.Value));

        ShadowForAnyTreatment = bundledTreatments.Max(t => t.ShadowForAnyTreatment);

        TreatmentSchedulings = BundledTreatments
            .SelectMany(t => t.Schedulings)
            .GroupBy(s => s.OffsetToFutureYear)
            .Select(BundleScheduling.Create)
            .ToList();
    }

    public IReadOnlyList<SelectableTreatment> BundledTreatments { get; }

    public override IReadOnlyDictionary<NumberAttribute, double> PerformanceCurveAdjustmentFactors { get; }

    public override int ShadowForAnyTreatment { get; }

    public override int ShadowForSameTreatment => throw new NotSupportedException("A treatment bundle does not have just one same-treatment shadow.");

    internal override bool CanUseBudget(Budget budget) => throw new NotImplementedException();

    internal override IReadOnlyCollection<Action> GetConsequenceActions(AssetContext scope) => throw new NotImplementedException();

    internal override double GetCost(AssetContext scope, bool shouldApplyMultipleFeasibleCosts) => throw new NotImplementedException();

    internal override IEnumerable<ITreatmentScheduling> GetSchedulings() => TreatmentSchedulings;

    private sealed record BundleScheduling(int OffsetToFutureYear, Treatment TreatmentToSchedule) : ITreatmentScheduling
    {
        public static BundleScheduling Create(IGrouping<int, TreatmentScheduling> g)
            => new(g.Key, new TreatmentBundle(g.Select(s => s.TreatmentToSchedule)));
    }
}
