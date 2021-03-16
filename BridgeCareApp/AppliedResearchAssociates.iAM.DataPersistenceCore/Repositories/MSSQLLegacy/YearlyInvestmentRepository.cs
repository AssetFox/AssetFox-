using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQLLegacy
{
    public class YearlyInvestmentRepository : MSSQLLegacyRepository, IYearlyInvestmentRepository
    {
        public YearlyInvestmentRepository(LegacyDbContext context) : base(context)
        {
        }

        public Dictionary<string, Budget> GetYearlyBudgetAmount(Guid simulationId, int firstYearOfAnalysisPeriod, int numberOfYears)
        {
            // It is hardcoded value for district 8
            var yearlyBudgetEntity = Context.yearlyInvestmentResults.Where(_ => _.SIMULATIONID == 1171);

            var explorer = new Explorer();
            var network = new Network(explorer);
            var simulation = new Simulation(network);
            var investmentPlan = new InvestmentPlan(simulation);
            investmentPlan.NumberOfYearsInAnalysisPeriod = numberOfYears;

            var budgetPerName = investmentPlan.Budgets.ToDictionary(budget => budget.Name, SelectionEqualityComparer<string>.Create(name => name.Trim()));
            foreach (var item in yearlyBudgetEntity)
            {
                var budgetName = item.BUDGETNAME;
                var newBudget = investmentPlan.AddBudget();
                newBudget.Name = budgetName;
                if (!budgetPerName.ContainsKey(budgetName))
                {
                    budgetPerName.Add(budgetName, newBudget);
                }
                var budget = budgetPerName[budgetName];
                var year = item.YEAR_;
                var yearOffset = year - firstYearOfAnalysisPeriod;
                budget.YearlyAmounts[yearOffset].Value += (decimal)item.AMOUNT;
            }
            return budgetPerName;
        }
    }
}
