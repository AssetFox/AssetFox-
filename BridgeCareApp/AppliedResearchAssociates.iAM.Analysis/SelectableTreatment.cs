using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.Validation;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class SelectableTreatment : Treatment
    {
        public ICollection<Budget> Budgets { get; } = new SetWithoutNulls<Budget>();

        public IReadOnlyCollection<ConditionalTreatmentConsequence> Consequences => _Consequences;

        public IReadOnlyCollection<TreatmentCost> Costs => _Costs;

        public string Description { get; set; }

        public TreatmentCategory Category { get; set; }

        public AssetCategory AssetCategory { get; set; }

        public IReadOnlyCollection<Criterion> FeasibilityCriteria => _FeasibilityCriteria;

        public ICollection<TreatmentScheduling> Schedulings { get; } = new SetWithoutNulls<TreatmentScheduling>();

        public IReadOnlyCollection<TreatmentSupersession> Supersessions => _Supersessions;

        public override ValidatorBag Subvalidators => base.Subvalidators.Add(Consequences).Add(Costs).Add(FeasibilityCriteria).Add(Schedulings).Add(Supersessions);

        public ConditionalTreatmentConsequence AddConsequence() => _Consequences.GetAdd(new ConditionalTreatmentConsequence(Simulation.Network.Explorer));

        public TreatmentCost AddCost() => _Costs.GetAdd(new TreatmentCost(Simulation.Network.Explorer));

        public Criterion AddFeasibilityCriterion() => _FeasibilityCriteria.GetAdd(new Criterion(Simulation.Network.Explorer));

        public TreatmentSupersession AddSupersession() => _Supersessions.GetAdd(new TreatmentSupersession(Simulation.Network.Explorer));

        public void DesignateAsPassiveForSimulation()
        {
            if (!Simulation.Treatments.Contains(this))
            {
                throw new InvalidOperationException("Simulation does not contain this treatment.");
            }

            Simulation.DesignatedPassiveTreatment = this;
        }

        public override ValidationResultBag GetDirectValidationResults()
        {
            var results = base.GetDirectValidationResults();

            if (Schedulings.Select(scheduling => scheduling.OffsetToFutureYear).Distinct().Count() < Schedulings.Count)
            {
                results.Add(ValidationStatus.Error, "At least one future year has more than one scheduling.", this, nameof(Schedulings));
            }

            var unconditionalConsequencesPerAttribute = Consequences
                .Where(consequence => consequence.Attribute is TextAttribute && consequence.Criterion.ExpressionIsBlank)
                .GroupBy(consequence => consequence.Attribute);

            if (unconditionalConsequencesPerAttribute.Any(group => group.Count() > 1))
            {
                results.Add(ValidationStatus.Error, "At least one text attribute is unconditionally acted on by more than one consequence.", this, nameof(Consequences));
            }

            var unconditionalSupersessionsPerTreatment = Supersessions
                .Where(supersession => supersession.Criterion.ExpressionIsBlank)
                .GroupBy(supersession => supersession.Treatment);

            if (unconditionalSupersessionsPerTreatment.Any(group => group.Count() > 1))
            {
                results.Add(ValidationStatus.Warning, "At least one treatment is unconditionally superseded more than once.", this, nameof(Supersessions));
            }

            return results;
        }

        public override IEnumerable<TreatmentScheduling> GetSchedulings() => Schedulings;

        internal bool IsFeasible(AssetContext scope) => FeasibilityCriteria.Any(feasibility => feasibility.EvaluateOrDefault(scope));

        public void Remove(TreatmentSupersession supersession) => _Supersessions.Remove(supersession);

        public void Remove(ConditionalTreatmentConsequence consequence) => _Consequences.Remove(consequence);

        public void Remove(TreatmentCost cost) => _Costs.Remove(cost);

        public void RemoveFeasibilityCriterion(Criterion criterion) => _FeasibilityCriteria.Remove(criterion);

        public SelectableTreatment(Simulation simulation) => Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));

        internal override bool CanUseBudget(Budget budget) => Budgets.Contains(budget);

        internal override IReadOnlyCollection<Action> GetConsequenceActions(AssetContext scope)
        {
            return ConsequencesPerAttribute.SelectMany(getConsequenceAction).ToArray();

            IEnumerable<Action> getConsequenceAction(IGrouping<Attribute, ConditionalTreatmentConsequence> consequences)
            {
                consequences.Channel(
                    consequence => consequence.Criterion.Evaluate(scope),
                    result => result ?? false,
                    result => !result.HasValue,
                    out var applicableConsequences,
                    out var defaultConsequences);

                var operativeConsequences = applicableConsequences.Count > 0 ? applicableConsequences : defaultConsequences;

                var changeApplicators = operativeConsequences.SelectMany(consequence => consequence.GetChangeApplicators(scope, this)).ToArray();

                if (changeApplicators.Length == 0)
                {
                    return Enumerable.Empty<Action>();
                }

                if (changeApplicators.Length == 1)
                {
                    return changeApplicators[0].Action.Once();
                }

                if (!(consequences.Key is NumberAttribute numberAttribute))
                {
                    var messageBuilder = new SimulationMessageBuilder(MessageStrings.NonNumberAttributeIsBeingActedOnByMultipleConsequences)
                    {
                        ItemName = Name,
                        ItemId = Id,
                        AssetName = scope.Asset.AssetName,
                        AssetId = scope.Asset.Id,
                    };

                    throw new SimulationException(messageBuilder.ToString());
                }

                Array.Sort(changeApplicators, ChangeApplicatorComparer);

                var worstConsequence = numberAttribute.IsDecreasingWithDeterioration ? changeApplicators.First() : changeApplicators.Last();

                return worstConsequence.Action.Once();
            }
        }

        private double getCost(TreatmentCost cost, AssetContext scope)
        {
            var returnValue = cost.Equation.Compute(scope);
            if (double.IsNaN(returnValue) || double.IsInfinity(returnValue))
            {
                var errorMessage = SimulationLogMessages.TreatmentCostReturned(scope.Asset, cost, this, returnValue);
                var message = new SimulationLogMessageBuilder
                {
                    SimulationId = scope.SimulationRunner.Simulation.Id,
                    Status = SimulationLogStatus.Fatal,
                    Message = errorMessage,
                    Subject = SimulationLogSubject.Calculation,
                };
                scope.SimulationRunner.Send(message);
            }
            return returnValue;
        }

        internal override double GetCost(AssetContext scope, bool shouldApplyMultipleFeasibleCosts)
        {
            var feasibleCosts = Costs.Where(cost => cost.Criterion.EvaluateOrDefault(scope)).ToArray();
            if (feasibleCosts.Length == 0)
            {
                return 0;
            }

            return shouldApplyMultipleFeasibleCosts ? feasibleCosts.Sum(cost => getCost(cost, scope)) : feasibleCosts.Max(cost => getCost(cost, scope));
        }

        internal void SetConsequencesPerAttribute() => ConsequencesPerAttribute = Consequences.ToLookup(c => c.Attribute);

        internal void UnsetConsequencesPerAttribute() => ConsequencesPerAttribute = null;

        private static readonly IComparer<ChangeApplicator> ChangeApplicatorComparer = SelectionComparer<ChangeApplicator>.Create(applicator => applicator.Number.Value);

        private readonly List<ConditionalTreatmentConsequence> _Consequences = new List<ConditionalTreatmentConsequence>();

        private readonly List<TreatmentCost> _Costs = new List<TreatmentCost>();

        private readonly List<Criterion> _FeasibilityCriteria = new List<Criterion>();

        private readonly List<TreatmentSupersession> _Supersessions = new List<TreatmentSupersession>();

        private readonly Simulation Simulation;

        private ILookup<Attribute, ConditionalTreatmentConsequence> ConsequencesPerAttribute;
    }
}
