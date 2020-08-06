using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class CommittedProject : Treatment
    {
        public CommittedProject(Section section, int year)
        {
            Section = section ?? throw new ArgumentNullException(nameof(section));
            Year = year;
        }

        public Budget Budget { get; set; }

        public ICollection<TreatmentConsequence> Consequences { get; } = new SetWithoutNulls<TreatmentConsequence>();

        public double Cost { get; set; }

        public Section Section { get; }

        public SelectableTreatment TemplateTreatment
        {
            get => _TemplateTreatment;
            set
            {
                _TemplateTreatment = value;

                Name = TemplateTreatment.Name;

                Consequences.Clear();
                foreach (var templateConsequence in TemplateTreatment.Consequences)
                {
                    var consequence = new TreatmentConsequence { Attribute = templateConsequence.Attribute };
                    consequence.Change.Expression = templateConsequence.Change.Expression;
                    Consequences.Add(consequence);
                }
            }
        }

        public int Year { get; }

        public override ValidationResultBag GetDirectValidationResults()
        {
            var results = base.GetDirectValidationResults();

            if (Budget == null)
            {
                results.Add(ValidationStatus.Error, "Budget is unset.", this, nameof(Budget));
            }

            if (Cost < 0)
            {
                results.Add(ValidationStatus.Error, "Cost is less than zero.", this, nameof(Cost));
            }
            else if (Cost == 0)
            {
                results.Add(ValidationStatus.Warning, "Cost is zero.", this, nameof(Cost));
            }

            if (Consequences.Select(consequence => consequence.Attribute).Distinct().Count() < Consequences.Count)
            {
                results.Add(ValidationStatus.Error, "At least one attribute is acted on by more than one consequence.", this, nameof(Consequences));
            }

            return results;
        }

        public override IEnumerable<TreatmentScheduling> GetSchedulings() => Enumerable.Empty<TreatmentScheduling>();

        internal override bool CanUseBudget(Budget budget) => budget == Budget;

        internal override IReadOnlyCollection<Action> GetConsequenceActions(CalculateEvaluateScope scope) => Consequences.Select(consequence => consequence.GetRecalculator(scope)).ToArray();

        internal override double GetCost(CalculateEvaluateScope scope, bool shouldApplyMultipleFeasibleCosts) => Cost;

        private SelectableTreatment _TemplateTreatment;
    }
}
