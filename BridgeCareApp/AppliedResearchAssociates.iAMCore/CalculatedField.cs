﻿using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.iAMCore.Analysis;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAMCore
{
    public sealed class CalculatedField : Attribute, INumericAttribute, IValidator
    {
        public CalculatedField(string name, Explorer explorer) : base(name) => Explorer = explorer ?? throw new ArgumentNullException(nameof(explorer));

        public bool IsDecreasingWithDeterioration { get; set; }

        public ValidatorBag Subvalidators => new ValidatorBag { ValueSources };

        public IReadOnlyCollection<CalculatedFieldValueSource> ValueSources => _ValueSources;

        public CalculatedFieldValueSource AddValueSource() => _ValueSources.GetAdd(new CalculatedFieldValueSource(Explorer));

        public double Calculate(CalculateEvaluateScope scope)
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
                throw new SimulationException(MessageStrings.CalculatedFieldHasNoOperativeEquations);
            }

            if (operativeSources.Count == 1)
            {
                return operativeSources[0].Equation.Compute(scope);
            }

            var potentialValues = operativeSources.Select(source => source.Equation.Compute(scope)).ToArray();

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
