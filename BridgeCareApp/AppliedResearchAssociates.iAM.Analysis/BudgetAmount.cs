using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis;

public sealed class BudgetAmount : WeakEntity, IValidator
{
    internal BudgetAmount()
    {
    }

    public string ShortDescription => "Amount";

    public ValidatorBag Subvalidators => new ValidatorBag();

    public decimal Value { get; set; }

    public ValidationResultBag GetDirectValidationResults()
    {
        var results = new ValidationResultBag();

        if (Value < 0)
        {
            results.Add(ValidationStatus.Error, "Amount is less than zero.", this, nameof(Value));
        }
        else if (Value == 0)
        {
            results.Add(ValidationStatus.Warning, "Amount is zero.", this, nameof(Value));
        }

        return results;
    }
}
