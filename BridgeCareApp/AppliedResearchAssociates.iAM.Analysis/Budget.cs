using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis;

public sealed class Budget : WeakEntity, IValidator
{
    internal Budget(InvestmentPlan investmentPlan) => InvestmentPlan = investmentPlan ?? throw new ArgumentNullException(nameof(investmentPlan));

    public InvestmentPlan InvestmentPlan { get; }

    public string Name { get; set; }

    public string ShortDescription => Name ?? "Unnamed budget";

    public ValidatorBag Subvalidators => new ValidatorBag { YearlyAmounts };

    public IReadOnlyList<BudgetAmount> YearlyAmounts => _YearlyAmounts;

    public ValidationResultBag GetDirectValidationResults()
    {
        var results = new ValidationResultBag();

        if (string.IsNullOrWhiteSpace(Name))
        {
            results.Add(ValidationStatus.Error, "Name is blank.", this, nameof(Name));
        }

        if (YearlyAmounts.Count == 0)
        {
            results.Add(ValidationStatus.Error, "There are no yearly amounts.", this, nameof(YearlyAmounts));
        }

        return results;
    }

    internal void SetNumberOfYears(int numberOfYears)
    {
        if (numberOfYears <= 0)
        {
            _YearlyAmounts.Clear();
        }
        else if (numberOfYears < _YearlyAmounts.Count)
        {
            _YearlyAmounts.RemoveRange(numberOfYears, _YearlyAmounts.Count - numberOfYears);
        }
        else if (numberOfYears > _YearlyAmounts.Count)
        {
            _YearlyAmounts.AddRange(Enumerable.Range(0, numberOfYears - _YearlyAmounts.Count).Select(_ => new BudgetAmount()));
        }
    }

    private readonly List<BudgetAmount> _YearlyAmounts = new List<BudgetAmount>();
}
