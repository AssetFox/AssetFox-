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

        public static void FillSimulationInvestmentPlan(this InvestmentPlanEntity entity, Simulation simulation)
        {
            simulation.InvestmentPlan.FirstYearOfAnalysisPeriod = entity.FirstYearOfAnalysisPeriod;
            simulation.InvestmentPlan.InflationRatePercentage = entity.InflationRatePercentage;
            simulation.InvestmentPlan.MinimumProjectCostLimit = entity.MinimumProjectCostLimit;
            simulation.InvestmentPlan.NumberOfYearsInAnalysisPeriod = entity.NumberOfYearsInAnalysisPeriod;

            if (entity.Budgets.Any())
            {
                entity.Budgets.ForEach(_ =>
                {
                    var budget = simulation.InvestmentPlan.AddBudget();
                    budget.Name = _.Name;

                    if (_.BudgetAmounts.Any())
                    {
                        var sortedBudgetAmountEntities = _.BudgetAmounts.OrderBy(__ => __.Year);
                        sortedBudgetAmountEntities.ForEach(__ =>
                        {
                            var year = __.Year;
                            var yearOffset = year - simulation.InvestmentPlan.FirstYearOfAnalysisPeriod;
                            budget.YearlyAmounts[yearOffset].Value = __.Value;
                        });
                    }

                    var budgetCondition = simulation.InvestmentPlan.AddBudgetCondition();
                    budgetCondition.Budget = budget;
                    budgetCondition.Criterion.Expression = _.CriterionLibraryBudgetJoin?.CriterionLibrary.MergedCriteriaExpression ?? string.Empty;
                });
            }

            var simulationEntity = entity.InvestmentPlanSimulationJoins.First().Simulation;
            simulationEntity.CashFlowRuleLibrarySimulationJoin?.CashFlowRuleLibrary.CashFlowRules.ForEach(_ =>
            {
                var cashFlowRule = simulation.InvestmentPlan.AddCashFlowRule();
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
