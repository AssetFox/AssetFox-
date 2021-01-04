﻿using System;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Domains
{
    public sealed class TreatmentSupersession : WeakEntity, IValidator
    {
        public TreatmentSupersession(Explorer explorer) => Criterion = new Criterion(explorer ?? throw new ArgumentNullException(nameof(explorer)));

        public Criterion Criterion { get; }

        public ValidatorBag Subvalidators => new ValidatorBag { Criterion };

        public SelectableTreatment Treatment { get; set; }

        public ValidationResultBag GetDirectValidationResults()
        {
            var results = new ValidationResultBag();

            if (Treatment == null)
            {
                results.Add(ValidationStatus.Error, "Treatment is unset.", this, nameof(Treatment));
            }

            return results;
        }
    }
}