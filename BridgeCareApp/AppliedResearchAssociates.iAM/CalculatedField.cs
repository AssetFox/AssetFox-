﻿using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class CalculatedField : Attribute
    {
        public ICollection<ConditionalEquation> Equations { get; } = new CollectionWithoutNulls<ConditionalEquation>();

        public override ICollection<ValidationResult> ValidationResults
        {
            get
            {
                var results = base.ValidationResults;

                if (Equations.Count == 0)
                {
                    results.Add(ValidationStatus.Error.Describe("There are no equations."));
                }
                else
                {
                    var numberOfEquationsWithBlankCriterion = Equations.Count(equation => equation.Criterion.ExpressionIsBlank);
                    if (numberOfEquationsWithBlankCriterion == 0)
                    {
                        results.Add(ValidationStatus.Warning.Describe("There are no equations with a blank criterion."));
                    }
                    else if (numberOfEquationsWithBlankCriterion > 1)
                    {
                        results.Add(ValidationStatus.Error.Describe("There are multiple equations with a blank criterion."));
                    }
                }

                return results;
            }
        }

        public double Calculate(CalculateEvaluateArgument argument, NumberAttribute ageAttribute)
        {
            Equations.Channel(
                equation => equation.Criterion.Evaluate(argument),
                result => result ?? false,
                result => !result.HasValue,
                out var applicableEquations,
                out var defaultEquations);

            var operativeEquations = applicableEquations.Count > 0 ? applicableEquations : defaultEquations;

            if (operativeEquations.Count == 0)
            {
                throw new SimulationException(MessageStrings.CalculatedFieldHasNoOperativeEquations);
            }

            if (operativeEquations.Count > 1)
            {
                throw new SimulationException(MessageStrings.CalculatedFieldHasMultipleOperativeEquations);
            }

            return operativeEquations[0].Equation.Compute(argument, ageAttribute);
        }

        internal CalculatedField(Explorer explorer) : base(explorer)
        {
        }
    }
}
