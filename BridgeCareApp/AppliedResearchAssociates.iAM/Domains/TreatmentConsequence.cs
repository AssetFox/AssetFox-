﻿using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Domains
{
    public class TreatmentConsequence : WeakEntity, IValidator
    {
        public Attribute Attribute { get => Change.Attribute; set => Change.Attribute = value; }

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

        public string ShortDescription => nameof(TreatmentConsequence);
        
        internal virtual IEnumerable<ChangeApplicator> GetChangeApplicators(SectionContext scope)
        {
            var changeApplicator = Change.GetApplicator(scope);
            return changeApplicator is null
                ? Array.Empty<ChangeApplicator>()
                : (new[] { changeApplicator });
        }
    }
}
