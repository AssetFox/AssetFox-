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
                Id = domain.Id,
                SimulationId = simulationId,
                AttributeId = attributeId,
                Description = domain.Description,
                OptimizationStrategy = domain.OptimizationStrategy,
                SpendingStrategy = domain.SpendingStrategy,
                ShouldApplyMultipleFeasibleCosts = domain.ShouldApplyMultipleFeasibleCosts,
                ShouldDeteriorateDuringCashFlow = domain.ShouldDeteriorateDuringCashFlow,
                ShouldUseExtraFundsAcrossBudgets = domain.ShouldUseExtraFundsAcrossBudgets
            };

        public static void FillSimulationAnalysisMethod(this AnalysisMethodEntity entity, Simulation simulation)
        {
            simulation.AnalysisMethod.Id = entity.Id;
            simulation.AnalysisMethod.Description = entity.Description;
            simulation.AnalysisMethod.OptimizationStrategy = entity.OptimizationStrategy;
            simulation.AnalysisMethod.SpendingStrategy = entity.SpendingStrategy;
            simulation.AnalysisMethod.ShouldApplyMultipleFeasibleCosts = entity.ShouldApplyMultipleFeasibleCosts;
            simulation.AnalysisMethod.ShouldDeteriorateDuringCashFlow = entity.ShouldDeteriorateDuringCashFlow;
            simulation.AnalysisMethod.ShouldUseExtraFundsAcrossBudgets = entity.ShouldUseExtraFundsAcrossBudgets;
            simulation.AnalysisMethod.Filter.Expression =
                entity.CriterionLibraryAnalysisMethodJoin?.CriterionLibrary.MergedCriteriaExpression ?? string.Empty;

            if (entity.Attribute != null)
            {
                simulation.AnalysisMethod.Weighting = simulation.Network.Explorer.NumberAttributes
                    .Single(_ => _.Name == entity.Attribute.Name);
            }

            if (entity.Benefit != null)
            {
                simulation.AnalysisMethod.Benefit.Id = entity.Benefit.Id;
                simulation.AnalysisMethod.Benefit.Limit = entity.Benefit.Limit;
                if (entity.Benefit.Attribute != null)
                {
                    simulation.AnalysisMethod.Benefit.Attribute = simulation.Network.Explorer.NumericAttributes
                        .Single(_ => _.Name == entity.Benefit.Attribute.Name);
                }
            }

            entity.Simulation.BudgetPriorityLibrarySimulationJoin?.BudgetPriorityLibrary.BudgetPriorities
                .ForEach(_ => _.CreateBudgetPriority(simulation));

            entity.Simulation.TargetConditionGoalLibrarySimulationJoin?.TargetConditionGoalLibrary.TargetConditionGoals
                .ForEach(_ => _.CreateTargetConditionGoal(simulation));

            entity.Simulation.DeficientConditionGoalLibrarySimulationJoin?.DeficientConditionGoalLibrary.DeficientConditionGoals
                .ForEach(_ => _.CreateDeficientConditionGoal(simulation));

            entity.Simulation.RemainingLifeLimitLibrarySimulationJoin?.RemainingLifeLimitLibrary.RemainingLifeLimits
                .ForEach(_ => _.CreateRemainingLifeLimit(simulation));
        }
    }
}
