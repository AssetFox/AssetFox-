﻿using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;

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
            InitializeCalculatedFields();
        }

        public SectionContext(SectionContext original) : base(original)
        {
            Section = original.Section;
            SimulationRunner = original.SimulationRunner;
            FirstUnshadowedYearForAnyTreatment = original.FirstUnshadowedYearForAnyTreatment;
            FirstUnshadowedYearForSameTreatment.CopyFrom(original.FirstUnshadowedYearForSameTreatment);
            EventSchedule.CopyFrom(original.EventSchedule);
            NumberCache.CopyFrom(original.NumberCache);

            ResetDetail();

            InitializeCalculatedFields();
        }

        public SectionDetail Detail { get; private set; }

        public IDictionary<int, Choice<Treatment, TreatmentProgress>> EventSchedule { get; } = new Dictionary<int, Choice<Treatment, TreatmentProgress>>();

        public Section Section { get; }

        public SimulationRunner SimulationRunner { get; }

        private AnalysisMethod AnalysisMethod => SimulationRunner.Simulation.AnalysisMethod;

        public void ApplyPassiveTreatment(int year)
        {
            var cost = GetCostOfTreatment(SimulationRunner.Simulation.DesignatedPassiveTreatment);
            if (cost != 0)
            {
                throw new SimulationException(MessageStrings.CostOfPassiveTreatmentIsNonZero);
            }

            ApplyTreatment(SimulationRunner.Simulation.DesignatedPassiveTreatment, year);
        }

        public void ApplyTreatment(Treatment treatment, int year)
        {
            var consequenceActions = treatment.GetConsequenceActions(this);
            foreach (var consequenceAction in consequenceActions)
            {
                consequenceAction();
            }

            ApplyPerformanceCurves();

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

        public void CopyAttributeValuesToDetail()
        {
            foreach (var attribute in SimulationRunner.Simulation.Network.Explorer.NumericAttributes)
            {
                Detail.ValuePerNumericAttribute.Add(attribute.Name, GetNumber(attribute.Name));
            }

            foreach (var attribute in SimulationRunner.Simulation.Network.Explorer.TextAttributes)
            {
                Detail.ValuePerTextAttribute.Add(attribute.Name, GetText(attribute.Name));
            }
        }

        public void CopyDetailFrom(SectionContext other) => Detail = new SectionDetail(other.Detail);

        public double GetAreaOfSection() => GetNumber(Section.AreaIdentifier);

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
            IEnumerable<int?> getMostRecentYearPerAttribute<T>(IEnumerable<Attribute<T>> attributes) =>
                attributes.Select(attribute => Section.GetHistory(attribute).Keys.AsNullables().Max());

            var earliestYearOfMostRecentValue = Enumerable.Concat(
                getMostRecentYearPerAttribute(SimulationRunner.Simulation.Network.Explorer.NumberAttributes),
                getMostRecentYearPerAttribute(SimulationRunner.Simulation.Network.Explorer.TextAttributes)
                ).Min();

            if (earliestYearOfMostRecentValue.HasValue)
            {
                var earliestYear = earliestYearOfMostRecentValue.Value;
                if (earliestYear < SimulationRunner.Simulation.InvestmentPlan.FirstYearOfAnalysisPeriod)
                {
                    SetHistoricalValues(earliestYear, true, SimulationRunner.Simulation.Network.Explorer.NumberAttributes, SetNumber);
                    SetHistoricalValues(earliestYear, true, SimulationRunner.Simulation.Network.Explorer.TextAttributes, SetText);

                    foreach (var year in Enumerable.Range(earliestYear + 1, SimulationRunner.Simulation.InvestmentPlan.FirstYearOfAnalysisPeriod - earliestYear))
                    {
                        ApplyPassiveTreatment(year);

                        SetHistoricalValues(year, false, SimulationRunner.Simulation.Network.Explorer.NumberAttributes, SetNumber);
                        SetHistoricalValues(year, false, SimulationRunner.Simulation.Network.Explorer.TextAttributes, SetText);
                    }
                }
            }
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

        public bool YearIsWithinShadowForAnyTreatment(int year) => year < FirstUnshadowedYearForAnyTreatment;

        public bool YearIsWithinShadowForSameTreatment(int year, Treatment treatment) => FirstUnshadowedYearForSameTreatment.TryGetValue(treatment.Name, out var firstUnshadowedYear) && year < firstUnshadowedYear;

        private readonly IDictionary<string, int> FirstUnshadowedYearForSameTreatment = new Dictionary<string, int>();

        private readonly IDictionary<string, double> NumberCache = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

        private int? FirstUnshadowedYearForAnyTreatment;

        private void ApplyPerformanceCurves()
        {
            var dataUpdates = SimulationRunner.CurvesPerAttribute.ToDictionary(curves => curves.Key.Name, curves =>
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
                    throw new SimulationException("No performances curves are operative for a deteriorating attribute.");
                }

                if (operativeCurves.Count > 1)
                {
                    SimulationRunner.Warn("Two or more performance curves are simultaneously operative for a single deteriorating attribute.");
                }

                double calculate(PerformanceCurve curve) => curve.Equation.Compute(this);

                return curves.Key.IsDecreasingWithDeterioration ? operativeCurves.Min(calculate) : operativeCurves.Max(calculate);
            });

            foreach (var (key, value) in dataUpdates)
            {
                SetNumber(key, value);
            }
        }

        private void Initialize()
        {
            SetNumber(Section.AreaIdentifier, Section.Area);

            var initialReferenceYear = SimulationRunner.Simulation.InvestmentPlan.FirstYearOfAnalysisPeriod;

            SetHistoricalValues(initialReferenceYear, true, SimulationRunner.Simulation.Network.Explorer.NumberAttributes, SetNumber);
            SetHistoricalValues(initialReferenceYear, true, SimulationRunner.Simulation.Network.Explorer.TextAttributes, SetText);

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
                double calculate() => calculatedField.Calculate(this);
                SetNumber(calculatedField.Name, calculate);
            }
        }

        private void SetHistoricalValues<T>(int referenceYear, bool useMostRecentAsFallback, IEnumerable<Attribute<T>> attributes, Action<string, T> setValue)
        {
            foreach (var attribute in attributes)
            {
                var attributeHistory = Section.GetHistory(attribute);
                if (attributeHistory.TryGetValue(referenceYear, out var value))
                {
                    setValue(attribute.Name, value);
                }
                else if (useMostRecentAsFallback)
                {
                    var mostRecentYear = attributeHistory.Keys.Where(year => year < referenceYear).AsNullables().Max();
                    if (mostRecentYear.HasValue)
                    {
                        setValue(attribute.Name, attributeHistory[mostRecentYear.Value]);
                    }
                    else
                    {
                        setValue(attribute.Name, attribute.DefaultValue);
                    }
                }
            }
        }
    }
}
