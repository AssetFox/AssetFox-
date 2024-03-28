using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis;

public sealed class SelectableTreatment : Treatment
{
    private const int DEFAULT_SHADOW = 1;

    private readonly List<ConditionalTreatmentConsequence> _Consequences = new();

    private readonly List<TreatmentCost> _Costs = new();

    private readonly List<Criterion> _FeasibilityCriteria = new();

    private readonly List<TreatmentSupersedeRule> _SupersedeRules = new();

    private readonly Simulation Simulation;

    private int _ShadowForAnyTreatment = DEFAULT_SHADOW;

    private int _ShadowForSameTreatment = DEFAULT_SHADOW;

    private ILookup<Attribute, ConditionalTreatmentConsequence> ConsequencesPerAttribute;

    public SelectableTreatment(Simulation simulation) => Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));

    /// <remarks>
    ///     This property isn't used by the analysis engine. It probably shouldn't exist among the
    ///     types in this module.
    /// </remarks>
    public string AssetCategory { get; set; }

    public ICollection<Budget> Budgets { get; } = new SetWithoutNulls<Budget>();

    /// <remarks>
    ///     This property isn't used by the analysis engine. It probably shouldn't exist among the
    ///     types in this module.
    /// </remarks>
    public TreatmentCategory Category { get; set; }

    public IReadOnlyCollection<ConditionalTreatmentConsequence> Consequences => _Consequences;

    public IReadOnlyCollection<TreatmentCost> Costs => _Costs;

    public string Description { get; set; }

    public IReadOnlyCollection<Criterion> FeasibilityCriteria => _FeasibilityCriteria;

    public bool ForCommittedProjectsOnly { get; set; }

    public bool IsPotentialPassiveTreatment => !ForCommittedProjectsOnly && FeasibilityCriteria.All(criterion => criterion.ExpressionIsBlank);

    public override Dictionary<NumberAttribute, double> PerformanceCurveAdjustmentFactors { get; } = new();

    public ICollection<TreatmentScheduling> Schedulings { get; } = new SetWithoutNulls<TreatmentScheduling>();

    public override int ShadowForAnyTreatment => _ShadowForAnyTreatment;

    public override int ShadowForSameTreatment => _ShadowForSameTreatment;

    public override ValidatorBag Subvalidators => base.Subvalidators.Add(Consequences).Add(Costs).Add(FeasibilityCriteria).Add(Schedulings).Add(SupersedeRules);

    public IReadOnlyCollection<TreatmentSupersedeRule> SupersedeRules => _SupersedeRules;

    public ConditionalTreatmentConsequence AddConsequence() => _Consequences.GetAdd(new ConditionalTreatmentConsequence(Simulation.Network.Explorer));

    public TreatmentCost AddCost() => _Costs.GetAdd(new TreatmentCost(Simulation.Network.Explorer));

    public Criterion AddFeasibilityCriterion() => _FeasibilityCriteria.GetAdd(new Criterion(Simulation.Network.Explorer));

    public TreatmentSupersedeRule AddSupersedeRule() => _SupersedeRules.GetAdd(new TreatmentSupersedeRule(Simulation.Network.Explorer));

    public void DesignateAsPassiveForSimulation()
    {
        if (!Simulation.Treatments.Contains(this))
        {
            throw new InvalidOperationException("Simulation does not contain this treatment.");
        }

        if (!IsPotentialPassiveTreatment)
        {
            throw new InvalidOperationException("This treatment is not a potential passive treatment.");
        }

        Simulation.DesignatedPassiveTreatment = this;
    }

    public override ValidationResultBag GetDirectValidationResults()
    {
        var results = base.GetDirectValidationResults();

        if (ShadowForSameTreatment < ShadowForAnyTreatment)
        {
            results.Add(ValidationStatus.Warning, "\"Same\" shadow is less than \"any\" shadow.", this);
        }

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

        var unconditionalSupersedeRulesPerTreatment = SupersedeRules
            .Where(supersedeRule => supersedeRule.Criterion.ExpressionIsBlank)
            .GroupBy(supersedeRule => supersedeRule.Treatment);

        if (unconditionalSupersedeRulesPerTreatment.Any(group => group.Count() > 1))
        {
            results.Add(ValidationStatus.Warning, "At least one treatment is unconditionally superseded more than once.", this, nameof(SupersedeRules));
        }

        foreach (var (attribute, factor) in PerformanceCurveAdjustmentFactors)
        {
            if (factor <= 0)
            {
                results.Add(
                    ValidationStatus.Error,
                    $"Attribute \"{attribute.Name}\" performance curve adjustment factor is non-positive.",
                    this);
            }
        }

        return results;
    }

    public void Remove(TreatmentSupersedeRule supersedeRule) => _SupersedeRules.Remove(supersedeRule);

    public void Remove(ConditionalTreatmentConsequence consequence) => _Consequences.Remove(consequence);

    public void Remove(TreatmentCost cost) => _Costs.Remove(cost);

    public void RemoveFeasibilityCriterion(Criterion criterion) => _FeasibilityCriteria.Remove(criterion);

    public void SetShadowForAnyTreatment(int value) => _ShadowForAnyTreatment = Math.Max(value, DEFAULT_SHADOW);

    public void SetShadowForSameTreatment(int value) => _ShadowForSameTreatment = Math.Max(value, DEFAULT_SHADOW);

    internal override bool CanUseBudget(Budget budget) => Budgets.Contains(budget);

    internal override IReadOnlyCollection<ConsequenceApplicator> GetConsequenceApplicators(AssetContext scope)
    {
        return ConsequencesPerAttribute.SelectMany(getConsequenceApplicator).ToArray();

        ConsequenceApplicator[] getConsequenceApplicator(IGrouping<Attribute, ConditionalTreatmentConsequence> consequences)
        {
            consequences.Channel(
                consequence => scope.Evaluate(consequence.Criterion),
                result => result ?? false,
                result => !result.HasValue,
                out var applicableConsequences,
                out var defaultConsequences);

            var operativeConsequences = applicableConsequences.Count > 0
                ? applicableConsequences
                : defaultConsequences;

            var consequenceApplicators = operativeConsequences
                .SelectMany(consequence => consequence.GetConsequenceApplicators(scope, this))
                .ToArray();

            if (consequenceApplicators.Length == 0)
            {
                return Array.Empty<ConsequenceApplicator>();
            }

            if (consequenceApplicators.Length == 1)
            {
                return consequenceApplicators;
            }

            if (consequences.Key is not NumberAttribute numberAttribute)
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

            var worstConsequenceApplicator = numberAttribute.IsDecreasingWithDeterioration
                ? consequenceApplicators.MinBy(static ca => ca.NewValue.Value)
                : consequenceApplicators.MaxBy(static ca => ca.NewValue.Value);

            return new[] { worstConsequenceApplicator };
        }
    }

    internal override double GetCost(AssetContext scope, bool shouldApplyMultipleFeasibleCosts)
    {
        var feasibleCosts = Costs.Where(cost => cost.Criterion.EvaluateOrDefault(scope)).ToArray();
        if (feasibleCosts.Length == 0)
        {
            // [REVIEW] This doesn't look right. Ask about this no later than the PR. (In other
            // words, don't let this comment through a PR.)
            return 0;
        }

        return shouldApplyMultipleFeasibleCosts
            ? feasibleCosts.Sum(cost => getCost(cost, scope))
            : feasibleCosts.Max(cost => getCost(cost, scope));
    }

    internal override IEnumerable<TreatmentScheduling> GetSchedulings() => Schedulings;

    internal bool IsFeasible(AssetContext scope) => FeasibilityCriteria.Any(feasibility => feasibility.EvaluateOrDefault(scope));

    internal void SetConsequencesPerAttribute() => ConsequencesPerAttribute = Consequences.ToLookup(c => c.Attribute);

    internal void UnsetConsequencesPerAttribute() => ConsequencesPerAttribute = null;

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
}
