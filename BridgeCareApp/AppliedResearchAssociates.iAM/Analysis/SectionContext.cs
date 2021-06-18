using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.Analysis
{
    internal sealed class SectionContext : CalculateEvaluateScope
    {
        public SectionContext(Section section, SimulationRunner simulationRunner)
        {
            Section = section ?? throw new ArgumentNullException(nameof(section));
            SimulationRunner = simulationRunner ?? throw new ArgumentNullException(nameof(simulationRunner));

            ResetDetail();

            Initialize();
        }

        public SectionContext(SectionContext original) : base(original)
        {
            Section = original.Section;
            SimulationRunner = original.SimulationRunner;

            ResetDetail();

            EventSchedule.CopyFrom(original.EventSchedule);
            FirstUnshadowedYearForAnyTreatment = original.FirstUnshadowedYearForAnyTreatment;
            FirstUnshadowedYearForSameTreatment.CopyFrom(original.FirstUnshadowedYearForSameTreatment);
            NumberCache.CopyFrom(original.NumberCache);

            InitializeCalculatedFields();
        }

        public SectionDetail Detail { get; private set; }

        public IDictionary<int, Choice<Treatment, TreatmentProgress>> EventSchedule { get; } = new Dictionary<int, Choice<Treatment, TreatmentProgress>>();

        public Section Section { get; }

        public SimulationRunner SimulationRunner { get; }

        public SectionSummaryDetail SummaryDetail
        {
            get
            {
                var detail = new SectionSummaryDetail(Section);
                CopyAttributeValuesToDetail(detail);
                return detail;
            }
        }

        private AnalysisMethod AnalysisMethod => SimulationRunner.Simulation.AnalysisMethod;

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

        public void ApplyTreatmentMetadataIfPending(int year)
        {
            if (AppliedTreatmentWithPendingMetadata is object)
            {
                ApplyTreatmentMetadata(year);
            }
        }

        public void CopyAttributeValuesToDetail() => CopyAttributeValuesToDetail(Detail);

        public void CopyDetailFrom(SectionContext other) => Detail = new SectionDetail(other.Detail);

        public void FixCalculatedFieldValues()
        {
            foreach (var calculatedField in SimulationRunner.Simulation.Network.Explorer.CalculatedFields)
            {
                NumberCache_Override[calculatedField.Name] = GetNumber(calculatedField.Name);
            }
        }

        public double GetBenefit()
        {
            var rawBenefit = GetNumber(AnalysisMethod.Benefit.Attribute.Name);
            var benefit = AnalysisMethod.Benefit.LimitValue(rawBenefit);

            if (AnalysisMethod.Weighting != null)
            {
                var weight = GetNumber(AnalysisMethod.Weighting.Name);
                benefit *= weight;
            }

            return benefit;
        }

        public double GetCostOfTreatment(Treatment treatment) => treatment.GetCost(this, AnalysisMethod.ShouldApplyMultipleFeasibleCosts);

        public override double GetNumber(string key)
        {
            if (GetNumber_ActiveKeysOfCurrentInvocation.Contains(key, StringComparer.OrdinalIgnoreCase))
            {
                var loop = GetNumber_ActiveKeysOfCurrentInvocation.SkipWhile(activeKey => !StringComparer.OrdinalIgnoreCase.Equals(activeKey, key)).Append(key);
                var loopText = string.Join(" to ", loop.Select(activeKey => "[" + activeKey + "]"));

                var messageBuilder = new SimulationMessageBuilder("Loop encountered during number calculation: " + loopText)
                {
                    SectionName = Section.Name,
                    SectionId = Section.Id,
                };
                var logBuilder = SimulationLogMessageBuilders.CalculationFatal(messageBuilder.ToString(), SimulationRunner.Simulation.Id);
                SimulationRunner.Send(logBuilder);
            }

            GetNumber_ActiveKeysOfCurrentInvocation.Push(key);

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

            _ = GetNumber_ActiveKeysOfCurrentInvocation.Pop();

            return number;
        }

        public double GetSpatialWeight()
        {
            var r = Section.SpatialWeighting.Compute(this);
            if (double.IsNaN(r) || double.IsInfinity(r))
            {
                var errorMessage = SimulationLogMessages.SpatialWeightCalculationReturned(Section, Section.SpatialWeighting, r);
                var messageBuilder = SimulationLogMessageBuilders.CalculationFatal(errorMessage, SimulationRunner.Simulation.Id);
                SimulationRunner.Send(messageBuilder);
            }
            return r;
        }

        public void MarkTreatmentProgress(Treatment treatment)
        {
            Detail.AppliedTreatment = treatment.Name;
            Detail.TreatmentStatus = TreatmentStatus.Progressed;
        }

        public void PreapplyPassiveTreatment()
        {
            CheckPassiveTreatmentCostIsZero();
            ApplyTreatmentButNotMetadata(SimulationRunner.Simulation.DesignatedPassiveTreatment);
        }

        public void ResetDetail() => Detail = new SectionDetail(Section);

        public void RollForward()
        {
            // Per email on 2020-05-06 from Gregg to Jake, Chad, and William: "We roll forward
            // attributes with performance curves. To do so we need to know which performance curve
            // to use. When evaluating which performance curve to use it is necessary to evaluate
            // the criteria. Ideally, the attribute in the criteria will have a value for the year
            // in question. If it does not, use the Most Recent Value (from the rollup). If it does
            // not have a Most Recent Value, use the default. Currently, I don't believe the Roll
            // Forward uses the No Treatment consequences. It should in the new code."

            IEnumerable<int?> getMostRecentYearPerAttribute<T>(IEnumerable<Attribute<T>> attributes) =>
                attributes.Select(attribute => Section.GetHistory(attribute).Keys.AsNullables().Max());

            var earliestYearOfMostRecentValue = Enumerable.Concat(
                getMostRecentYearPerAttribute(SimulationRunner.Simulation.Network.Explorer.NumberAttributes),
                getMostRecentYearPerAttribute(SimulationRunner.Simulation.Network.Explorer.TextAttributes)
                ).Min();

            if (earliestYearOfMostRecentValue < SimulationRunner.Simulation.InvestmentPlan.FirstYearOfAnalysisPeriod)
            {
                SetHistoricalValues(earliestYearOfMostRecentValue.Value, true, SimulationRunner.Simulation.Network.Explorer.NumberAttributes, SetNumber);
                SetHistoricalValues(earliestYearOfMostRecentValue.Value, true, SimulationRunner.Simulation.Network.Explorer.TextAttributes, SetText);

                ApplyPerformanceCurves();
                ApplyPassiveTreatment(earliestYearOfMostRecentValue.Value);

                foreach (var year in Static.RangeFromBounds(earliestYearOfMostRecentValue.Value + 1, SimulationRunner.Simulation.InvestmentPlan.FirstYearOfAnalysisPeriod - 1))
                {
                    SetHistoricalValues(year, false, SimulationRunner.Simulation.Network.Explorer.NumberAttributes, SetNumber);
                    SetHistoricalValues(year, false, SimulationRunner.Simulation.Network.Explorer.TextAttributes, SetText);

                    ApplyPerformanceCurves();
                    ApplyPassiveTreatment(year);
                }
            }

            SetHistoricalValues(SimulationRunner.Simulation.InvestmentPlan.FirstYearOfAnalysisPeriod, false, SimulationRunner.Simulation.Network.Explorer.NumberAttributes, SetNumber);
            SetHistoricalValues(SimulationRunner.Simulation.InvestmentPlan.FirstYearOfAnalysisPeriod, false, SimulationRunner.Simulation.Network.Explorer.TextAttributes, SetText);
        }

        public override void SetNumber(string key, double value)
        {
            NumberCache.Clear();
            base.SetNumber(key, value);
        }

        public override void SetNumber(string key, Func<double> getValue)
        {
            NumberCache.Clear();
            base.SetNumber(key, getValue);
        }

        public override void SetText(string key, string value)
        {
            NumberCache.Clear();
            base.SetText(key, value);
        }

        public void UnfixCalculatedFieldValues() => NumberCache_Override.Clear();

        public bool YearIsWithinShadowForAnyTreatment(int year) => year < FirstUnshadowedYearForAnyTreatment;

        public bool YearIsWithinShadowForSameTreatment(int year, Treatment treatment) => FirstUnshadowedYearForSameTreatment.TryGetValue(treatment.Name, out var firstUnshadowedYear) && year < firstUnshadowedYear;

        private static readonly StringComparer KeyComparer = StringComparer.OrdinalIgnoreCase;

        private readonly IDictionary<string, int> FirstUnshadowedYearForSameTreatment = new Dictionary<string, int>();

        private readonly Stack<string> GetNumber_ActiveKeysOfCurrentInvocation = new Stack<string>();

        private readonly IDictionary<string, double> NumberCache = new Dictionary<string, double>(KeyComparer);

        private readonly IDictionary<string, double> NumberCache_Override = new Dictionary<string, double>(KeyComparer);

        private Treatment AppliedTreatmentWithPendingMetadata;

        private int? FirstUnshadowedYearForAnyTreatment;

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
            var consequenceActions = treatment.GetConsequenceActions(this);
            foreach (var consequenceAction in consequenceActions)
            {
                consequenceAction();
            }

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
                    Detail.TreatmentSchedulingCollisions.Add(new TreatmentSchedulingCollisionDetail(schedulingYear, scheduling.Treatment.Name));
                }
                else
                {
                    EventSchedule.Add(schedulingYear, scheduling.Treatment);
                }
            }

            FirstUnshadowedYearForAnyTreatment = year + treatment.ShadowForAnyTreatment;
            FirstUnshadowedYearForSameTreatment[treatment.Name] = year + treatment.ShadowForSameTreatment;

            Detail.AppliedTreatment = treatment.Name;
            Detail.TreatmentStatus = TreatmentStatus.Applied;
        }

        private double CalculateValueOnCurve(PerformanceCurve curve) => curve.Equation.Compute(this, curve);

        private double CalculateValueOnCurve(PerformanceCurve curve, Action<double> handle)
        {
            var value = CalculateValueOnCurve(curve);
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
                    SectionName = Section.Name,
                    SectionId = Section.Id,
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

        private void CopyAttributeValuesToDetail(SectionSummaryDetail detail)
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

        private Func<double> GetCalculator(IGrouping<NumberAttribute, PerformanceCurve> curves)
        {
            curves.Channel(
                curve => curve.Criterion.Evaluate(this),
                result => result ?? false,
                result => !result.HasValue,
                out var applicableCurves,
                out var defaultCurves);

            var operativeCurves = applicableCurves.Count > 0 ? applicableCurves : defaultCurves;

            if (operativeCurves.Count == 0)
            {
                var messageBuilder = new SimulationMessageBuilder("No performance curves are operative for a deteriorating attribute.")
                {
                    ItemName = curves.Key.Name,
                    SectionName = Section.Name,
                    SectionId = Section.Id,
                };

                var logBuilder = SimulationLogMessageBuilders.RuntimeFatal(messageBuilder, SimulationRunner.Simulation.Id);
                SimulationRunner.Send(logBuilder);
            }

            if (operativeCurves.Count > 1)
            {
                var messageBuilder = new SimulationMessageBuilder("Two or more performance curves are simultaneously operative for a single deteriorating attribute.")
                {
                    ItemName = curves.Key.Name,
                    SectionName = Section.Name,
                    SectionId = Section.Id,
                };

                var logMessage = SimulationLogMessageBuilders.RuntimeWarning(messageBuilder, SimulationRunner.Simulation.Id);
                SimulationRunner.Send(logMessage);
            }

            Func<double>
                calculateMinimum = () => operativeCurves.Min(curve => CalculateValueOnCurve(curve, value => SendToSimulationLogIfNeeded(curve, value))),
                calculateMaximum = () => operativeCurves.Max(curve => CalculateValueOnCurve(curve, value => SendToSimulationLogIfNeeded(curve, value)));

            return curves.Key.IsDecreasingWithDeterioration ? calculateMinimum : calculateMaximum;
        }

        private IDictionary<string, Func<double>> GetPerformanceCurveCalculatorPerAttribute() => SimulationRunner.CurvesPerAttribute.ToDictionary(curves => curves.Key.Name, GetCalculator);

        private void Initialize()
        {
            var initialReferenceYear = SimulationRunner.Simulation.InvestmentPlan.FirstYearOfAnalysisPeriod;

            SetInitialValues(SimulationRunner.Simulation.Network.Explorer.NumberAttributes, SetNumber);
            SetInitialValues(SimulationRunner.Simulation.Network.Explorer.TextAttributes, SetText);

            foreach (var committedProject in SimulationRunner.CommittedProjectsPerSection[Section])
            {
                EventSchedule.Add(committedProject.Year, committedProject);
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

        private void SendToSimulationLogIfNeeded(PerformanceCurve curve, double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                var key = curve.Attribute.Name;
                var errorMessage = SimulationLogMessages.SectionCalculationReturned(Section, curve, key, value);
                var messageBuilder = SimulationLogMessageBuilders.CalculationFatal(errorMessage, SimulationRunner.Simulation.Id);
                SimulationRunner.Send(messageBuilder);
            }
        }

        private void SetHistoricalValues<T>(int referenceYear, bool fallForward, IEnumerable<Attribute<T>> attributes, Action<string, T> setValue)
        {
            foreach (var attribute in attributes)
            {
                var attributeHistory = Section.GetHistory(attribute);
                if (attributeHistory.TryGetValue(referenceYear, out var value))
                {
                    setValue(attribute.Name, value);
                }
                else if (fallForward)
                {
                    var earliestFutureYear = attributeHistory.Keys.Where(year => year > referenceYear).AsNullables().Min();
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
                var initialValue = Section.GetHistory(attribute).MostRecentValue;
                setValue(attribute.Name, initialValue);
            }
        }
    }
}
