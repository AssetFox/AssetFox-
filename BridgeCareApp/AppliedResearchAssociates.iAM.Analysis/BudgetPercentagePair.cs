using System;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis;

public sealed class BudgetPercentagePair : WeakEntity, IValidator
{
    internal BudgetPercentagePair(Budget budget) => Budget = budget ?? throw new ArgumentNullException(nameof(budget));

    public string ShortDescription => $"Percentage {Percentage}";

    public Budget Budget { get; }

    public decimal Percentage { get; set; }

    public ValidatorBag Subvalidators => new ValidatorBag();

    public ValidationResultBag GetDirectValidationResults()
    {
        var results = new ValidationResultBag();

        if (Percentage < 0)
        {
            results.Add(ValidationStatus.Error, "Percentage is less than zero.", this, nameof(Percentage));
        }
        else if (Percentage > 100)
        {
            results.Add(ValidationStatus.Error, "Percentage is greater than 100.", this, nameof(Percentage));
        }

        return results;
    }
}
