using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.Domains;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class TargetConditionGoalRepository : ITargetConditionGoalRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfWork;

        public TargetConditionGoalRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public void CreateTargetConditionGoalLibrary(string name, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var targetConditionGoalLibraryEntity = new TargetConditionGoalLibraryEntity { Id = Guid.NewGuid(), Name = name };

            _unitOfWork.Context.AddEntity(targetConditionGoalLibraryEntity);

            _unitOfWork.Context.AddEntity(new TargetConditionGoalLibrarySimulationEntity
            {
                TargetConditionGoalLibraryId = targetConditionGoalLibraryEntity.Id,
                SimulationId = simulationId
            });
        }

        public void CreateTargetConditionGoals(List<TargetConditionGoal> targetConditionGoals, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var simulationEntity = _unitOfWork.Context.Simulation
                .Include(_ => _.TargetConditionGoalLibrarySimulationJoin)
                .Single(_ => _.Id == simulationId);

            var attributeEntities = _unitOfWork.Context.Attribute.ToList();
            var attributeNames = attributeEntities.Select(_ => _.Name).ToList();
            if (!targetConditionGoals.All(_ => attributeNames.Contains(_.Attribute.Name)))
            {
                var missingAttributes =
                    targetConditionGoals.Select(_ => _.Attribute.Name).Except(attributeNames).ToList();
                if (missingAttributes.Count == 1)
                {
                    throw new RowNotInTableException($"No attribute found having name {missingAttributes[0]}.");
                }

                throw new RowNotInTableException(
                    $"No attributes found having the names: {string.Join(", ", missingAttributes)}.");
            }

            var targetConditionGoalEntities = targetConditionGoals
                .Select(_ => _.ToEntity(simulationEntity.TargetConditionGoalLibrarySimulationJoin.TargetConditionGoalLibraryId,
                    attributeEntities.Single(__ => __.Name == _.Attribute.Name).Id))
                .ToList();

            _unitOfWork.Context.AddAll(targetConditionGoalEntities);

            if (targetConditionGoals.Any(_ => !_.Criterion.ExpressionIsBlank))
            {
                var targetIdsPerExpression = targetConditionGoals
                    .Where(_ => !_.Criterion.ExpressionIsBlank)
                    .GroupBy(_ => _.Criterion.Expression, _ => _)
                    .ToDictionary(_ => _.Key, _ => _.Select(__ => __.Id).ToList());

                _unitOfWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(targetIdsPerExpression,
                    DataPersistenceConstants.CriterionLibraryJoinEntities.TargetConditionGoal, simulationEntity.Name);
            }
        }

        public Task<List<TargetConditionGoalLibraryDTO>> TargetConditionGoalLibrariesWithTargetConditionGoals()
        {
            if (!_unitOfWork.Context.TargetConditionGoalLibrary.Any())
            {
                return Task.Factory.StartNew(() => new List<TargetConditionGoalLibraryDTO>());
            }

            return Task.Factory.StartNew(() => _unitOfWork.Context.TargetConditionGoalLibrary
                .Include(_ => _.TargetConditionGoals)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.TargetConditionGoals)
                .ThenInclude(_ => _.CriterionLibraryTargetConditionGoalJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.TargetConditionGoalLibrarySimulationJoins)
                .Select(_ => _.ToDto())
                .ToList());
        }

        public void UpsertPermitted(UserInfoDTO userInfo, Guid simulationId, TargetConditionGoalLibraryDTO dto)
        {
            if (simulationId != Guid.Empty)
            {
                if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
                {
                    throw new RowNotInTableException($"No simulation found having id {dto.Id}");
                }

                if (!_unitOfWork.Context.Simulation.Any(_ =>
                    _.Id == dto.Id && _.SimulationUserJoins.Any(__ => __.User.Username == userInfo.Sub && __.CanModify)))
                {
                    throw new UnauthorizedAccessException("You are not authorized to modify this simulation.");
                }
            }

            UpsertTargetConditionGoalLibrary(dto, simulationId, userInfo);
            UpsertOrDeleteTargetConditionGoals(dto.TargetConditionGoals, dto.Id, userInfo);
        }

        public void UpsertTargetConditionGoalLibrary(TargetConditionGoalLibraryDTO dto, Guid simulationId, UserInfoDTO userInfo)
        {
            var userEntity = _unitOfWork.Context.User.SingleOrDefault(_ => _.Username == userInfo.Sub);

            var targetConditionGoalLibraryEntity = dto.ToEntity();

            _unitOfWork.Context.Upsert(targetConditionGoalLibraryEntity, dto.Id, userEntity?.Id);

            if (simulationId != Guid.Empty)
            {
                if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
                {
                    throw new RowNotInTableException($"No simulation found having id {simulationId}.");
                }

                _unitOfWork.Context.DeleteEntity<TargetConditionGoalLibrarySimulationEntity>(_ =>
                    _.SimulationId == simulationId);

                _unitOfWork.Context.AddEntity(
                    new TargetConditionGoalLibrarySimulationEntity
                    {
                        TargetConditionGoalLibraryId = dto.Id,
                        SimulationId = simulationId
                    }, userEntity?.Id);
            }
        }

        public void UpsertOrDeleteTargetConditionGoals(List<TargetConditionGoalDTO> targetConditionGoals,
            Guid libraryId, UserInfoDTO userInfo)
        {
            if (!_unitOfWork.Context.TargetConditionGoalLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException($"No target condition goal library found having id {libraryId}.");
            }

            if (targetConditionGoals.Any(_ => string.IsNullOrEmpty(_.Attribute)))
            {
                throw new InvalidOperationException("All target condition goals must have an attribute.");
            }

            var userEntity = _unitOfWork.Context.User.SingleOrDefault(_ => _.Username == userInfo.Sub);

            var attributeEntities = _unitOfWork.Context.Attribute.ToList();
            var attributeNames = attributeEntities.Select(_ => _.Name).ToList();
            if (!targetConditionGoals.All(_ => attributeNames.Contains(_.Attribute)))
            {
                var missingAttributes = targetConditionGoals.Select(_ => _.Attribute).Except(attributeNames).ToList();
                if (missingAttributes.Count == 1)
                {
                    throw new RowNotInTableException($"No attribute found having name {missingAttributes[0]}.");
                }

                throw new RowNotInTableException(
                    $"No attributes found having the names: {string.Join(", ", missingAttributes)}.");
            }

            var targetConditionGoalEntities = targetConditionGoals
                .Select(_ => _.ToEntity(libraryId, attributeEntities.Single(__ => __.Name == _.Attribute).Id)).ToList();

            var entityIds = targetConditionGoalEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.TargetConditionGoal
                .Where(_ => _.TargetConditionGoalLibraryId == libraryId && entityIds.Contains(_.Id)).Select(_ => _.Id)
                .ToList();

            var predicatesPerCrudOperation = new Dictionary<string, Expression<Func<TargetConditionGoalEntity, bool>>>
            {
                {"delete", _ => _.TargetConditionGoalLibraryId == libraryId && !entityIds.Contains(_.Id)},
                {"update", _ => existingEntityIds.Contains(_.Id)},
                {"add", _ => !existingEntityIds.Contains(_.Id)}
            };

            if (IsRunningFromXUnit)
            {
                _unitOfWork.Context.UpsertOrDelete(targetConditionGoalEntities, predicatesPerCrudOperation, userEntity?.Id);
            }
            else
            {
                _unitOfWork.Context.BulkUpsertOrDelete(targetConditionGoalEntities, predicatesPerCrudOperation, userEntity?.Id);
            }

            _unitOfWork.Context.DeleteAll<CriterionLibraryTargetConditionGoalEntity>(_ =>
                _.TargetConditionGoal.TargetConditionGoalLibraryId == libraryId);

            if (targetConditionGoals.Any(_ =>
                _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
            {
                var criterionLibraryJoinsToAdd = targetConditionGoals
                    .Where(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)).Select(_ =>
                        new CriterionLibraryTargetConditionGoalEntity
                        {
                            CriterionLibraryId = _.CriterionLibrary.Id,
                            TargetConditionGoalId = _.Id
                        }).ToList();

                _unitOfWork.Context.AddAll(criterionLibraryJoinsToAdd, userEntity?.Id);
            }
        }

        public void DeleteTargetConditionGoalLibrary(Guid libraryId)
        {
            if (!_unitOfWork.Context.TargetConditionGoalLibrary.Any(_ => _.Id == libraryId))
            {
                return;
            }

            _unitOfWork.Context.DeleteEntity<TargetConditionGoalLibraryEntity>(_ => _.Id == libraryId);
        }
    }
}
