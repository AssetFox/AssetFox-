using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

internal sealed class TreatmentBundle : Treatment
{
    private readonly IReadOnlyList<ITreatmentScheduling> TreatmentSchedulings;

    public TreatmentBundle(IEnumerable<Treatment> bundledTreatments)
    {
        BundledTreatments = bundledTreatments?.OrderBy(t => t.Name).ToList()
            ?? throw new ArgumentNullException(nameof(bundledTreatments));

        // These initializations assume that each bundled treatment doesn't change its relevant
        // properties afterward. This is a safe assumption, because this type is only used inside
        // the analysis engine.

        var joinedNames = string.Join('|', BundledTreatments.Select(t => t.Name));
        Name = $"Bundle[{joinedNames}]";

        PerformanceCurveAdjustmentFactors = BundledTreatments
            .SelectMany(t => t.PerformanceCurveAdjustmentFactors)
            .GroupBy(kv => kv.Key)
            .ToDictionary(g => g.Key, g => g.Max(kv => kv.Value));

        ShadowForAnyTreatment = BundledTreatments.Max(t => t.ShadowForAnyTreatment);

        TreatmentSchedulings = BundledTreatments
            .SelectMany(t => t.GetSchedulings())
            .GroupBy(s => s.OffsetToFutureYear)
            .Select(BundleScheduling.Create)
            .ToList();
    }

    public IReadOnlyList<Treatment> BundledTreatments { get; }

    public override IReadOnlyDictionary<NumberAttribute, double> PerformanceCurveAdjustmentFactors { get; }

    public override int ShadowForAnyTreatment { get; }

    public override int ShadowForSameTreatment => throw new NotSupportedException("A treatment bundle does not have just one same-treatment shadow.");

    internal override bool CanUseBudget(Budget budget) => BundledTreatments.All(t => t.CanUseBudget(budget));

    internal override IReadOnlyCollection<ConsequenceApplicator> GetConsequenceApplicators(AssetContext scope)
    {
        var bestApplicators = new List<ConsequenceApplicator>();

        var applicatorsPerTarget = BundledTreatments
            .SelectMany(t => t.GetConsequenceApplicators(scope))
            .GroupBy(ca => ca.Target);

        foreach (var g in applicatorsPerTarget)
        {
            var applicators = g.ToArray();
            if (applicators.Length == 1)
            {
                bestApplicators.Add(applicators[0]);
            }
            else if (applicators.Length > 1)
            {
                if (g.Key is NumberAttribute target)
                {
                    scope.SimulationRunner.Send(new()
                    {
                        Message = $"Multiple treatments in a bundle are applying consequences to a single number attribute ({g.Key.Name}).",
                        SimulationId = scope.SimulationRunner.Simulation.Id,
                        Status = DTOs.Enums.SimulationLogStatus.Warning,
                        Subject = DTOs.Enums.SimulationLogSubject.Calculation,
                    });

                    var bestApplicator = target.IsDecreasingWithDeterioration
                        ? applicators.MaxBy(a => a.NewValue)
                        : applicators.MinBy(a => a.NewValue);

                    bestApplicators.Add(bestApplicator);
                }
                else
                {
                    scope.SimulationRunner.Send(new()
                    {
                        Message = $"Multiple treatments in a bundle are applying consequences to a single non-number attribute ({g.Key.Name}).",
                        SimulationId = scope.SimulationRunner.Simulation.Id,
                        Status = DTOs.Enums.SimulationLogStatus.Fatal,
                        Subject = DTOs.Enums.SimulationLogSubject.Calculation,
                    });
                }
            }
        }

        return bestApplicators;
    }

    internal override double GetCost(AssetContext scope, bool shouldApplyMultipleFeasibleCosts)
        => BundledTreatments.Sum(t => t.GetCost(scope, shouldApplyMultipleFeasibleCosts));

    internal override IEnumerable<ITreatmentScheduling> GetSchedulings() => TreatmentSchedulings;

    private sealed record BundleScheduling(int OffsetToFutureYear, Treatment TreatmentToSchedule) : ITreatmentScheduling
    {
        public static BundleScheduling Create(IGrouping<int, ITreatmentScheduling> g)
            => new(g.Key, new TreatmentBundle(g.Select(s => s.TreatmentToSchedule)));
    }
}
