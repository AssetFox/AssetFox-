﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAMCore.Analysis
{
    internal sealed class TreatmentOutlook
    {
        public TreatmentOutlook(SimulationRunner simulationRunner, SectionContext templateContext, SelectableTreatment initialTreatment, int initialYear, IEnumerable<RemainingLifeCalculator.Factory> remainingLifeCalculatorFactories)
        {
            SimulationRunner = simulationRunner ?? throw new ArgumentNullException(nameof(simulationRunner));
            TemplateContext = templateContext ?? throw new ArgumentNullException(nameof(templateContext));
            InitialTreatment = initialTreatment ?? throw new ArgumentNullException(nameof(initialTreatment));
            InitialYear = initialYear;

            if (remainingLifeCalculatorFactories is null)
            {
                throw new ArgumentNullException(nameof(remainingLifeCalculatorFactories));
            }

            RemainingLifeCalculators = remainingLifeCalculatorFactories.Select(factory => factory.Create(AccumulationContext)).ToArray();

            AccumulationContext = new SectionContext(TemplateContext);
            MostRecentBenefit = AccumulationContext.GetBenefit();

            Run();
        }

        public TreatmentOption GetOptionRelativeToBaseline(TreatmentOutlook baseline)
        {
            if (TemplateContext != baseline.TemplateContext)
            {
                throw new ArgumentException("Template context does not match.", nameof(baseline));
            }

            return new TreatmentOption(
                TemplateContext,
                InitialTreatment,
                CumulativeCost - baseline.CumulativeCost,
                CumulativeBenefit - baseline.CumulativeBenefit,
                RemainingLife - baseline.RemainingLife);
        }

        private readonly SimulationRunner SimulationRunner;
        private readonly SectionContext TemplateContext;
        private readonly SectionContext AccumulationContext;
        private readonly SelectableTreatment InitialTreatment;
        private readonly int InitialYear;
        private readonly IReadOnlyCollection<RemainingLifeCalculator> RemainingLifeCalculators;

        private double CumulativeBenefit;
        private double CumulativeCost;
        private double MostRecentBenefit;
        private double? RemainingLife;

        private void AccumulateBenefit()
        {
            // The "cumulative benefit" is the "area under the curve" (a key phrase from the
            // legacy system). To accumulate, we want to add the trapezoidal area between the
            // previous data point and the current data point.

            var additionalBenefit = MostRecentBenefit; // Start with the rectangle area of (benefit_0) * 1 year.
            MostRecentBenefit = AccumulationContext.GetBenefit(); // Update benefit_1.
            additionalBenefit += (MostRecentBenefit - additionalBenefit) / 2; // Add the right triangle area of ((b_1 - b_0) * 1 year) / 2.

            CumulativeBenefit += additionalBenefit;
        }

        private void ApplyTreatment(Treatment treatment, int year)
        {
            var cost = AccumulationContext.GetCostOfTreatment(treatment);
            CumulativeCost += cost * SimulationRunner.GetInflationFactor(year);

            AccumulationContext.ApplyTreatment(treatment, year);
        }

        private void Run()
        {
            Action updateRemainingLife = null;

            if (RemainingLifeCalculators.Count > 0)
            {
                RemainingLife = 0;

                updateRemainingLife = delegate
                {
                    foreach (var calculator in RemainingLifeCalculators)
                    {
                        calculator.UpdateValue();
                    }

                    var minimumFractionalRemainingLife = Enumerable.Min(
                        from calculator in RemainingLifeCalculators
                        where calculator.CurrentValueIsBeyondLimit()
                        select calculator.GetLimitLocationRelativeToLatestValues().AsNullable());

                    if (minimumFractionalRemainingLife.HasValue)
                    {
                        RemainingLife += minimumFractionalRemainingLife.Value;
                        updateRemainingLife = null;
                    }
                    else
                    {
                        RemainingLife += 1;
                    }
                };
            }

            ApplyTreatment(InitialTreatment, InitialYear);

            AccumulateBenefit();
            updateRemainingLife?.Invoke();

            foreach (var year in Enumerable.Range(InitialYear + 1, AccumulationContext.SimulationRunner.Simulation.NumberOfYearsOfTreatmentOutlook))
            {
                var yearIsScheduled = AccumulationContext.EventSchedule.TryGetValue(year, out var scheduledEvent);

                if (yearIsScheduled && scheduledEvent.IsT2())
                {
                    AccumulationContext.SimulationRunner.Fail(MessageStrings.TreatmentOutlookIsConsumingAProgressEvent);
                }

                AccumulationContext.ApplyPerformanceCurves();

                if (yearIsScheduled && scheduledEvent.IsT1(out var treatment))
                {
                    ApplyTreatment(treatment, year);
                }
                else
                {
                    AccumulationContext.ApplyPassiveTreatment(year);
                }

                AccumulateBenefit();
                updateRemainingLife?.Invoke();
            }
        }
    }
}
