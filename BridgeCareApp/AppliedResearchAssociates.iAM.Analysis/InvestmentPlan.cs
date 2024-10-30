using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis;

public sealed class InvestmentPlan : WeakEntity, IValidator
{
    public IReadOnlyCollection<BudgetCondition> BudgetConditions => _BudgetConditions;

    /// <summary>
    ///     The order of budgets determines the order in which they are considered to fund each treatment.
    /// </summary>
    public IReadOnlyList<Budget> Budgets => _Budgets;

    /// <summary>
    ///     The order of cash flow rules determines precedence when multiple rules may apply.
    /// </summary>
    public IReadOnlyList<CashFlowRule> CashFlowRules => _CashFlowRules;

    [Obsolete("Legacy analysis does not use this correctly.")] //--Gregg
    public double DiscountRatePercentage { get; set; }

    public int FirstYearOfAnalysisPeriod { get; set; }

    public double InflationRatePercentage { get; set; }

    public int LastYearOfAnalysisPeriod => FirstYearOfAnalysisPeriod + NumberOfYearsInAnalysisPeriod - 1;

    public decimal MinimumProjectCostLimit { get; set; }

    public int NumberOfYearsInAnalysisPeriod
    {
        get => _NumberOfYearsInAnalysisPeriod;
        set
        {
            _NumberOfYearsInAnalysisPeriod = value;

            foreach (var budget in Budgets)
            {
                budget.SetNumberOfYears(NumberOfYearsInAnalysisPeriod);
            }
        }
    }

    public bool AllowFundingCarryover { get; set; }

    public ValidatorBag Subvalidators => new ValidatorBag { BudgetConditions, Budgets, CashFlowRules };

    public IEnumerable<int> YearsOfAnalysis => Enumerable.Range(FirstYearOfAnalysisPeriod, NumberOfYearsInAnalysisPeriod);

    /// <summary>
    ///     Add a new budget at the end of the current list of <see cref="Budgets"/>.
    /// </summary>
    public Budget AddBudget()
    {
        var budget = new Budget(this);
        budget.SetNumberOfYears(NumberOfYearsInAnalysisPeriod);
        _Budgets.Add(budget);
        SynchronizeBudgetPriorities();
        return budget;
    }

    public BudgetCondition AddBudgetCondition() => _BudgetConditions.GetAdd(new BudgetCondition(Simulation.Network.Explorer));

    public CashFlowRule AddCashFlowRule() => _CashFlowRules.GetAdd(new CashFlowRule(Simulation.Network.Explorer));

    /// <summary>
    ///     Move the given budget into the next higher priority.
    /// </summary>
    /// <param name="budget"></param>
    public void DecrementIndexOf(Budget budget) => _Budgets.DecrementIndexOf(budget);

    /// <summary>
    ///     Move the given cash flow rule into the next higher priority.
    /// </summary>
    /// <param name="cashFlowRule"></param>
    public void DecrementIndexOf(CashFlowRule cashFlowRule) => _CashFlowRules.DecrementIndexOf(cashFlowRule);

    public ValidationResultBag GetDirectValidationResults()
    {
        var results = new ValidationResultBag();

        if (MinimumProjectCostLimit < 0)
        {
            results.Add(ValidationStatus.Error, "Minimum project cost limit is less than zero.", this, nameof(MinimumProjectCostLimit));
        }

        if (NumberOfYearsInAnalysisPeriod < 1)
        {
            results.Add(ValidationStatus.Error, "Number of years in analysis period is less than one.", this, nameof(NumberOfYearsInAnalysisPeriod));
        }

        var defaultCashFlowRules = CashFlowRules.Where(cf => cf.Criterion.ExpressionIsBlank).ToList();
        if (defaultCashFlowRules.Count != 1)
        {
            addWarningForCfcpRequirement();
        }
        else
        {
            var oneYearDistributions = defaultCashFlowRules[0].DistributionRules.Where(d => d.YearlyPercentages.Count == 1).ToList();
            if (oneYearDistributions.Count != 1)
            {
                addWarningForCfcpRequirement();
            }
            else
            {
                var cashFlowCommittedProjectCostThreshold = oneYearDistributions[0].CostCeiling;
                if (cashFlowCommittedProjectCostThreshold is null)
                {
                    addWarningForCfcpRequirement();
                }
            }
        }

        return results;

        void addWarningForCfcpRequirement()
        {
            results.Add(
                ValidationStatus.Warning,
                "Cash-flow committed projects identification will be disabled due to an unmet requirement: (a) exactly one default cash-flow rule, (b) with exactly one 1-year distribution, (c) with an explicit cost-ceiling value.",
                this,
                nameof(CashFlowRules));
        }
    }

    /// <summary>
    ///     Move the given budget into the next lower priority.
    /// </summary>
    /// <param name="budget"></param>
    public void IncrementIndexOf(Budget budget) => _Budgets.IncrementIndexOf(budget);

    /// <summary>
    ///     Move the given cash flow rule into the next lower priority.
    /// </summary>
    /// <param name="cashFlowRule"></param>
    public void IncrementIndexOf(CashFlowRule cashFlowRule) => _CashFlowRules.IncrementIndexOf(cashFlowRule);

    /// <summary>
    ///     Remove the given budget from the list of <see cref="Budgets"/>.
    /// </summary>
    /// <param name="budget"></param>
    public void Remove(Budget budget)
    {
        if (_Budgets.Remove(budget))
        {
            SynchronizeBudgetPriorities();
        }
    }

    public void Remove(BudgetCondition budgetCondition) => _BudgetConditions.Remove(budgetCondition);

    public void Remove(CashFlowRule cashFlowRule) => _CashFlowRules.Remove(cashFlowRule);

    internal InvestmentPlan(Simulation simulation)
    {
        Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));

        SynchronizeBudgetPriorities();
    }

    internal double GetInflationFactor(int year) => Math.Pow(1 + InflationRatePercentage / 100, year - FirstYearOfAnalysisPeriod);

    private readonly List<BudgetCondition> _BudgetConditions = new List<BudgetCondition>();

    private readonly List<Budget> _Budgets = new List<Budget>();

    private readonly List<CashFlowRule> _CashFlowRules = new List<CashFlowRule>();

    private readonly Simulation Simulation;

    private int _NumberOfYearsInAnalysisPeriod;

    private void SynchronizeBudgetPriorities()
    {
        foreach (var budgetPriority in Simulation.AnalysisMethod.BudgetPriorities)
        {
            budgetPriority.SynchronizeWithBudgets(Budgets);
        }
    }

    public string ShortDescription => nameof(InvestmentPlan);
}
