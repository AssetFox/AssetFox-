﻿using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAMCore.Analysis
{
    internal sealed class SectionContext : CalculateEvaluateScope
    {
        public SectionContext(Section section, SimulationRunner simulationRunner)
        {
            Section = section ?? throw new ArgumentNullException(nameof(section));
            SimulationRunner = simulationRunner ?? throw new ArgumentNullException(nameof(simulationRunner));

            ResetDetail();

            Initialize();
            InitializeCalculatedFields();
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
            var cost = GetCostOfTreatment(SimulationRunner.Simulation.DesignatedPassiveTreatment);
            if (cost != 0)
            {
                SimulationRunner.Fail(MessageStrings.CostOfPassiveTreatmentIsNonZero);
            }

            ApplyTreatment(SimulationRunner.Simulation.DesignatedPassiveTreatment, year);
        }

        public void ApplyPerformanceCurves() => ApplyPerformanceCurves(GetPerformanceCurveCalculatorPerAttribute());

        public void ApplyTreatment(Treatment treatment, int year)
        {
            try
            {
                var consequenceActions = treatment.GetConsequenceActions(this);
                foreach (var consequenceAction in consequenceActions)
                {
                    consequenceAction();
                }
            }
            catch (SimulationException e)
            {
                SimulationRunner.Fail(e.Message, false);
                throw;
            }

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

            Detail.TreatmentName = treatment.Name;
            Detail.TreatmentStatus = TreatmentStatus.Applied;
        }

        public void CopyAttributeValuesToDetail() => CopyAttributeValuesToDetail(Detail);

        public void CopyDetailFrom(SectionContext other) => Detail = new SectionDetail(other.Detail);

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
                SimulationRunner.Fail("Loop encountered during number calculation: " + loopText);
            }

            GetNumber_ActiveKeysOfCurrentInvocation.Push(key);

            if (!NumberCache.TryGetValue(key, out var number))
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

        public void MarkTreatmentProgress(Treatment treatment)
        {
            Detail.TreatmentName = treatment.Name;
            Detail.TreatmentStatus = TreatmentStatus.Progressed;
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

                SetHistoricalValues(SimulationRunner.Simulation.InvestmentPlan.FirstYearOfAnalysisPeriod, false, SimulationRunner.Simulation.Network.Explorer.NumberAttributes, SetNumber);
                SetHistoricalValues(SimulationRunner.Simulation.InvestmentPlan.FirstYearOfAnalysisPeriod, false, SimulationRunner.Simulation.Network.Explorer.TextAttributes, SetText);
            }
        }

        public override void SetNumber(string key, double value)
        {
            PrepareSet(key);
            base.SetNumber(key, value);
        }

        public override void SetNumber(string key, Func<double> getValue)
        {
            PrepareSet(key);
            base.SetNumber(key, getValue);
        }

        public override void SetText(string key, string value)
        {
            PrepareSet(key);
            base.SetText(key, value);
        }

        private void PrepareSet(string key)
        {
            if (KeyComparer.Equals(key, Section.AreaIdentifier))
            {
                SimulationRunner.Fail("Section area is being mutated. The analysis does not support this.");
            }

            if (KeyComparer.Equals(key, SimulationRunner.Simulation.Network.Explorer.AgeAttribute.Name) && NumberKeys.Contains(key))
            {
                PreviousAge = GetNumber(key);
            }

            NumberCache.Clear();
        }

        public bool YearIsWithinShadowForAnyTreatment(int year) => year < FirstUnshadowedYearForAnyTreatment;

        public bool YearIsWithinShadowForSameTreatment(int year, Treatment treatment) => FirstUnshadowedYearForSameTreatment.TryGetValue(treatment.Name, out var firstUnshadowedYear) && year < firstUnshadowedYear;

        private static readonly StringComparer KeyComparer = StringComparer.OrdinalIgnoreCase;

        private readonly IDictionary<string, int> FirstUnshadowedYearForSameTreatment = new Dictionary<string, int>();

        private readonly Stack<string> GetNumber_ActiveKeysOfCurrentInvocation = new Stack<string>();

        private readonly IDictionary<string, double> NumberCache = new Dictionary<string, double>(KeyComparer);

        private int? FirstUnshadowedYearForAnyTreatment;

        private double PreviousAge;

        private void ApplyPerformanceCurves(IDictionary<string, Func<double>> calculatorPerAttribute)
        {
            var dataUpdates = calculatorPerAttribute.Select(kv => (kv.Key, kv.Value())).ToArray();

            foreach (var (key, value) in dataUpdates)
            {
                SetNumber(key, value);
            }
        }

        private double CalculateValueOnCurve(PerformanceCurve curve) => curve.Equation.Compute(this, curve, PreviousAge);

        private void CopyAttributeValuesToDetail(SectionSummaryDetail detail)
        {
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
                SimulationRunner.Fail("No performance curves are operative for a deteriorating attribute.");
            }

            if (operativeCurves.Count > 1)
            {
                SimulationRunner.Warn("Two or more performance curves are simultaneously operative for a single deteriorating attribute.");
            }

            Func<double>
                calculateMinimum = () => operativeCurves.Min(CalculateValueOnCurve),
                calculateMaximum = () => operativeCurves.Max(CalculateValueOnCurve);

            return curves.Key.IsDecreasingWithDeterioration ? calculateMinimum : calculateMaximum;
        }

        private IDictionary<string, Func<double>> GetPerformanceCurveCalculatorPerAttribute() => SimulationRunner.CurvesPerAttribute.ToDictionary(curves => curves.Key.Name, GetCalculator);

        private void Initialize()
        {
            base.SetNumber(Section.AreaIdentifier, Section.Area);

            var initialReferenceYear = SimulationRunner.Simulation.InvestmentPlan.FirstYearOfAnalysisPeriod;

            SetInitialValues(SimulationRunner.Simulation.Network.Explorer.NumberAttributes, SetNumber);
            SetInitialValues(SimulationRunner.Simulation.Network.Explorer.TextAttributes, SetText);

            foreach (var committedProject in SimulationRunner.CommittedProjectsPerSection[Section])
            {
                EventSchedule.Add(committedProject.Year, committedProject);
            }
        }

        private void InitializeCalculatedFields()
        {
            // This initialization is separate from the rest because it needs to be called again in
            // the copy constructor. Otherwise, the copy's calculated fields will use the original's
            // scope values. (Note that "this" is captured in the local function's closure object.)

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
                        SimulationRunner.Fail(e.Message, false);
                        throw;
                    }
                }

                SetNumber(calculatedField.Name, calculate);
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
