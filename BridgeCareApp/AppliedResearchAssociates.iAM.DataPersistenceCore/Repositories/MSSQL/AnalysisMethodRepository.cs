using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAccess;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AnalysisMethodRepository : IAnalysisMethodRepository
    {
        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfWork;

        public AnalysisMethodRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ??
                                         throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateAnalysisMethod(AnalysisMethod analysisMethod, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            if (analysisMethod.Weighting != null && !string.IsNullOrEmpty(analysisMethod.Weighting.Name) &&
                !_unitOfWork.Context.Attribute.Any(_ => _.Name == analysisMethod.Weighting.Name))
            {
                throw new RowNotInTableException($"No attribute found having name {analysisMethod.Weighting.Name}");
            }

            var simulationEntity = _unitOfWork.Context.Simulation.Single(_ => _.Id == simulationId);
            var attributeEntity = _unitOfWork.Context.Attribute.SingleOrDefault(_ => _.Name == analysisMethod.Weighting.Name);

            var analysisMethodEntity = analysisMethod.ToEntity(simulationEntity.Id, attributeEntity?.Id);
            _unitOfWork.Context.AddEntity(analysisMethodEntity);

            _unitOfWork.BenefitRepo.CreateBenefit(analysisMethod.Benefit, analysisMethodEntity.Id);

            if (!analysisMethod.Filter.ExpressionIsBlank)
            {
                var analysisMethodEntityIdPerExpression = new Dictionary<string, List<Guid>>
                {
                    {analysisMethod.Filter.Expression, new List<Guid> {analysisMethodEntity.Id}}
                };
                _unitOfWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(analysisMethodEntityIdPerExpression,
                    DataPersistenceConstants.CriterionLibraryJoinEntities.AnalysisMethod, simulationEntity.Name);
            }

            if (analysisMethod.BudgetPriorities.Any())
            {
                CreateAnalysisMethodBudgetPriorities(simulationEntity, analysisMethod.BudgetPriorities.ToList());
            }

            if (analysisMethod.TargetConditionGoals.Any())
            {
                CreateAnalysisMethodTargetConditionGoals(simulationEntity, analysisMethod.TargetConditionGoals.ToList());
            }

            if (analysisMethod.DeficientConditionGoals.Any())
            {
                CreateAnalysisMethodDeficientConditionGoals(simulationEntity, analysisMethod.DeficientConditionGoals.ToList());
            }

            if (analysisMethod.RemainingLifeLimits.Any())
            {
                CreateAnalysisMethodRemainingLifeLimits(simulationEntity, analysisMethod.RemainingLifeLimits.ToList());
            }
        }

        private void CreateAnalysisMethodBudgetPriorities(SimulationEntity simulationEntity, List<BudgetPriority> budgetPriorities)
        {
            _unitOfWork.BudgetPriorityRepo.CreateBudgetPriorityLibrary(
                $"{simulationEntity.Name} Budget Priority Library", simulationEntity.Id);

            _unitOfWork.BudgetPriorityRepo.CreateBudgetPriorities(budgetPriorities, simulationEntity.Id);
        }

        private void CreateAnalysisMethodTargetConditionGoals(SimulationEntity simulationEntity, List<TargetConditionGoal> targetConditionGoals)
        {
            _unitOfWork.TargetConditionGoalRepo.CreateTargetConditionGoalLibrary(
                $"{simulationEntity.Name} Target Condition Goal Library", simulationEntity.Id);

            _unitOfWork.TargetConditionGoalRepo.CreateTargetConditionGoals(targetConditionGoals, simulationEntity.Id);
        }

        private void CreateAnalysisMethodDeficientConditionGoals(SimulationEntity simulationEntity, List<DeficientConditionGoal> deficientConditionGoals)
        {
            _unitOfWork.DeficientConditionGoalRepo.CreateDeficientConditionGoalLibrary(
                $"{simulationEntity.Name} Deficient Condition Goal Library", simulationEntity.Id);

            _unitOfWork.DeficientConditionGoalRepo.CreateDeficientConditionGoals(deficientConditionGoals, simulationEntity.Id);
        }

        private void CreateAnalysisMethodRemainingLifeLimits(SimulationEntity simulationEntity,
            List<RemainingLifeLimit> remainingLifeLimits)
        {
            _unitOfWork.RemainingLifeLimitRepo.CreateRemainingLifeLimitLibrary(
                $"{simulationEntity.Name} Remaining Life Limit Library", simulationEntity.Id);

            _unitOfWork.RemainingLifeLimitRepo.CreateRemainingLifeLimits(remainingLifeLimits, simulationEntity.Id);
        }

        public void GetSimulationAnalysisMethod(Simulation simulation)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulation.Id))
            {
                throw new RowNotInTableException($"No simulation found having id {simulation.Id}");
            }

            _unitOfWork.Context.AnalysisMethod
                .Include(_ => _.Attribute)
                .Include(_ => _.Benefit)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.CriterionLibraryAnalysisMethodJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.BudgetPriorityLibrarySimulationJoin)
                .ThenInclude(_ => _.BudgetPriorityLibrary)
                .ThenInclude(_ => _.BudgetPriorities)
                .ThenInclude(_ => _.CriterionLibraryBudgetPriorityJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.BudgetPriorityLibrarySimulationJoin)
                .ThenInclude(_ => _.BudgetPriorityLibrary)
                .ThenInclude(_ => _.BudgetPriorities)
                .ThenInclude(_ => _.BudgetPercentagePairs)

                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.ScenarioTargetConditionalGoals)
                .ThenInclude(_ => _.CriterionLibraryScenarioTargetConditionGoalJoin)
                .ThenInclude(_ => _.CriterionLibrary)

                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.DeficientConditionGoalLibrarySimulationJoin)
                .ThenInclude(_ => _.DeficientConditionGoalLibrary)
                .ThenInclude(_ => _.DeficientConditionGoals)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.DeficientConditionGoalLibrarySimulationJoin)
                .ThenInclude(_ => _.DeficientConditionGoalLibrary)
                .ThenInclude(_ => _.DeficientConditionGoals)
                .ThenInclude(_ => _.CriterionLibraryDeficientConditionGoalJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.RemainingLifeLimitLibrarySimulationJoin)
                .ThenInclude(_ => _.RemainingLifeLimitLibrary)
                .ThenInclude(_ => _.RemainingLifeLimits)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.RemainingLifeLimitLibrarySimulationJoin)
                .ThenInclude(_ => _.RemainingLifeLimitLibrary)
                .ThenInclude(_ => _.RemainingLifeLimits)
                .ThenInclude(_ => _.CriterionLibraryRemainingLifeLimitJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .AsNoTracking()
                .Single(_ => _.Simulation.Id == simulation.Id)
                .FillSimulationAnalysisMethod(simulation);
        }

        public AnalysisMethodDTO GetAnalysisMethod(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            if (!_unitOfWork.Context.AnalysisMethod.Any(_ => _.SimulationId == simulationId))
            {
                return new AnalysisMethodDTO
                {
                    Id = Guid.NewGuid(),
                    OptimizationStrategy = OptimizationStrategy.Benefit,
                    SpendingStrategy = SpendingStrategy.NoSpending,
                    ShouldApplyMultipleFeasibleCosts = false,
                    ShouldDeteriorateDuringCashFlow = false,
                    ShouldUseExtraFundsAcrossBudgets = false,
                    Benefit = new BenefitDTO(),
                    CriterionLibrary = new CriterionLibraryDTO()
                };
            }

            return _unitOfWork.Context.AnalysisMethod
                .Include(_ => _.Attribute)
                .Include(_ => _.Benefit)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.CriterionLibraryAnalysisMethodJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Single(_ => _.SimulationId == simulationId)
                .ToDto();
        }

        public void UpsertAnalysisMethod(Guid simulationId, AnalysisMethodDTO dto)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            AttributeEntity attributeEntity = null;

            if (!string.IsNullOrEmpty(dto.Attribute))
            {
                if (!_unitOfWork.Context.Attribute.Any(_ => _.Name == dto.Attribute))
                {
                    throw new RowNotInTableException($"No attribute found having name {dto.Attribute}.");
                }

                attributeEntity = _unitOfWork.Context.Attribute.Single(_ => _.Name == dto.Attribute);
            }

            var analysisMethodEntity = dto.ToEntity(simulationId, attributeEntity?.Id);

            _unitOfWork.Context.Upsert(analysisMethodEntity, _ => _.Id == dto.Id, _unitOfWork.UserEntity?.Id);

            // Update last modified date
            var simulationEntity = _unitOfWork.Context.Simulation.Where(_ => _.Id == simulationId).FirstOrDefault();
            _unitOfWork.SimulationRepo.UpdateLastModifiedDate(simulationEntity);

            _unitOfWork.BenefitRepo.UpsertBenefit(dto.Benefit, dto.Id);

            _unitOfWork.Context.DeleteEntity<CriterionLibraryAnalysisMethodEntity>(_ => _.AnalysisMethodId == dto.Id);

            if (dto.CriterionLibrary?.Id != null && dto.CriterionLibrary?.Id != Guid.Empty &&
                !string.IsNullOrEmpty(dto.CriterionLibrary.MergedCriteriaExpression))
            {
                var criterionJoinEntity = new CriterionLibraryAnalysisMethodEntity
                {
                    CriterionLibraryId = dto.CriterionLibrary.Id,
                    AnalysisMethodId = dto.Id
                };

                _unitOfWork.Context.AddEntity(criterionJoinEntity, _unitOfWork.UserEntity?.Id);
            }
        }
    }
}
