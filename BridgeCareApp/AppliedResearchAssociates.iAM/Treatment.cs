using System;
using System.Collections.Generic;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public abstract class Treatment : IValidator
    {
        public string Name { get; set; }

        public int ShadowForAnyTreatment { get; set; }

        public int ShadowForSameTreatment { get; set; }

        public virtual ValidatorBag Subvalidators => new ValidatorBag();

        public virtual ValidationResultBag GetDirectValidationResults()
        {
            var results = new ValidationResultBag();

            if (string.IsNullOrWhiteSpace(Name))
            {
                results.Add(ValidationStatus.Error, "Name is blank.", this, nameof(Name));
            }

            if (ShadowForAnyTreatment < 1)
            {
                results.Add(ValidationStatus.Error, "Shadow for any treatment is less than one.", this, nameof(ShadowForAnyTreatment));
            }

            if (ShadowForSameTreatment < 1)
            {
                results.Add(ValidationStatus.Error, "Shadow for same treatment is less than one.", this, nameof(ShadowForSameTreatment));
            }

            if (ShadowForSameTreatment < ShadowForAnyTreatment)
            {
                results.Add(ValidationStatus.Warning, "\"Same\" shadow is less than \"any\" shadow.", this);
            }

            return results;
        }

        public abstract IEnumerable<TreatmentScheduling> GetSchedulings();

        internal abstract bool CanUseBudget(Budget budget);

        internal abstract IReadOnlyCollection<Action> GetConsequenceActions(CalculateEvaluateScope scope);

        internal abstract double GetCost(CalculateEvaluateScope scope, bool shouldApplyMultipleFeasibleCosts);
    }
}
