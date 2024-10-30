using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

internal sealed class AssetContext : CalculateEvaluateScope
{
    public AssetContext(AnalysisMaintainableAsset asset, SimulationRunner simulationRunner)
    {
        Asset = asset ?? throw new ArgumentNullException(nameof(asset));
        SimulationRunner = simulationRunner ?? throw new ArgumentNullException(nameof(simulationRunner));

        ResetDetail();

        Initialize();
    }

    public AssetContext(AssetContext original) : base(original)
    {
        Asset = original.Asset;
        SimulationRunner = original.SimulationRunner;

        ResetDetail();

        EventSchedule.CopyFrom(original.EventSchedule);
        FirstUnshadowedYearForAnyTreatment = original.FirstUnshadowedYearForAnyTreatment;
        FirstUnshadowedYearForSameTreatment.CopyFrom(original.FirstUnshadowedYearForSameTreatment);
        NumberCache.CopyFrom(original.NumberCache);

        InitializeCalculatedFields();
    }

    public AnalysisMaintainableAsset Asset { get; }

    public AssetDetail Detail { get; private set; }

    public IDictionary<int, Choice<Treatment, TreatmentProgress>> EventSchedule { get; } = new Dictionary<int, Choice<Treatment, TreatmentProgress>>();

    public SimulationRunner SimulationRunner { get; }

    public AssetSummaryDetail SummaryDetail
    {
        get
        {
            var detail = new AssetSummaryDetail(Asset);
            CopyAttributeValuesToDetail(detail);
            return detail;
        }
    }

    public void ApplyPassiveTreatment(int year)
    {
        CheckPassiveTreatmentCostIsZero();
        ApplyTreatment(SimulationRunner.Simulation.DesignatedPassiveTreatment, year);
    }

    public void ApplyPerformanceCurves() => ApplyPerformanceCurves(GetPerformanceCurveCalculatorPerAttribute());

    public void ApplyTreatment(Treatment treatment, int year)
    {
        ApplyTreatmentButNotMetadata(treatment);
        ApplyTreatmentMetadata(year);
    }

    public void ApplyTreatmentConsequences(Treatment treatment)
    {
        var consequenceApplicators = treatment.GetConsequenceApplicators(this);
        foreach (var consequenceApplicator in consequenceApplicators)
        {
            consequenceApplicator.Change();
        }
    }

    public void ApplyTreatmentMetadataIfPending(int year)
    {
        if (AppliedTreatmentWithPendingMetadata is not null)
        {
            ApplyTreatmentMetadata(year);
        }
    }

    public void CopyAttributeValuesToDetail() => CopyAttributeValuesToDetail(Detail);

    public void CopyDetailFrom(AssetContext other) => Detail = new AssetDetail(other.Detail);

    public bool? Evaluate(Criterion criterion)
    {
        if (!EvaluationCache.TryGetValue(criterion.Expression, out var result))
        {
            result = criterion.Evaluate(this);
            EvaluationCache.Add(criterion.Expression, result);
        }

        return result;
    }

    public (double rawBenefit, double lruBenefit, double weight, double benefit) GetBenefitData()
    {
        var rawBenefit = GetNumber(AnalysisMethod.Benefit.Attribute.Name);

        // "Limit-Relativized Unweighted"
        var lruBenefit = AnalysisMethod.Benefit.GetValueRelativeToLimit(rawBenefit);

        var weight = AnalysisMethod.Weighting != null
            ? GetNumber(AnalysisMethod.Weighting.Name)
            : 1;

        return (rawBenefit, lruBenefit, weight, lruBenefit * weight);
    }

    public double GetCostOfTreatment(Treatment treatment) => treatment.GetCost(this, AnalysisMethod.ShouldApplyMultipleFeasibleCosts);

    public override double GetNumber(string key)
    {
        if (!GetNumber_ActiveKeysOfCurrentInvocation.TryAdd(key, GetNumber_ActiveKeysOfCurrentInvocation.Count))
        {
            var previousInvocationIndex = GetNumber_ActiveKeysOfCurrentInvocation[key];

            var invocationStack =
                GetNumber_ActiveKeysOfCurrentInvocation
                .OrderBy(kv => kv.Value)
                .Select(kv => kv.Key)
                .Append(key)
                .Select(k => $"[{k}]")
                .ToArray();

            static void emphasize(ref string key) => key = $"**{key}**";

            emphasize(ref invocationStack[previousInvocationIndex]);
            emphasize(ref invocationStack[^1]);

            var invocationText = string.Join(" to ", invocationStack);

            var messageBuilder = new SimulationMessageBuilder("Loop encountered during number calculation: " + invocationText)
            {
                AssetName = Asset.AssetName,
                AssetId = Asset.Id,
            };

            var logBuilder = SimulationLogMessageBuilders.CalculationFatal(
                messageBuilder.ToString(),
                SimulationRunner.Simulation.Id);

            SimulationRunner.Send(logBuilder);
        }

        if (!NumberCache_Override.TryGetValue(key, out var number) && !NumberCache.TryGetValue(key, out number))
        {
            number = base.GetNumber(key);

            if (SimulationRunner.NumberAttributeByName.TryGetValue(key, out var attribute))
            {
                if (attribute.Minimum.HasValue)
                {
                    number = Math.Max(number, attribute.Minimum.Value);
                }

                if (attribute.Maximum.HasValue)
                {
                    number = Math.Min(number, attribute.Maximum.Value);
                }
            }

            NumberCache[key] = number;
        }

        _ = GetNumber_ActiveKeysOfCurrentInvocation.Remove(key);

        return number;
    }

    public double GetSpatialWeight()
    {
        var returnValue = Asset.SpatialWeighting.Compute(this);
        if (double.IsNaN(returnValue) || double.IsInfinity(returnValue))
        {
            var errorMessage = SimulationLogMessages.SpatialWeightCalculationReturned(Asset, Asset.SpatialWeighting, returnValue);
            var messageBuilder = SimulationLogMessageBuilders.CalculationFatal(errorMessage, SimulationRunner.Simulation.Id);
            SimulationRunner.Send(messageBuilder);
        }
        return returnValue;
    }

    public void MarkTreatmentProgress(Treatment treatment)
    {
        AppliedTreatmentWithPendingMetadata = null;

        Detail.AppliedTreatment = treatment.Name;
        Detail.TreatmentStatus = TreatmentStatus.Progressed;
    }

    public void PrepareForTreatment()
    {
        FixCalculatedFieldValuesWithPreDeteriorationTiming();

        if (SimulationRunner.Simulation.ShouldPreapplyPassiveTreatment)
        {
            FixCalculatedFieldValuesWithoutPreDeteriorationTiming();
        }

        ApplyPerformanceCurves();

        if (SimulationRunner.Simulation.ShouldPreapplyPassiveTreatment)
        {
            PreapplyPassiveTreatment();
            UnfixCalculatedFieldValuesWithoutPreDeteriorationTiming();
        }

        FixCalculatedFieldValuesWithPostDeteriorationTiming();
    }

    public void ResetDetail() => Detail = new AssetDetail(Asset);

    public void RollForward(ConcurrentBag<RollForwardEventDetail> rollForwardEvents)
    {
        // Per email on 2020-05-06 from Gregg to Jake, Chad, and William: "We roll forward
        // attributes with performance curves. To do so we need to know which performance curve
        // to use. When evaluating which performance curve to use it is necessary to evaluate
        // the criteria. Ideally, the attribute in the criteria will have a value for the year
        // in question. If it does not, use the Most Recent Value (from the rollup). If it does
        // not have a Most Recent Value, use the default. Currently, I don't believe the Roll
        // Forward uses the No Treatment consequences. It should in the new code."

        IEnumerable<int?> getMostRecentYearPerAttribute<T>(IEnumerable<Attribute<T>> attributes) =>
            attributes.Select(attribute => Asset.GetHistory(attribute).MostRecentYear);

        var earliestYearOfMostRecentValue = Enumerable.Concat(
            getMostRecentYearPerAttribute(SimulationRunner.Simulation.Network.Explorer.NumberAttributes),
            getMostRecentYearPerAttribute(SimulationRunner.Simulation.Network.Explorer.TextAttributes)
            ).Min();

        var earliestYearOfSchedule = EventSchedule.Keys.AsNullables().Min();

        var firstYearOfRollForward = (earliestYearOfMostRecentValue.HasValue, earliestYearOfSchedule.HasValue) switch
        {
            (true, true) => Math.Min(earliestYearOfMostRecentValue.Value, earliestYearOfSchedule.Value),
            (true, false) => earliestYearOfMostRecentValue.Value,
            (false, true) => earliestYearOfSchedule.Value,
            (false, false) => (int?)null
        };

        if (firstYearOfRollForward < SimulationRunner.Simulation.InvestmentPlan.FirstYearOfAnalysisPeriod)
        {
            SetHistoricalValues(firstYearOfRollForward.Value, true, SimulationRunner.Simulation.Network.Explorer.NumberAttributes, SetNumber);
            SetHistoricalValues(firstYearOfRollForward.Value, true, SimulationRunner.Simulation.Network.Explorer.TextAttributes, SetText);

            HandleTreatmentDuringRollForward(rollForwardEvents, firstYearOfRollForward.Value);

            foreach (var year in Static.RangeFromBounds(firstYearOfRollForward.Value + 1, SimulationRunner.Simulation.InvestmentPlan.FirstYearOfAnalysisPeriod - 1))
            {
                SetHistoricalValues(year, false, SimulationRunner.Simulation.Network.Explorer.NumberAttributes, SetNumber);
                SetHistoricalValues(year, false, SimulationRunner.Simulation.Network.Explorer.TextAttributes, SetText);

                HandleTreatmentDuringRollForward(rollForwardEvents, year);
            }
        }

        SetHistoricalValues(SimulationRunner.Simulation.InvestmentPlan.FirstYearOfAnalysisPeriod, false, SimulationRunner.Simulation.Network.Explorer.NumberAttributes, SetNumber);
        SetHistoricalValues(SimulationRunner.Simulation.InvestmentPlan.FirstYearOfAnalysisPeriod, false, SimulationRunner.Simulation.Network.Explorer.TextAttributes, SetText);
    }

    public override void SetNumber(string key, double value)
    {
        ClearCache();
        base.SetNumber(key, value);
    }

    public override void SetNumber(string key, Func<double> getValue)
    {
        ClearCache();
        base.SetNumber(key, getValue);
    }

    public override void SetText(string key, string value)
    {
        ClearCache();
        base.SetText(key, value);
    }

    public void UnfixCalculatedFieldValues() => NumberCache_Override.Clear();

    public bool YearIsWithinShadowForAnyTreatment(int year) => year < FirstUnshadowedYearForAnyTreatment;

    public bool YearIsWithinShadowForSameTreatment(int year, Treatment treatment) => FirstUnshadowedYearForSameTreatment.TryGetValue(treatment.Name, out var firstUnshadowedYear) && year < firstUnshadowedYear;

    private static readonly StringComparer KeyComparer = StringComparer.OrdinalIgnoreCase;

    private readonly Dictionary<string, bool?> EvaluationCache = new();

    private readonly Dictionary<string, int> FirstUnshadowedYearForSameTreatment = new();

    private readonly Dictionary<string, int> GetNumber_ActiveKeysOfCurrentInvocation = new(StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<Attribute, double> MostRecentAdjustmentFactorsForPerformanceCurves = new();

    private readonly Dictionary<string, double> NumberCache = new(KeyComparer);

    private readonly Dictionary<string, double> NumberCache_Override = new(KeyComparer);

    private Treatment AppliedTreatmentWithPendingMetadata;

    private int? FirstUnshadowedYearForAnyTreatment;

    private IEnumerable<CalculatedField> AllCalculatedFields => SimulationRunner.Simulation.Network.Explorer.CalculatedFields;

    private AnalysisMethod AnalysisMethod => SimulationRunner.Simulation.AnalysisMethod;

    private void ApplyPerformanceCurves(IDictionary<string, Func<double>> calculatorPerAttribute)
    {
        var dataUpdates = calculatorPerAttribute.Select(kv => (kv.Key, kv.Value())).ToArray();

        foreach (var (key, value) in dataUpdates)
        {
            SetNumber(key, value);
        }
    }

    private void ApplyTreatmentButNotMetadata(Treatment treatment)
    {
        ApplyTreatmentConsequences(treatment);
        AppliedTreatmentWithPendingMetadata = treatment;
    }

    private void ApplyTreatmentMetadata(int year)
    {
        var treatment = AppliedTreatmentWithPendingMetadata;
        AppliedTreatmentWithPendingMetadata = null;

        foreach (var scheduling in treatment.GetSchedulings())
        {
            var schedulingYear = year + scheduling.OffsetToFutureYear;

            if (EventSchedule.ContainsKey(schedulingYear))
            {
                Detail.TreatmentSchedulingCollisions.Add(new TreatmentSchedulingCollisionDetail(schedulingYear, scheduling.TreatmentToSchedule.Name));
            }
            else
            {
                EventSchedule.Add(schedulingYear, scheduling.TreatmentToSchedule);
            }
        }

        if (treatment != SimulationRunner.Simulation.DesignatedPassiveTreatment)
        {
            FirstUnshadowedYearForAnyTreatment = year + treatment.ShadowForAnyTreatment;

            if (treatment is TreatmentBundle bundle)
            {
                // [REVIEW] The assumption that any treatment has just one same-treatment shadow
                // (i.e. that a Treatment object is always atomic) was thoroughly baked into the
                // data design of the analysis engine rewrite back in 2020. We haven't yet
                // accumulated budget (especially time) for refactoring with such a large splash
                // radius, so this is, for now, how we're handling same-treatment shadows for a
                // treatment that is actually multiple treatments.

                foreach (var bundledTreatment in bundle.BundledTreatments)
                {
                    FirstUnshadowedYearForSameTreatment[bundledTreatment.Name] = year + bundledTreatment.ShadowForSameTreatment;
                }
            }
            else
            {
                FirstUnshadowedYearForSameTreatment[treatment.Name] = year + treatment.ShadowForSameTreatment;
            }

            foreach (var (attribute, factor) in treatment.PerformanceCurveAdjustmentFactors)
            {
                MostRecentAdjustmentFactorsForPerformanceCurves[attribute] = factor;
            }
        }

        Detail.AppliedTreatment = treatment.Name;
        Detail.TreatmentStatus = TreatmentStatus.Applied;
    }

    private double CalculateValueOnCurve(PerformanceCurve curve, Action<double> handle)
    {
        var value = curve.Equation.Compute(this, curve, MostRecentAdjustmentFactorsForPerformanceCurves);
        handle(value);
        return value;
    }

    private void CheckPassiveTreatmentCostIsZero()
    {
        var cost = GetCostOfTreatment(SimulationRunner.Simulation.DesignatedPassiveTreatment);
        if (cost != 0)
        {
            var messageBuilder = new SimulationMessageBuilder(MessageStrings.CostOfPassiveTreatmentIsNonZero)
            {
                ItemName = SimulationRunner.Simulation.DesignatedPassiveTreatment.Name,
                ItemId = SimulationRunner.Simulation.DesignatedPassiveTreatment.Id,
                AssetName = Asset.AssetName,
                AssetId = Asset.Id,
            };

            var builder = new SimulationLogMessageBuilder
            {
                SimulationId = SimulationRunner.Simulation.Id,
                Message = messageBuilder.ToString(),
                Status = SimulationLogStatus.Fatal,
                Subject = SimulationLogSubject.Runtime,
            };

            SimulationRunner.Send(builder);
        }
    }

    private void ClearCache()
    {
        NumberCache.Clear();
        EvaluationCache.Clear();
    }

    private void CopyAttributeValuesToDetail(AssetSummaryDetail detail)
    {
        detail.ValuePerNumericAttribute.Add(Network.SpatialWeightIdentifier, GetNumber(Network.SpatialWeightIdentifier));

        foreach (var attribute in SimulationRunner.Simulation.Network.Explorer.NumericAttributes)
        {
            detail.ValuePerNumericAttribute.Add(attribute.Name, GetNumber(attribute.Name));
        }

        foreach (var attribute in SimulationRunner.Simulation.Network.Explorer.TextAttributes)
        {
            detail.ValuePerTextAttribute.Add(attribute.Name, GetText(attribute.Name));
        }
    }

    private void FixCalculatedFieldValues(IEnumerable<CalculatedField> calculatedFields)
    {
        foreach (var calculatedField in calculatedFields)
        {
            NumberCache_Override[calculatedField.Name] = GetNumber(calculatedField.Name);
        }
    }

    private void FixCalculatedFieldValuesWithoutPreDeteriorationTiming() => FixCalculatedFieldValues(AllCalculatedFields.Where(cf => cf.Timing != CalculatedFieldTiming.PreDeterioration));

    private void FixCalculatedFieldValuesWithPostDeteriorationTiming() => FixCalculatedFieldValues(AllCalculatedFields.Where(cf => cf.Timing == CalculatedFieldTiming.PostDeterioration));

    private void FixCalculatedFieldValuesWithPreDeteriorationTiming() => FixCalculatedFieldValues(AllCalculatedFields.Where(cf => cf.Timing == CalculatedFieldTiming.PreDeterioration));

    private Func<double> GetCalculator(IGrouping<NumberAttribute, PerformanceCurve> curves)
    {
        List<PerformanceCurve> applicableCurves = new();
        List<PerformanceCurve> defaultCurves = new();

        foreach (var curve in curves)
        {
            var evaluation = Evaluate(curve.Criterion);

            if (!evaluation.HasValue)
            {
                defaultCurves.Add(curve);
            }
            else if (evaluation.Value)
            {
                applicableCurves.Add(curve);
            }
        }

        var operativeCurves = applicableCurves.Count > 0 ? applicableCurves : defaultCurves;

        if (operativeCurves.Count == 0)
        {
            var messageBuilder = new SimulationMessageBuilder("No performance curves are operative for a deteriorating attribute.")
            {
                ItemName = curves.Key.Name,
                AssetName = Asset.AssetName,
                AssetId = Asset.Id,
            };

            var logBuilder = SimulationLogMessageBuilders.RuntimeFatal(messageBuilder, SimulationRunner.Simulation.Id);
            SimulationRunner.Send(logBuilder);
        }

        if (operativeCurves.Count > 1)
        {
            var messageBuilder = new SimulationMessageBuilder("Two or more performance curves are simultaneously operative for a single deteriorating attribute.")
            {
                ItemName = curves.Key.Name,
                AssetName = Asset.AssetName,
                AssetId = Asset.Id,
            };

            var logMessage = SimulationLogMessageBuilders.RuntimeWarning(messageBuilder, SimulationRunner.Simulation.Id);
            SimulationRunner.Send(logMessage);
        }

        return curves.Key.IsDecreasingWithDeterioration
            ? () => operativeCurves.Min(curve => CalculateValueOnCurve(curve, value => SendToSimulationLogIfNeeded(curve, value)))
            : () => operativeCurves.Max(curve => CalculateValueOnCurve(curve, value => SendToSimulationLogIfNeeded(curve, value)));
    }

    private IDictionary<string, Func<double>> GetPerformanceCurveCalculatorPerAttribute() => SimulationRunner.CurvesPerAttribute.ToDictionary(curves => curves.Key.Name, GetCalculator);

    private void HandleTreatmentDuringRollForward(ConcurrentBag<RollForwardEventDetail> rollForwardEvents, int year)
    {
        PrepareForTreatment();

        if (EventSchedule.TryGetValue(year, out var scheduledEvent) &&
            scheduledEvent.IsT1(out var treatment))
        {
            if (treatment is not (CommittedProject or CommittedProjectBundle))
            {
                throw new InvalidOperationException("Simulation engine scheduled invalid roll-forward event.");
            }

            ApplyTreatment(treatment, year);
            rollForwardEvents.Add(new(year, Asset.Id, Asset.AssetName, treatment.Name));
        }
        else if (!SimulationRunner.Simulation.ShouldPreapplyPassiveTreatment)
        {
            ApplyPassiveTreatment(year);
        }

        ApplyTreatmentMetadataIfPending(year);
        UnfixCalculatedFieldValues();
    }

    private void Initialize()
    {
        var initialReferenceYear = SimulationRunner.Simulation.InvestmentPlan.FirstYearOfAnalysisPeriod;

        SetInitialValues(SimulationRunner.Simulation.Network.Explorer.NumberAttributes, SetNumber);
        SetInitialValues(SimulationRunner.Simulation.Network.Explorer.TextAttributes, SetText);

        foreach (var committedProjects in SimulationRunner.CommittedProjectsPerAsset[Asset].GroupBy(cp => cp.Year))
        {
            var year = committedProjects.Key;
            var numberOfProjects = committedProjects.Count();
            if (numberOfProjects > 1)
            {
                if (committedProjects.Any(cp => !cp.ShouldApplyConsequences))
                {
                    SimulationRunner.MessageBuilder = new("Cash-flow committed project overlaps with another committed project on the same asset.")
                    {
                        AssetId = Asset.Id,
                        AssetName = Asset.AssetName,
                    };
                    var error = SimulationLogMessageBuilders.RuntimeFatal(SimulationRunner.MessageBuilder, SimulationRunner.Simulation.Id);
                    SimulationRunner.Send(error);
                }

                EventSchedule.Add(year, new CommittedProjectBundle(committedProjects));
            }
            else if (numberOfProjects == 1)
            {
                EventSchedule.Add(year, committedProjects.Single());
            }
        }

        InitializeCalculatedFields();
    }

    private void InitializeCalculatedFields()
    {
        // This initialization is separate from the rest because it needs to be called again in
        // the copy constructor. Otherwise, the copy's calculated fields will use the original's
        // scope values. (Note that "this" is captured on each invocation of SetNumber.)

        foreach (var calculatedField in SimulationRunner.Simulation.Network.Explorer.CalculatedFields)
        {
            double calculate()
            {
                try
                {
                    return calculatedField.Calculate(this);
                }
                catch (SimulationException e)
                {
                    var logBuilder = SimulationLogMessageBuilders.Exception(e, SimulationRunner.Simulation.Id);
                    SimulationRunner.Send(logBuilder, false);
                    throw;
                }
            }

            SetNumber(calculatedField.Name, calculate);
        }

        base.SetNumber(Network.SpatialWeightIdentifier, GetSpatialWeight);
    }

    private void PreapplyPassiveTreatment()
    {
        CheckPassiveTreatmentCostIsZero();
        ApplyTreatmentButNotMetadata(SimulationRunner.Simulation.DesignatedPassiveTreatment);
    }

    private void SendToSimulationLogIfNeeded(PerformanceCurve curve, double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            var key = curve.Attribute.Name;
            var errorMessage = SimulationLogMessages.AssetCalculationReturned(Asset, curve, key, value);
            var messageBuilder = SimulationLogMessageBuilders.CalculationFatal(errorMessage, SimulationRunner.Simulation.Id);
            SimulationRunner.Send(messageBuilder);
        }
    }

    private void SetHistoricalValues<T>(int referenceYear, bool fallForward, IEnumerable<Attribute<T>> attributes, Action<string, T> setValue)
    {
        foreach (var attribute in attributes)
        {
            var attributeHistory = Asset.GetHistory(attribute);
            if (attributeHistory.TryGetValue(referenceYear, out var value))
            {
                setValue(attribute.Name, value);
            }
            else if (fallForward)
            {
                var earliestFutureYear = attributeHistory.Years.Where(year => year > referenceYear).AsNullables().Min();
                if (earliestFutureYear.HasValue)
                {
                    setValue(attribute.Name, attributeHistory[earliestFutureYear.Value]);
                }
                else
                {
                    setValue(attribute.Name, attributeHistory.MostRecentValue);
                }
            }
        }
    }

    private void SetInitialValues<T>(IEnumerable<Attribute<T>> attributes, Action<string, T> setValue)
    {
        foreach (var attribute in attributes)
        {
            var initialValue = Asset.GetHistory(attribute).MostRecentValue;
            setValue(attribute.Name, initialValue);
        }
    }

    private void UnfixCalculatedFieldValuesWithoutPreDeteriorationTiming()
    {
        foreach (var calculatedField in AllCalculatedFields.Where(cf => cf.Timing != CalculatedFieldTiming.PreDeterioration))
        {
            _ = NumberCache_Override.Remove(calculatedField.Name);
        }
    }
}
