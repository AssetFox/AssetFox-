using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis;

public sealed class ConditionalTreatmentConsequence : TreatmentConsequence
{
    internal ConditionalTreatmentConsequence(Explorer explorer)
    {
        if (explorer == null)
        {
            throw new ArgumentNullException(nameof(explorer));
        }

        Criterion = new Criterion(explorer);
        Equation = new Equation(explorer);
    }

    public Criterion Criterion { get; }

    public Equation Equation { get; }

    public override ValidatorBag Subvalidators
    {
        get
        {
            var validators = base.Subvalidators.Add(Criterion);
            if (!Equation.ExpressionIsBlank)
            {
                _ = validators.Remove(Change).Add(Equation);
            }

            return validators;
        }
    }

    public override ValidationResultBag GetDirectValidationResults()
    {
        var results = base.GetDirectValidationResults();

        if (!Equation.ExpressionIsBlank && !(Attribute is NumberAttribute))
        {
            results.Add(ValidationStatus.Error, "Equation is set and attribute is not a number.", this);
        }

        return results;
    }

    internal override IEnumerable<ChangeApplicator> GetChangeApplicators(AssetContext scope, Treatment treatment)
    {
        var applicators = base.GetChangeApplicators(scope, treatment);

        if (!Equation.ExpressionIsBlank && Attribute is NumberAttribute)
        {
            var newValue = Equation.Compute(scope);
            if (double.IsNaN(newValue) || double.IsInfinity(newValue))
            {
                var errorMessage = SimulationLogMessages.TreatmentConsequenceReturned(scope.Asset, treatment, Equation, this, newValue);
                var logBuilder = SimulationLogMessageBuilders.CalculationFatal(errorMessage, scope.SimulationRunner.Simulation.Id);
                scope.SimulationRunner.Send(logBuilder);
            }
            var equationApplicator = new ChangeApplicator(() => scope.SetNumber(Attribute.Name, newValue), newValue);
            applicators = applicators.Append(equationApplicator);
        }

        return applicators;
    }
}
