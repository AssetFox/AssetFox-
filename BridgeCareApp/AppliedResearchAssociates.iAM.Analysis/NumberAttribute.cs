using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis;

public sealed class NumberAttribute : Attribute<double>, INumericAttribute, IValidator
{
    internal NumberAttribute(string name) : base(name)
    {
    }

    public bool IsDecreasingWithDeterioration { get; set; }

    public double? Maximum { get; set; }

    public double? Minimum { get; set; }

    public ValidatorBag Subvalidators => new ValidatorBag();

    public ValidationResultBag GetDirectValidationResults()
    {
        var results = new ValidationResultBag();

        if (Minimum.HasValue && double.IsNaN(Minimum.Value))
        {
            results.Add(ValidationStatus.Error, "Minimum is not a number.", this, nameof(Minimum));
        }

        if (Maximum.HasValue && double.IsNaN(Maximum.Value))
        {
            results.Add(ValidationStatus.Error, "Maximum is not a number.", this, nameof(Minimum));
        }

        if (Minimum is double minimum && Maximum is double maximum)
        {
            if (minimum == maximum)
            {
                results.Add(ValidationStatus.Warning, "Minimum is equal to maximum.", this);
            }
            else if (minimum > maximum)
            {
                results.Add(ValidationStatus.Error, "Minimum is greater than maximum.", this);
            }
        }

        return results;
    }

    public string ShortDescription => $"{Name} {Minimum}-{Maximum}";
}
