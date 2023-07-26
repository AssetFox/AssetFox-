using System;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis;

public sealed class BudgetCondition : WeakEntity, IValidator
{
    internal BudgetCondition(Explorer explorer) => Criterion = new Criterion(explorer ?? throw new ArgumentNullException(nameof(explorer)));

    public Budget Budget { get; set; }
    public string ShortDescription => nameof(BudgetCondition);

    public Criterion Criterion { get; }

    public ValidatorBag Subvalidators => new ValidatorBag { Criterion };

    public ValidationResultBag GetDirectValidationResults()
    {
        var results = new ValidationResultBag();

        if (Budget == null)
        {
            results.Add(ValidationStatus.Error, "Budget is unset.", this, nameof(Budget));
        }

        return results;
    }
}
