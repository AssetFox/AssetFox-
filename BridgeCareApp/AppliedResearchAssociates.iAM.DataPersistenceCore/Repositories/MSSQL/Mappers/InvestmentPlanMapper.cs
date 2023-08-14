using System;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
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
                NumberOfYearsInAnalysisPeriod = domain.NumberOfYearsInAnalysisPeriod,
                ShouldAccumulateUnusedBudgetAmounts = domain.ShouldAccumulateUnusedBudgetAmounts
            };

        public static InvestmentPlanEntity ToEntity(this InvestmentPlanDTO dto, Guid simulationId) =>
            new InvestmentPlanEntity
            {
                Id = dto.Id,
                SimulationId = simulationId,
                FirstYearOfAnalysisPeriod = dto.FirstYearOfAnalysisPeriod,
                InflationRatePercentage = dto.InflationRatePercentage,
                MinimumProjectCostLimit = dto.MinimumProjectCostLimit,
                NumberOfYearsInAnalysisPeriod = dto.NumberOfYearsInAnalysisPeriod,
                ShouldAccumulateUnusedBudgetAmounts = dto.ShouldAccumulateUnusedBudgetAmounts
            };

        public static InvestmentPlanEntity ToEntityNullPropagating(this InvestmentPlanDTO dto, Guid simulationId)
        {
            if (dto == null)
            {
                return null;
            }
            return ToEntity(dto, simulationId);
        }



        public static InvestmentPlanDTO ToDto(this InvestmentPlanEntity entity) =>
            new InvestmentPlanDTO
            {
                Id = entity.Id,
                FirstYearOfAnalysisPeriod = entity.FirstYearOfAnalysisPeriod,
                InflationRatePercentage = entity.InflationRatePercentage,
                MinimumProjectCostLimit = entity.MinimumProjectCostLimit,
                NumberOfYearsInAnalysisPeriod = entity.NumberOfYearsInAnalysisPeriod,
                ShouldAccumulateUnusedBudgetAmounts = entity.ShouldAccumulateUnusedBudgetAmounts
            };

        public static void FillSimulationInvestmentPlan(this InvestmentPlanEntity entity, Simulation simulation)
        {
            simulation.InvestmentPlan.Id = entity.Id;
            simulation.InvestmentPlan.FirstYearOfAnalysisPeriod = entity.FirstYearOfAnalysisPeriod;
            simulation.InvestmentPlan.InflationRatePercentage = entity.InflationRatePercentage;
            simulation.InvestmentPlan.MinimumProjectCostLimit = entity.MinimumProjectCostLimit;
            simulation.InvestmentPlan.NumberOfYearsInAnalysisPeriod = entity.NumberOfYearsInAnalysisPeriod;
            simulation.InvestmentPlan.ShouldAccumulateUnusedBudgetAmounts = entity.ShouldAccumulateUnusedBudgetAmounts;
            
            entity.Simulation.Budgets?.OrderBy(_ => _.BudgetOrder).ForEach(_ =>
            {
                var budget = simulation.InvestmentPlan.AddBudget();
                budget.Id = _.Id;
                budget.Name = _.Name;
                if (_.ScenarioBudgetAmounts.Any())
                {
                    var sortedBudgetAmountEntities = _.ScenarioBudgetAmounts.OrderBy(__ => __.Year);
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
                budgetCondition.Criterion.Expression =
                    _.CriterionLibraryScenarioBudgetJoin?.CriterionLibrary.MergedCriteriaExpression ?? string.Empty;
            });

            entity.Simulation.CashFlowRules.ForEach(_ =>
            {
                var cashFlowRule = simulation.InvestmentPlan.AddCashFlowRule();
                cashFlowRule.Id = _.Id;
                cashFlowRule.Name = _.Name;
                cashFlowRule.Criterion.Expression =
                    _.CriterionLibraryScenarioCashFlowRuleJoin?.CriterionLibrary.MergedCriteriaExpression ?? string.Empty;

                if (_.ScenarioCashFlowDistributionRules.Any())
                {
                    var sortedDistributionRules = _.ScenarioCashFlowDistributionRules.OrderBy(__ => __.DurationInYears);
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
