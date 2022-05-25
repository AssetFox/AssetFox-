﻿using System;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class AnalysisMethodMapper
    {
        public static AnalysisMethodEntity ToEntity(this AnalysisMethod domain, Guid simulationId, Guid? attributeId) =>
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

        public static void FillSimulationAnalysisMethod(this AnalysisMethodEntity entity, Simulation simulation, string userCriteria)
        {
            simulation.AnalysisMethod.Id = entity.Id;
            simulation.AnalysisMethod.Description = entity.Description;
            simulation.AnalysisMethod.OptimizationStrategy = entity.OptimizationStrategy;
            simulation.AnalysisMethod.SpendingStrategy = entity.SpendingStrategy;
            simulation.AnalysisMethod.ShouldApplyMultipleFeasibleCosts = entity.ShouldApplyMultipleFeasibleCosts;
            simulation.AnalysisMethod.ShouldDeteriorateDuringCashFlow = entity.ShouldDeteriorateDuringCashFlow;
            simulation.AnalysisMethod.ShouldUseExtraFundsAcrossBudgets = entity.ShouldUseExtraFundsAcrossBudgets;

            var specifiedFilter = entity.CriterionLibraryAnalysisMethodJoin?.CriterionLibrary.MergedCriteriaExpression ?? string.Empty;
            simulation.AnalysisMethod.Filter.Expression =
                userCriteria == null ? specifiedFilter :
                $"({userCriteria}) AND ({specifiedFilter})";

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

            entity.Simulation.BudgetPriorities
                .ForEach(_ => _.CreateBudgetPriority(simulation));

            entity.Simulation.ScenarioTargetConditionalGoals
                .ForEach(_ => _.CreateTargetConditionGoal(simulation));

            entity.Simulation.ScenarioDeficientConditionGoals
                .ForEach(_ => _.CreateDeficientConditionGoal(simulation));

            entity.Simulation.RemainingLifeLimits
                .ForEach(_ => _.CreateRemainingLifeLimit(simulation));
        }

        public static AnalysisMethodEntity ToEntity(this AnalysisMethodDTO dto, Guid simulationId, Guid? attributeId = null) =>
            new AnalysisMethodEntity
            {
                Id = dto.Id,
                SimulationId = simulationId,
                Description = dto.Description,
                OptimizationStrategy = dto.OptimizationStrategy,
                SpendingStrategy = dto.SpendingStrategy,
                ShouldApplyMultipleFeasibleCosts = dto.ShouldApplyMultipleFeasibleCosts,
                ShouldDeteriorateDuringCashFlow = dto.ShouldDeteriorateDuringCashFlow,
                ShouldUseExtraFundsAcrossBudgets = dto.ShouldUseExtraFundsAcrossBudgets,
                AttributeId = attributeId,
            };

        public static AnalysisMethodDTO ToDto(this AnalysisMethodEntity entity) =>
            new AnalysisMethodDTO
            {
                Id = entity.Id,
                Description = entity.Description,
                OptimizationStrategy = entity.OptimizationStrategy,
                SpendingStrategy = entity.SpendingStrategy,
                ShouldApplyMultipleFeasibleCosts = entity.ShouldApplyMultipleFeasibleCosts,
                ShouldDeteriorateDuringCashFlow = entity.ShouldDeteriorateDuringCashFlow,
                ShouldUseExtraFundsAcrossBudgets = entity.ShouldUseExtraFundsAcrossBudgets,
                Attribute = entity.Attribute?.Name ?? string.Empty,
                Benefit = entity.Benefit?.ToDto() ?? new BenefitDTO(),
                CriterionLibrary = entity.CriterionLibraryAnalysisMethodJoin != null
                    ? entity.CriterionLibraryAnalysisMethodJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO()
            };
    }
}
