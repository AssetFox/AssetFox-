using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAccess;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AnalysisMethodRepository : IAnalysisMethodRepository
    {
        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public AnalysisMethodRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfDataPersistenceWork)
        {
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
        }

        public void CreateAnalysisMethod(AnalysisMethod analysisMethod, Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            if (!_unitOfDataPersistenceWork.Context.Attribute.Any(_ => _.Name == analysisMethod.Weighting.Name))
            {
                throw new RowNotInTableException($"No attribute found having name {analysisMethod.Weighting.Name}");
            }

            var simulationEntity = _unitOfDataPersistenceWork.Context.Simulation.Single(_ => _.Id == simulationId);
            var attributeEntity = _unitOfDataPersistenceWork.Context.Attribute.Single(_ => _.Name == analysisMethod.Weighting.Name);

            var analysisMethodEntity = analysisMethod.ToEntity(simulationEntity.Id, attributeEntity.Id);
            _unitOfDataPersistenceWork.Context.AnalysisMethod.Add(analysisMethodEntity);
            _unitOfDataPersistenceWork.Context.SaveChanges();

            _unitOfDataPersistenceWork.BenefitRepo.CreateBenefit(analysisMethod.Benefit, analysisMethodEntity.Id);

            if (!analysisMethod.Filter.ExpressionIsBlank)
            {
                var analysisMethodEntityIdPerExpression = new Dictionary<string, List<Guid>>
                        {
                            {analysisMethod.Filter.Expression, new List<Guid> {analysisMethodEntity.Id}}
                        };
                _unitOfDataPersistenceWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(analysisMethodEntityIdPerExpression,
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
            _unitOfDataPersistenceWork.BudgetPriorityRepo.CreateBudgetPriorityLibrary($"{simulationEntity.Name} Simulation Budget Priority Library", simulationEntity.Id);

            _unitOfDataPersistenceWork.BudgetPriorityRepo.CreateBudgetPriorities(budgetPriorities, simulationEntity.Id);
        }

        private void CreateAnalysisMethodTargetConditionGoals(SimulationEntity simulationEntity, List<TargetConditionGoal> targetConditionGoals)
        {
            _unitOfDataPersistenceWork.TargetConditionGoalRepo.CreateTargetConditionGoalLibrary($"{simulationEntity.Name} Simulation Target Condition Goal Library", simulationEntity.Id);

            _unitOfDataPersistenceWork.TargetConditionGoalRepo.CreateTargetConditionGoals(targetConditionGoals, simulationEntity.Id);
        }

        private void CreateAnalysisMethodDeficientConditionGoals(SimulationEntity simulationEntity, List<DeficientConditionGoal> deficientConditionGoals)
        {
            _unitOfDataPersistenceWork.DeficientConditionGoalRepo.CreateDeficientConditionGoalLibrary($"{simulationEntity.Name} Simulation Deficient Condition Goal Library", simulationEntity.Id);

            _unitOfDataPersistenceWork.DeficientConditionGoalRepo.CreateDeficientConditionGoals(deficientConditionGoals, simulationEntity.Id);
        }

        private void CreateAnalysisMethodRemainingLifeLimits(SimulationEntity simulationEntity,
            List<RemainingLifeLimit> remainingLifeLimits)
        {
            _unitOfDataPersistenceWork.RemainingLifeLimitRepo.CreateRemainingLifeLimitLibrary($"{simulationEntity.Name} Simulation Remaining Life Limit Library", simulationEntity.Id);

            _unitOfDataPersistenceWork.RemainingLifeLimitRepo.CreateRemainingLifeLimits(remainingLifeLimits, simulationEntity.Id);
        }

        public void GetSimulationAnalysisMethod(Simulation simulation)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulation.Id))
            {
                throw new RowNotInTableException($"No simulation found having id {simulation.Id}");
            }

            _unitOfDataPersistenceWork.Context.AnalysisMethod.Include(_ => _.Attribute)
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
                .ThenInclude(_ => _.TargetConditionGoalLibrarySimulationJoin)
                .ThenInclude(_ => _.TargetConditionGoalLibrary)
                .ThenInclude(_ => _.TargetConditionGoals)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.TargetConditionGoalLibrarySimulationJoin)
                .ThenInclude(_ => _.TargetConditionGoalLibrary)
                .ThenInclude(_ => _.TargetConditionGoals)
                .ThenInclude(_ => _.CriterionLibraryTargetConditionGoalJoin)
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
                .Single(_ => _.Simulation.Id == simulation.Id)
                .FillSimulationAnalysisMethod(simulation);
        }

        public AnalysisMethodDTO GetPermittedAnalysisMethod(UserInfoDTO userInfo, Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ =>
                _.Id == simulationId && _.SimulationUserJoins.Any(__ => __.User.Username == userInfo.Sub)))
            {
                throw new UnauthorizedAccessException("You are not authorized to view this simulation analysis method.");
            }

            return GetAnalysisMethod(simulationId);
        }

        public AnalysisMethodDTO GetAnalysisMethod(Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            if (!_unitOfDataPersistenceWork.Context.AnalysisMethod.Any(_ => _.SimulationId == simulationId))
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

            return _unitOfDataPersistenceWork.Context.AnalysisMethod
                .Include(_ => _.Attribute)
                .Include(_ => _.Benefit)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.CriterionLibraryAnalysisMethodJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Single(_ => _.SimulationId == simulationId)
                .ToDto();
        }

        public void UpsertPermittedAnalysisMethod(UserInfoDTO userInfo, Guid simulationId,
            AnalysisMethodDTO dto)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ =>
                _.Id == simulationId && _.SimulationUserJoins.Any(__ => __.User.Username == userInfo.Sub && __.CanModify)))
            {
                throw new UnauthorizedAccessException("You are not authorized to modify this simulation analysis method.");
            }

            UpsertAnalysisMethod(simulationId, dto, userInfo);
        }

        public void UpsertAnalysisMethod(Guid simulationId, AnalysisMethodDTO dto, UserInfoDTO userInfo)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            AttributeEntity attributeEntity = null;

            if (!string.IsNullOrEmpty(dto.Attribute))
            {
                if (!_unitOfDataPersistenceWork.Context.Attribute.Any(_ => _.Name == dto.Attribute))
                {
                    throw new RowNotInTableException($"No attribute found having name {dto.Attribute}.");
                }

                attributeEntity = _unitOfDataPersistenceWork.Context.Attribute.Single(_ => _.Name == dto.Attribute);
            }

            var userEntity = _unitOfDataPersistenceWork.Context.User.SingleOrDefault(_ => _.Username == userInfo.Sub);

            var analysisMethodEntity = dto.ToEntity(simulationId, attributeEntity?.Id);

            _unitOfDataPersistenceWork.Context.Upsert(analysisMethodEntity, _ => _.Id == dto.Id, userEntity?.Id);

            if (dto.Benefit.Id != Guid.Empty)
            {
                _unitOfDataPersistenceWork.BenefitRepo.UpsertBenefit(dto.Benefit, dto.Id, userEntity?.Id);
            }

            _unitOfDataPersistenceWork.Context.Delete<CriterionLibraryAnalysisMethodEntity>(_ => _.AnalysisMethodId == dto.Id);

            if (dto.CriterionLibrary?.Id != null && dto.CriterionLibrary?.Id != Guid.Empty &&
                !string.IsNullOrEmpty(dto.CriterionLibrary.MergedCriteriaExpression))
            {
                var criterionJoinEntity = new CriterionLibraryAnalysisMethodEntity
                {
                    CriterionLibraryId = dto.CriterionLibrary.Id,
                    AnalysisMethodId = dto.Id
                };

                _unitOfDataPersistenceWork.Context.AddEntity(criterionJoinEntity, userEntity?.Id);
            }
        }
    }
}
