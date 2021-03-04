using System;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using MoreLinq;


namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class InvestmentPlanMapper
    {
        public static InvestmentPlanEntity ToEntity(this InvestmentPlan domain, Guid simulationId) =>
            new InvestmentPlanEntity
            {
                Id = domain.Id,
                SimulationId = simulationId,
                FirstYearOfAnalysisPeriod = domain.FirstYearOfAnalysisPeriod,
                InflationRatePercentage = domain.InflationRatePercentage,
                MinimumProjectCostLimit = domain.MinimumProjectCostLimit,
                NumberOfYearsInAnalysisPeriod = domain.NumberOfYearsInAnalysisPeriod
            };

        public static void FillSimulationInvestmentPlan(this InvestmentPlanEntity entity, Simulation simulation)
        {
            simulation.InvestmentPlan.Id = entity.Id;
            simulation.InvestmentPlan.FirstYearOfAnalysisPeriod = entity.FirstYearOfAnalysisPeriod;
            simulation.InvestmentPlan.InflationRatePercentage = entity.InflationRatePercentage;
            simulation.InvestmentPlan.MinimumProjectCostLimit = entity.MinimumProjectCostLimit;
            simulation.InvestmentPlan.NumberOfYearsInAnalysisPeriod = entity.NumberOfYearsInAnalysisPeriod;

            entity.Simulation.BudgetLibrarySimulationJoin?.BudgetLibrary.Budgets.ForEach(_ =>
            {
                var budget = simulation.InvestmentPlan.AddBudget();
                budget.Id = _.Id;
                budget.Name = _.Name;

                if (_.BudgetAmounts.Any())
                {
                    var sortedBudgetAmountEntities = _.BudgetAmounts.OrderBy(__ => __.Year);
                    sortedBudgetAmountEntities.ForEach(__ =>
                    {
                        var year = __.Year;
                        var yearOffset = year - simulation.InvestmentPlan.FirstYearOfAnalysisPeriod;
                        budget.YearlyAmounts[yearOffset].Id = __.Id;
                        budget.YearlyAmounts[yearOffset].Value = __.Value;
                    });
                }

                var budgetCondition = simulation.InvestmentPlan.AddBudgetCondition();
                budgetCondition.Budget = budget;
                budgetCondition.Criterion.Expression = _.CriterionLibraryBudgetJoin?.CriterionLibrary.MergedCriteriaExpression ?? string.Empty;
            });


            entity.Simulation.CashFlowRuleLibrarySimulationJoin?.CashFlowRuleLibrary.CashFlowRules.ForEach(_ =>
            {
                var cashFlowRule = simulation.InvestmentPlan.AddCashFlowRule();
                cashFlowRule.Id = _.Id;
                cashFlowRule.Name = _.Name;
                cashFlowRule.Criterion.Expression = _.CriterionLibraryCashFlowRuleJoin?.CriterionLibrary.MergedCriteriaExpression ?? string.Empty;

                if (_.CashFlowDistributionRules.Any())
                {
                    var sortedDistributionRules = _.CashFlowDistributionRules.OrderBy(__ => __.DurationInYears);
                    sortedDistributionRules.ForEach(__ =>
                    {
                        var distributionRule = cashFlowRule.DistributionRules.GetAdd(new CashFlowDistributionRule());
                        distributionRule.Expression = __.YearlyPercentages;
                        if (__.CostCeiling > 0)
                        {
                            distributionRule.CostCeiling = __.CostCeiling;
                        }
                    });
                }
            });
        }
    }
}
