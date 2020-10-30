﻿using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public class CriterionEquationPair : IValidator
    {
        public CriterionEquationPair(Explorer explorer)
        {
            Criterion = new Criterion(explorer);
            Equation = new Equation(explorer);
        }

        public Criterion Criterion { get; }

        public Equation Equation { get; }

        public ValidatorBag Subvalidators => new ValidatorBag { Criterion, Equation };

        public ValidationResultBag GetDirectValidationResults() => new ValidationResultBag();
    }
}