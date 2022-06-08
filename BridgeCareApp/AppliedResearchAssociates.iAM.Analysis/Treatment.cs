using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public abstract class Treatment : WeakEntity, IValidator
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

        public string ShortDescription => Name;

        public abstract IEnumerable<TreatmentScheduling> GetSchedulings();

        internal abstract bool CanUseBudget(Budget budget);

        internal abstract IReadOnlyCollection<Action> GetConsequenceActions(AssetContext scope);

        internal abstract double GetCost(AssetContext scope, bool shouldApplyMultipleFeasibleCosts);

        private const int DEFAULT_SHADOW = 1;

        private int _ShadowForAnyTreatment = DEFAULT_SHADOW;

        private int _ShadowForSameTreatment = DEFAULT_SHADOW;
    }
}
