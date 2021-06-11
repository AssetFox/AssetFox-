using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Domains
{
    public sealed class CalculatedField : Attribute, INumericAttribute, IValidator
    {
        internal CalculatedField(string name, Explorer explorer) : base(name) => Explorer = explorer ?? throw new ArgumentNullException(nameof(explorer));

        public string ShortDescription => Name;

        public bool IsDecreasingWithDeterioration { get; set; }

        public ValidatorBag Subvalidators => new ValidatorBag { ValueSources };

        public IReadOnlyCollection<CalculatedFieldValueSource> ValueSources => _ValueSources;

        public CalculatedFieldValueSource AddValueSource() => _ValueSources.GetAdd(new CalculatedFieldValueSource(Explorer));

        private double Compute(Equation equation, SectionContext sectionContext)
        {
            double r = equation.Compute(sectionContext);
            if (double.IsNaN(r) || double.IsInfinity(r))
            {
                var errorMessage = SimulationLogMessages.CalculatedFieldReturned(sectionContext.Section, equation, Name, r);
                var messageBuilder = SimulationLogMessageBuilders.CalculationFatal(errorMessage, sectionContext.SimulationRunner.Simulation.Id);
                sectionContext.SimulationRunner.Send(messageBuilder);
            }
            return r;
        }

        internal double Calculate(SectionContext scope)
        {
            ValueSources.Channel(
                source => source.Criterion.Evaluate(scope),
                result => result ?? false,
                result => !result.HasValue,
                out var applicableSources,
                out var defaultSources);

            var operativeSources = applicableSources.Count > 0 ? applicableSources : defaultSources;

            if (operativeSources.Count == 0)
            {
                var messageBuilder = new SimulationMessageBuilder(MessageStrings.CalculatedFieldHasNoOperativeEquations)
                {
                    ItemName = Name,
                    SectionName = scope.Section.Name,
                    SectionId = scope.Section.Id,
                };

                throw new SimulationException(messageBuilder.ToString());
            }

            if (operativeSources.Count == 1)
            {
                return Compute(operativeSources[0].Equation, scope);
            }

            var potentialValues = operativeSources.Select(source => Compute(source.Equation, scope)).ToArray();

            return IsDecreasingWithDeterioration ? potentialValues.Min() : potentialValues.Max();
        }

        public ValidationResultBag GetDirectValidationResults()
        {
            var results = new ValidationResultBag();

            if (ValueSources.Count == 0)
            {
                results.Add(ValidationStatus.Error, "There are no value sources.", this, nameof(ValueSources));
            }
            else
            {
                var numberOfSourcesWithBlankCriterion = ValueSources.Count(source => source.Criterion.ExpressionIsBlank);
                if (numberOfSourcesWithBlankCriterion == 0)
                {
                    results.Add(ValidationStatus.Warning, "There are no value sources with a blank criterion.", this, nameof(ValueSources));
                }
            }

            return results;
        }

        public void Remove(CalculatedFieldValueSource source) => _ValueSources.Remove(source);

        private readonly List<CalculatedFieldValueSource> _ValueSources = new List<CalculatedFieldValueSource>();

        private readonly Explorer Explorer;
    }
}
