using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAMCore
{
    public sealed class InvestmentPlan : IValidator
    {
        public InvestmentPlan(Simulation simulation)
        {
            Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));

            SynchronizeBudgetPriorities();
        }

        public IReadOnlyCollection<BudgetCondition> BudgetConditions => _BudgetConditions;

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

        public ValidatorBag Subvalidators => new ValidatorBag { BudgetConditions, Budgets, CashFlowRules };

        public IEnumerable<int> YearsOfAnalysis => Enumerable.Range(FirstYearOfAnalysisPeriod, NumberOfYearsInAnalysisPeriod);

        public Budget AddBudget()
        {
            var budget = new Budget();
            budget.SetNumberOfYears(NumberOfYearsInAnalysisPeriod);
            _Budgets.Add(budget);
            SynchronizeBudgetPriorities();
            return budget;
        }

        public BudgetCondition AddBudgetCondition() => _BudgetConditions.GetAdd(new BudgetCondition(Simulation.Network.Explorer));

        public CashFlowRule AddCashFlowRule() => _CashFlowRules.GetAdd(new CashFlowRule(Simulation.Network.Explorer));

        public void DecrementIndexOf(Budget budget) => _Budgets.DecrementIndexOf(budget);

        public void DecrementIndexOf(CashFlowRule cashFlowRule) => _CashFlowRules.DecrementIndexOf(cashFlowRule);

        public ValidationResultBag GetDirectValidationResults()
        {
            var results = new ValidationResultBag();

            if (NumberOfYearsInAnalysisPeriod < 1)
            {
                results.Add(ValidationStatus.Error, "Number of years in analysis period is less than one.", this, nameof(NumberOfYearsInAnalysisPeriod));
            }

            return results;
        }

        public void IncrementIndexOf(Budget budget) => _Budgets.IncrementIndexOf(budget);

        public void IncrementIndexOf(CashFlowRule cashFlowRule) => _CashFlowRules.IncrementIndexOf(cashFlowRule);

        public void Remove(Budget budget)
        {
            if (_Budgets.Remove(budget))
            {
                SynchronizeBudgetPriorities();
            }
        }

        public void Remove(BudgetCondition budgetCondition) => _BudgetConditions.Remove(budgetCondition);

        public void Remove(CashFlowRule cashFlowRule) => _CashFlowRules.Remove(cashFlowRule);

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
    }
}
