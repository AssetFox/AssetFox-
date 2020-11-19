using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using MoreLinq;


namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class InvestmentPlanMapper
    {
        public static InvestmentPlanEntity ToEntity(this InvestmentPlan domain) =>
            new InvestmentPlanEntity
            {
                Id = Guid.NewGuid(),
                FirstYearOfAnalysisPeriod = domain.FirstYearOfAnalysisPeriod,
                InflationRatePercentage = domain.InflationRatePercentage,
                MinimumProjectCostLimit = domain.MinimumProjectCostLimit,
                NumberOfYearsInAnalysisPeriod = domain.NumberOfYearsInAnalysisPeriod
            };

        public static InvestmentPlan ToDomain(this InvestmentPlanEntity entity)
        {
            var simulation = entity.InvestmentPlanSimulationJoins.First().Simulation.ToDomain();
            var investmentPlan = new InvestmentPlan(simulation)
            {
                FirstYearOfAnalysisPeriod = entity.FirstYearOfAnalysisPeriod,
                InflationRatePercentage = entity.InflationRatePercentage,
                MinimumProjectCostLimit = entity.MinimumProjectCostLimit,
                NumberOfYearsInAnalysisPeriod = entity.NumberOfYearsInAnalysisPeriod
            };

            if (entity.Budgets.Any())
            {
                entity.Budgets.ForEach(_ =>
                {
                    var budget = investmentPlan.AddBudget();
                    budget.Name = _.Name;

                    if (_.BudgetAmounts.Any())
                    {
                        var sortedBudgetAmountEntities = _.BudgetAmounts.OrderBy(__ => __.Year);
                        sortedBudgetAmountEntities.ForEach(__ =>
                        {
                            var year = __.Year;
                            var yearOffset = year - investmentPlan.FirstYearOfAnalysisPeriod;
                            budget.YearlyAmounts[yearOffset].Value = __.Value;
                        });
                    }

                    var budgetCondition = investmentPlan.AddBudgetCondition();
                    budgetCondition.Budget = budget;
                    budgetCondition.Criterion.Expression = _.CriterionLibraryBudgetJoin?.CriterionLibrary.MergedCriteriaExpression ?? string.Empty;
                });
            }

            var simulationEntity = entity.InvestmentPlanSimulationJoins.First().Simulation;
            simulationEntity.CashFlowRuleLibrarySimulationJoin?.CashFlowRuleLibrary.CashFlowRules.ForEach(_ =>
            {
                var cashFlowRule = investmentPlan.AddCashFlowRule();
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

            return investmentPlan;
        }
    }
}
