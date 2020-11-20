using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class AnalysisMethodMapper
    {
        public static AnalysisMethodEntity ToEntity(this AnalysisMethod domain, Guid simulationId, Guid attributeId) =>
            new AnalysisMethodEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                AttributeId = attributeId,
                Description = domain.Description,
                OptimizationStrategy = domain.OptimizationStrategy,
                SpendingStrategy = domain.SpendingStrategy,
                ShouldApplyMultipleFeasibleCosts = domain.ShouldApplyMultipleFeasibleCosts,
                ShouldDeteriorateDuringCashFlow = domain.ShouldDeteriorateDuringCashFlow,
                ShouldUseExtraFundsAcrossBudgets = domain.ShouldUseExtraFundsAcrossBudgets
            };

        public static AnalysisMethod ToDomain(this AnalysisMethodEntity entity, InvestmentPlan investmentPlan)
        {
            var simulation = entity.Simulation.ToDomain();
            simulation.InvestmentPlan.FirstYearOfAnalysisPeriod = investmentPlan.FirstYearOfAnalysisPeriod;
            simulation.InvestmentPlan.NumberOfYearsInAnalysisPeriod = investmentPlan.NumberOfYearsInAnalysisPeriod;
            investmentPlan.Budgets.ToList().ForEach(_ =>
            {
                var budget = simulation.InvestmentPlan.AddBudget();
                budget.Name = _.Name;
                var index = 0;
                _.YearlyAmounts.ToList().ForEach(__ =>
                {
                    budget.YearlyAmounts[index].Value = __.Value;
                    index++;
                });
            });

            var analysisMethod = new AnalysisMethod(simulation)
            {
                Description = entity.Description,
                OptimizationStrategy = entity.OptimizationStrategy,
                SpendingStrategy = entity.SpendingStrategy,
                ShouldApplyMultipleFeasibleCosts = entity.ShouldApplyMultipleFeasibleCosts,
                ShouldDeteriorateDuringCashFlow = entity.ShouldDeteriorateDuringCashFlow,
                ShouldUseExtraFundsAcrossBudgets = entity.ShouldUseExtraFundsAcrossBudgets
            };

            if (entity.Attribute != null)
            {
                analysisMethod.Weighting = new NumberAttribute(entity.Attribute.Name)
                {
                    IsDecreasingWithDeterioration = entity.Attribute.IsAscending
                };
            }

            analysisMethod.Filter.Expression =
                entity.CriterionLibraryAnalysisMethodJoin?.CriterionLibrary.MergedCriteriaExpression ?? string.Empty;

            entity.Simulation.BudgetPriorityLibrarySimulationJoin?.BudgetPriorityLibrary.BudgetPriorities.ForEach(
                _ =>
                {
                    _.ToSimulationAnalysisDomain(analysisMethod, simulation.InvestmentPlan);
                });

            entity.Simulation.TargetConditionGoalLibrarySimulationJoin?.TargetConditionGoalLibrary.TargetConditionGoals.ForEach(
                _ =>
                {
                    _.ToSimulationAnalysisDomain(analysisMethod);
                });

            entity.Simulation.DeficientConditionGoalLibrarySimulationJoin?.DeficientConditionGoalLibrary.DeficientConditionGoals.ForEach(
                _ =>
                {
                    _.ToSimulationAnalysisDomain(analysisMethod);
                });

            return analysisMethod;
        }
    }
}
