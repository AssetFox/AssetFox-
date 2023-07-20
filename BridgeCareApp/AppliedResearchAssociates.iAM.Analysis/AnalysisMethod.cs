using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis;

public sealed class AnalysisMethod : WeakEntity, IValidator
{
    internal AnalysisMethod(Simulation simulation)
    {
        Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));

        Filter = new Criterion(Simulation.Network.Explorer);
    }

    /// <summary>
    ///     When this is enabled, the order in which the budgets are considered for funding is
    ///     determined by the order of <see cref="InvestmentPlan.Budgets"/>. Formerly named "ShouldUseExtraFundsAcrossBudgets".
    /// </summary>
    public bool AllowFundingFromMultipleBudgets { get; set; }

    public Benefit Benefit { get; } = new Benefit();

    public IReadOnlyCollection<BudgetPriority> BudgetPriorities => _BudgetPriorities;

    public IReadOnlyCollection<DeficientConditionGoal> DeficientConditionGoals => _DeficientConditionGoals;

    public string Description { get; set; }

    public Criterion Filter { get; }

    public OptimizationStrategy OptimizationStrategy { get; set; }

    public IReadOnlyCollection<RemainingLifeLimit> RemainingLifeLimits => _RemainingLifeLimits;

    public bool ShouldApplyMultipleFeasibleCosts { get; set; }

    public bool ShouldDeteriorateDuringCashFlow { get; set; }

    public bool ShouldRestrictCashFlowToFirstYearBudgets { get; set; } = true;

    public SpendingStrategy SpendingStrategy { get; set; } = SpendingStrategy.AsBudgetPermits;

    public ValidatorBag Subvalidators => new ValidatorBag { Benefit, BudgetPriorities, DeficientConditionGoals, Filter, RemainingLifeLimits, TargetConditionGoals };

    public IReadOnlyCollection<TargetConditionGoal> TargetConditionGoals => _TargetConditionGoals;

    public INumericAttribute Weighting { get; set; }

    public BudgetPriority AddBudgetPriority()
    {
        var budgetPriority = new BudgetPriority(Simulation.Network.Explorer);
        budgetPriority.SynchronizeWithBudgets(Simulation.InvestmentPlan.Budgets);
        _BudgetPriorities.Add(budgetPriority);
        return budgetPriority;
    }

    public DeficientConditionGoal AddDeficientConditionGoal() => _DeficientConditionGoals.GetAdd(new DeficientConditionGoal(Simulation.Network.Explorer));

    public RemainingLifeLimit AddRemainingLifeLimit() => _RemainingLifeLimits.GetAdd(new RemainingLifeLimit(Simulation.Network.Explorer));

    public TargetConditionGoal AddTargetConditionGoal() => _TargetConditionGoals.GetAdd(new TargetConditionGoal(Simulation.Network.Explorer));

    public ValidationResultBag GetDirectValidationResults()
    {
        var results = new ValidationResultBag();

        if (!OptimizationStrategy.IsDefined())
        {
            results.Add(ValidationStatus.Error, MessageStrings.InvalidOptimizationStrategy, this, nameof(OptimizationStrategy));
        }

        if (!SpendingStrategy.IsDefined())
        {
            results.Add(ValidationStatus.Error, MessageStrings.InvalidSpendingStrategy, this, nameof(SpendingStrategy));
        }

        var prioritiesByLevelYear = BudgetPriorities.ToLookup(priority => (priority.PriorityLevel, priority.Year));
        if (prioritiesByLevelYear.Any(group => group.Count() > 1 && group.Any(priority => priority.Criterion.ExpressionIsBlank)))
        {
            results.Add(ValidationStatus.Error, "At least one priority level-year has multiple criteria where at least one criterion is blank.", this, nameof(BudgetPriorities));
        }

        var deficientConditionGoalNames = GetNames(DeficientConditionGoals);
        if (deficientConditionGoalNames.Distinct().Count() < deficientConditionGoalNames.Count)
        {
            results.Add(ValidationStatus.Error, "Multiple deficient condition goals have the same name.", this, nameof(DeficientConditionGoals));
        }

        var targetConditionGoalNames = GetNames(TargetConditionGoals);
        if (targetConditionGoalNames.Distinct().Count() < targetConditionGoalNames.Count)
        {
            results.Add(ValidationStatus.Error, "Multiple target condition goals have the same name.", this, nameof(TargetConditionGoals));
        }

        var remainingLifeLimitsWithBlankCriterion = RemainingLifeLimits.Where(limit => limit.Criterion.ExpressionIsBlank).ToArray();
        if (remainingLifeLimitsWithBlankCriterion.Select(limit => limit.Attribute).Distinct().Count() < remainingLifeLimitsWithBlankCriterion.Length)
        {
            results.Add(ValidationStatus.Warning, "At least one attribute has more than one remaining life limit with a blank criterion.", this, nameof(RemainingLifeLimits));
        }

        if (OptimizationStrategy.UsesRemainingLife() && RemainingLifeLimits.Count == 0)
        {
            results.Add(ValidationStatus.Error, "Optimization strategy uses remaining life, but no remaining life limits are defined.", this);
        }

        return results;
    }

    public void Remove(TargetConditionGoal targetConditionGoal) => _TargetConditionGoals.Remove(targetConditionGoal);

    public void Remove(DeficientConditionGoal deficientConditionGoal) => _DeficientConditionGoals.Remove(deficientConditionGoal);

    public void Remove(BudgetPriority budgetPriority) => _BudgetPriorities.Remove(budgetPriority);

    public void Remove(RemainingLifeLimit remainingLifeLimit) => _RemainingLifeLimits.Remove(remainingLifeLimit);

    internal Func<TreatmentOption, double> ObjectiveFunction
    {
        get
        {
            return OptimizationStrategy switch
            {
                OptimizationStrategy.Benefit => option => option.Benefit,
                OptimizationStrategy.BenefitToCostRatio => option => option.Benefit / option.Cost,
                OptimizationStrategy.RemainingLife => option => option.RemainingLife.Value,
                OptimizationStrategy.RemainingLifeToCostRatio => option => option.RemainingLife.Value / option.Cost,
                _ => throw new InvalidOperationException(MessageStrings.InvalidOptimizationStrategy),
            };
        }
    }

    internal SpendingLimit SpendingLimit
    {
        get
        {
            return SpendingStrategy switch
            {
                SpendingStrategy.NoSpending => SpendingLimit.Zero,
                SpendingStrategy.UnlimitedSpending => SpendingLimit.NoLimit,
                SpendingStrategy.UntilTargetAndDeficientConditionGoalsMet => SpendingLimit.NoLimit,
                SpendingStrategy.UntilTargetConditionGoalsMet => SpendingLimit.NoLimit,
                SpendingStrategy.UntilDeficientConditionGoalsMet => SpendingLimit.NoLimit,
                SpendingStrategy.AsBudgetPermits => SpendingLimit.Budget,
                _ => throw new InvalidOperationException(MessageStrings.InvalidSpendingStrategy),
            };
        }
    }

    public string ShortDescription => nameof(AnalysisMethod);

    private readonly List<BudgetPriority> _BudgetPriorities = new List<BudgetPriority>();

    private readonly List<DeficientConditionGoal> _DeficientConditionGoals = new List<DeficientConditionGoal>();

    private readonly List<RemainingLifeLimit> _RemainingLifeLimits = new List<RemainingLifeLimit>();

    private readonly List<TargetConditionGoal> _TargetConditionGoals = new List<TargetConditionGoal>();

    private readonly Simulation Simulation;

    private static IReadOnlyCollection<string> GetNames(IEnumerable<ConditionGoal> conditionGoals) => conditionGoals.Select(conditionGoal => conditionGoal.Name).Where(name => !string.IsNullOrWhiteSpace(name)).ToArray();
}
