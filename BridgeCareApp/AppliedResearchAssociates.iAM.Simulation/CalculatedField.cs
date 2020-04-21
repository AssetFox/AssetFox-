﻿using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class CalculatedField
    {
        public List<Criterial<Equation>> Equations { get; }

        public string Name { get; }

        public double Calculate(CalculateEvaluateArgument argument)
        {
            Equations.Channel(
                equation => equation.Criterion.Evaluate(argument),
                result => result ?? false,
                result => !result.HasValue,
                out var applicableEquations,
                out var defaultEquations);

            var operativeEquations = applicableEquations.Count > 0 ? applicableEquations : defaultEquations;
            var operativeEquation = operativeEquations.Single();

            return operativeEquation.Item.Calculate(argument);
        }
    }
}
