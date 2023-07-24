using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis;

public sealed class Explorer : IValidator
{
    public static string DefaultAgeAttributeName => "AGE";

    public Explorer(string ageAttributeName)
    {
        if (string.IsNullOrWhiteSpace(ageAttributeName))
        {
            throw new ArgumentException("Age attribute name is blank.", nameof(ageAttributeName));
        }

        _ = AddNumberAttribute(AgeAttributeName = ageAttributeName);
    }

    public Explorer() : this(DefaultAgeAttributeName)
    {
    }

    public INumericAttribute AgeAttribute { get; private set; }

    public IEnumerable<Attribute> AllAttributes => CalculatedFields.Concat<Attribute>(NumberAttributes).Concat(TextAttributes);

    public IReadOnlyCollection<CalculatedField> CalculatedFields => _CalculatedFields;

    public IReadOnlyCollection<Network> Networks => _Networks;

    public IReadOnlyCollection<NumberAttribute> NumberAttributes => _NumberAttributes;

    public IEnumerable<INumericAttribute> NumericAttributes => CalculatedFields.Concat<INumericAttribute>(NumberAttributes);

    public string ShortDescription => nameof(Explorer);

    public ValidatorBag Subvalidators => new ValidatorBag { CalculatedFields, Networks, NumberAttributes };

    public IReadOnlyCollection<TextAttribute> TextAttributes => _TextAttributes;

    public CalculatedField AddCalculatedField(string name) => Add(new CalculatedField(name, this), _CalculatedFields, CalculateEvaluateParameterType.Number);

    public Network AddNetwork() => _Networks.GetAdd(new Network(this));

    public NumberAttribute AddNumberAttribute(string name) => Add(new NumberAttribute(name), _NumberAttributes, CalculateEvaluateParameterType.Number);

    public TextAttribute AddTextAttribute(string name) => Add(new TextAttribute(name), _TextAttributes, CalculateEvaluateParameterType.Text);

    public ValidationResultBag GetDirectValidationResults()
    {
        var results = new ValidationResultBag();

        if (AgeAttribute.IsDecreasingWithDeterioration)
        {
            results.Add(ValidationStatus.Error, "Age attribute must increase with deterioration.", this, nameof(AgeAttribute));
        }

        if (Networks.Select(network => network.Name).Distinct().Count() < Networks.Count)
        {
            results.Add(ValidationStatus.Error, "Multiple networks have the same name.", this, nameof(Networks));
        }

        var calculatedFieldByName = CalculatedFields.ToDictionary(field => field.Name);
        var cycles = CycleDetection.FindAllCycles(CalculatedFields, field => field.ValueSources.SelectMany(getReferencedFields));

        IEnumerable<CalculatedField> getReferencedFields(CalculatedFieldValueSource source) => source.Equation.ReferencedParameters.SelectMany(getField);
        IEnumerable<CalculatedField> getField(string parameter) => calculatedFieldByName.TryGetValue(parameter, out var referencedField) ? referencedField.Once() : Enumerable.Empty<CalculatedField>();

        foreach (var cycle in cycles)
        {
            var loopText = string.Join(" to ", cycle.Append(cycle[0]).Select(field => "[" + field.Name + "]"));
            results.Add(ValidationStatus.Warning, "Possible loop among calculated fields: " + loopText, this, nameof(CalculatedFields));
        }

        return results;
    }

    public void Remove(CalculatedField attribute) => Remove(attribute, _CalculatedFields);

    public void Remove(Network network) => _ = _Networks.Remove(network);

    public void Remove(NumberAttribute attribute) => Remove(attribute, _NumberAttributes);

    public void Remove(TextAttribute attribute) => Remove(attribute, _TextAttributes);

    internal CalculateEvaluateCompiler Compiler { get; } = new CalculateEvaluateCompiler();

    private readonly List<CalculatedField> _CalculatedFields = new List<CalculatedField>();

    private readonly List<Network> _Networks = new List<Network>();

    private readonly List<NumberAttribute> _NumberAttributes = new List<NumberAttribute>();

    private readonly List<TextAttribute> _TextAttributes = new List<TextAttribute>();

    private readonly string AgeAttributeName;

    private T Add<T>(T attribute, ICollection<T> attributes, CalculateEvaluateParameterType parameterType) where T : Attribute
    {
        var isAgeAttribute = StringComparer.OrdinalIgnoreCase.Equals(attribute.Name, AgeAttributeName);

        if (!isAgeAttribute)
        {
            if (parameterType is CalculateEvaluateParameterType.Number && StringComparer.OrdinalIgnoreCase.Equals(attribute.Name, Network.SpatialWeightIdentifier))
            {
                throw new ArgumentException($"Cannot use the reserved spatial weight identifier \"{Network.SpatialWeightIdentifier}\" as a numeric attribute name.");
            }

            if (AllAttributes.Any(a => StringComparer.OrdinalIgnoreCase.Equals(a.Name, attribute.Name)))
            {
                throw new ArgumentException("Attribute name is already in use for another attribute.", nameof(attribute));
            }
        }

        if (isAgeAttribute)
        {
            if (parameterType != CalculateEvaluateParameterType.Number)
            {
                throw new ArgumentException("Cannot add a non-numeric age attribute.");
            }

            _ = Compiler.ParameterTypes.Remove(AgeAttributeName);

            if (AgeAttribute is NumberAttribute na)
            {
                _ = _NumberAttributes.Remove(na);
            }
            else if (AgeAttribute is CalculatedField cf)
            {
                _ = _CalculatedFields.Remove(cf);
            }

            AgeAttribute = (INumericAttribute)attribute;
        }

        Compiler.ParameterTypes.Add(attribute.Name, parameterType);
        attributes.Add(attribute);
        return attribute;
    }

    private void Remove<T>(T attribute, ICollection<T> attributes) where T : Attribute
    {
        if (attribute != AgeAttribute)
        {
            if (!Compiler.ParameterTypes.Remove(attribute.Name))
            {
                throw new InvalidOperationException("Failed to remove parameter from compiler.");
            }

            _ = attributes.Remove(attribute);
        }
    }
}
