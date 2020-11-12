using System;
using System.Collections.Generic;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Domains
{
    public abstract class Treatment : IValidator
    {
        public string Name { get; set; }

        public int ShadowForAnyTreatment
        {
            get => _ShadowForAnyTreatment;
            set => _ShadowForAnyTreatment = Math.Max(value, DEFAULT_SHADOW);
        }

        public int ShadowForSameTreatment
        {
            get => _ShadowForSameTreatment;
            set => _ShadowForSameTreatment = Math.Max(value, DEFAULT_SHADOW);
        }

        public virtual ValidatorBag Subvalidators => new ValidatorBag();

        public virtual ValidationResultBag GetDirectValidationResults()
        {
            var results = new ValidationResultBag();

            if (string.IsNullOrWhiteSpace(Name))
            {
                results.Add(ValidationStatus.Error, "Name is blank.", this, nameof(Name));
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

        private const int DEFAULT_SHADOW = 1;

        private int _ShadowForAnyTreatment = DEFAULT_SHADOW;

        private int _ShadowForSameTreatment = DEFAULT_SHADOW;
    }
}
