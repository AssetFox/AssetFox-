﻿using System.Collections.Generic;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAMCore
{
    public class TreatmentConsequence : IValidator
    {
        public Attribute Attribute { get; set; }

        public AttributeValueChange Change { get; } = new AttributeValueChange();

        public virtual ValidatorBag Subvalidators => new ValidatorBag { Change };

        public virtual ValidationResultBag GetDirectValidationResults()
        {
            var results = new ValidationResultBag();

            if (Attribute == null)
            {
                results.Add(ValidationStatus.Error, "Attribute is unset.", this, nameof(Attribute));
            }

            return results;
        }

        internal virtual IEnumerable<ChangeApplicator> GetChangeApplicators(CalculateEvaluateScope scope) => Change.GetApplicator(Attribute, scope).Once();
    }
}
