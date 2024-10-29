using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis;

public sealed class CalculatedField : Attribute, INumericAttribute, IValidator
{
    public bool IsDecreasingWithDeterioration { get; set; }

    public string ShortDescription => Name;

    public ValidatorBag Subvalidators => new ValidatorBag { ValueSources };

    /// <summary>
    ///     When to "fix" the value of this field during an analysis year.
    /// </summary>
    public CalculatedFieldTiming Timing
    {
        get => Enum.IsDefined(typeof(CalculatedFieldTiming), _Timing) ? _Timing : CalculatedFieldTiming.OnDemand;
        set => _Timing = value;
    }

    public IReadOnlyCollection<CalculatedFieldValueSource> ValueSources => _ValueSources;

    public CalculatedFieldValueSource AddValueSource() => _ValueSources.GetAdd(new CalculatedFieldValueSource(Explorer));

    public ValidationResultBag GetDirectValidationResults()
    {
        var results = new ValidationResultBag();

        if (ValueSources.Count is 0)
        {
            results.Add(ValidationStatus.Error, "There are no value sources.", this, nameof(ValueSources));
        }
        else
        {
            var numberOfSourcesWithBlankCriterion = ValueSources.Count(source => source.Criterion.ExpressionIsBlank);
            if (numberOfSourcesWithBlankCriterion == 0)
            {
                results.Add(ValidationStatus.Error, "There are no value sources with a blank criterion.", this, nameof(ValueSources));
            }
        }

        return results;
    }

    public void Remove(CalculatedFieldValueSource source) => _ValueSources.Remove(source);

    internal CalculatedField(string name, Explorer explorer) : base(name) => Explorer = explorer ?? throw new ArgumentNullException(nameof(explorer));

    internal double Calculate(AssetContext scope)
    {
        List<CalculatedFieldValueSource> applicableSources = new();
        List<CalculatedFieldValueSource> defaultSources = new();

        foreach (var source in _ValueSources)
        {
            var evaluation = scope.Evaluate(source.Criterion);

            if (!evaluation.HasValue)
            {
                defaultSources.Add(source);
            }
            else if (evaluation.Value)
            {
                applicableSources.Add(source);
            }
        }

        var operativeSources = applicableSources.Count > 0 ? applicableSources : defaultSources;

        if (operativeSources.Count == 0)
        {
            var messageBuilder = new SimulationMessageBuilder(MessageStrings.CalculatedFieldHasNoOperativeEquations)
            {
                ItemName = Name,
                AssetName = scope.Asset.AssetName,
                AssetId = scope.Asset.Id,
            };

            throw new SimulationException(messageBuilder.ToString());
        }

        if (operativeSources.Count == 1)
        {
            return Compute(operativeSources[0].Equation, scope);
        }

        var potentialValues = operativeSources.Select(source => Compute(source.Equation, scope)).ToArray();

        return IsDecreasingWithDeterioration ? potentialValues.Min() : potentialValues.Max();
    }

    private readonly List<CalculatedFieldValueSource> _ValueSources = new List<CalculatedFieldValueSource>();

    private readonly Explorer Explorer;

    private CalculatedFieldTiming _Timing = CalculatedFieldTiming.OnDemand;

    private double Compute(Equation equation, AssetContext assetContext)
    {
        var equationValue = equation.Compute(assetContext);
        if (double.IsNaN(equationValue) || double.IsInfinity(equationValue))
        {
            var errorMessage = SimulationLogMessages.CalculatedFieldReturned(assetContext.Asset, equation, Name, equationValue);
            var messageBuilder = SimulationLogMessageBuilders.CalculationFatal(errorMessage, assetContext.SimulationRunner.Simulation.Id);
            assetContext.SimulationRunner.Send(messageBuilder);
        }
        return equationValue;
    }
}
