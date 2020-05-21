﻿using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class TreatmentSupersession : IValidator
    {
        public Criterion Criterion { get; } = new Criterion();

        public SelectableTreatment Treatment { get; set; }

        public ICollection<ValidationResult> DirectValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (Treatment == null)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, this, "Treatment is unset."));
                }

                return results;
            }
        }
    }
}
