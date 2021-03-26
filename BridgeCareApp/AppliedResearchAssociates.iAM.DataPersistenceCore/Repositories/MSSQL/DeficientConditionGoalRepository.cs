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
    public class DeficientConditionGoalRepository : IDeficientConditionGoalRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfWork;

        public DeficientConditionGoalRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public void CreateDeficientConditionGoalLibrary(string name, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var deficientConditionGoalLibraryEntity = new DeficientConditionGoalLibraryEntity { Id = Guid.NewGuid(), Name = name };

            _unitOfWork.Context.AddEntity(deficientConditionGoalLibraryEntity);

            _unitOfWork.Context.AddEntity(new DeficientConditionGoalLibrarySimulationEntity
            {
                DeficientConditionGoalLibraryId = deficientConditionGoalLibraryEntity.Id,
                SimulationId = simulationId
            });
        }

        public void CreateDeficientConditionGoals(List<DeficientConditionGoal> deficientConditionGoals, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var simulationEntity = _unitOfWork.Context.Simulation
                .Include(_ => _.DeficientConditionGoalLibrarySimulationJoin)
                .Single(_ => _.Id == simulationId);

            var attributeEntities = _unitOfWork.Context.Attribute.ToList();
            var attributeNames = attributeEntities.Select(_ => _.Name).ToList();
            if (!deficientConditionGoals.All(_ => attributeNames.Contains(_.Attribute.Name)))
            {
                var missingAttributes =
                    deficientConditionGoals.Select(_ => _.Attribute.Name).Except(attributeNames).ToList();
                if (missingAttributes.Count == 1)
                {
                    throw new RowNotInTableException($"No attribute found having name {missingAttributes[0]}.");
                }

                throw new RowNotInTableException(
                    $"No attributes found having the names: {string.Join(", ", missingAttributes)}.");
            }

            var deficientConditionGoalEntities = deficientConditionGoals
                .Select(_ => _.ToEntity(simulationEntity.DeficientConditionGoalLibrarySimulationJoin.DeficientConditionGoalLibraryId,
                    attributeEntities.Single(__ => __.Name == _.Attribute.Name).Id))
                .ToList();

            _unitOfWork.Context.AddAll(deficientConditionGoalEntities);

            if (deficientConditionGoals.Any(_ => !_.Criterion.ExpressionIsBlank))
            {
                var deficientIdsPerExpression = deficientConditionGoals
                    .Where(_ => !_.Criterion.ExpressionIsBlank)
                    .GroupBy(_ => _.Criterion.Expression, _ => _)
                    .ToDictionary(_ => _.Key, _ => _.Select(__ => __.Id).ToList());

                _unitOfWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(deficientIdsPerExpression,
                    DataPersistenceConstants.CriterionLibraryJoinEntities.DeficientConditionGoal, simulationEntity.Name);
            }
        }

        public Task<List<DeficientConditionGoalLibraryDTO>> DeficientConditionGoalLibrariesWithDeficientConditionGoals()
        {
            if (!_unitOfWork.Context.DeficientConditionGoalLibrary.Any())
            {
                return Task.Factory.StartNew(() => new List<DeficientConditionGoalLibraryDTO>());
            }

            return Task.Factory.StartNew(() => _unitOfWork.Context.DeficientConditionGoalLibrary
                .Include(_ => _.DeficientConditionGoals)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.DeficientConditionGoals)
                .ThenInclude(_ => _.CriterionLibraryDeficientConditionGoalJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.DeficientConditionGoalLibrarySimulationJoins)
                .Select(_ => _.ToDto())
                .ToList());
        }

        public void UpsertPermitted(UserInfoDTO userInfo, Guid simulationId, DeficientConditionGoalLibraryDTO dto)
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

            UpsertDeficientConditionGoalLibrary(dto, simulationId, userInfo);
            UpsertOrDeleteDeficientConditionGoals(dto.DeficientConditionGoals, dto.Id, userInfo);
        }

        public void UpsertDeficientConditionGoalLibrary(DeficientConditionGoalLibraryDTO dto, Guid simulationId, UserInfoDTO userInfo)
        {
            var userEntity = _unitOfWork.Context.User.SingleOrDefault(_ => _.Username == userInfo.Sub);

            var deficientConditionGoalLibraryEntity = dto.ToEntity();

            _unitOfWork.Context.Upsert(deficientConditionGoalLibraryEntity, dto.Id, userEntity?.Id);

            if (simulationId != Guid.Empty)
            {
                if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
                {
                    throw new RowNotInTableException($"No simulation found having id {simulationId}.");
                }

                _unitOfWork.Context.DeleteEntity<DeficientConditionGoalLibrarySimulationEntity>(_ => _.SimulationId == simulationId);

                _unitOfWork.Context.AddEntity(
                    new DeficientConditionGoalLibrarySimulationEntity
                    {
                        DeficientConditionGoalLibraryId = dto.Id,
                        SimulationId = simulationId
                    }, userEntity?.Id);
            }
        }

        public void UpsertOrDeleteDeficientConditionGoals(List<DeficientConditionGoalDTO> deficientConditionGoals,
            Guid libraryId, UserInfoDTO userInfo)
        {
            if (!_unitOfWork.Context.DeficientConditionGoalLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException($"No deficient condition goal library found having id {libraryId}.");
            }

            if (deficientConditionGoals.Any(_ => string.IsNullOrEmpty(_.Attribute)))
            {
                throw new InvalidOperationException("All deficient condition goals must have an attribute.");
            }

            var userEntity = _unitOfWork.Context.User.SingleOrDefault(_ => _.Username == userInfo.Sub);

            var attributeEntities = _unitOfWork.Context.Attribute.ToList();
            var attributeNames = attributeEntities.Select(_ => _.Name).ToList();
            if (!deficientConditionGoals.All(_ => attributeNames.Contains(_.Attribute)))
            {
                var missingAttributes =
                    deficientConditionGoals.Select(_ => _.Attribute).Except(attributeNames).ToList();
                if (missingAttributes.Count == 1)
                {
                    throw new RowNotInTableException($"No attribute found having name {missingAttributes[0]}.");
                }

                throw new RowNotInTableException(
                    $"No attributes found having the names: {string.Join(", ", missingAttributes)}.");
            }

            var deficientConditionGoalEntities = deficientConditionGoals
                .Select(_ => _.ToEntity(libraryId, attributeEntities.Single(__ => __.Name == _.Attribute).Id)).ToList();

            var entityIds = deficientConditionGoalEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.DeficientConditionGoal
                .Where(_ => _.DeficientConditionGoalLibraryId == libraryId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            var predicatesPerCrudOperation =
                new Dictionary<string, Expression<Func<DeficientConditionGoalEntity, bool>>>
                {
                    {"delete", _ => _.DeficientConditionGoalLibraryId == libraryId && !entityIds.Contains(_.Id)},
                    {"update", _ => existingEntityIds.Contains(_.Id)},
                    {"add", _ => !existingEntityIds.Contains(_.Id)}
                };

            if (IsRunningFromXUnit)
            {
                _unitOfWork.Context.UpsertOrDelete(deficientConditionGoalEntities, predicatesPerCrudOperation, userEntity?.Id);
            }
            else
            {
                _unitOfWork.Context.BulkUpsertOrDelete(deficientConditionGoalEntities, predicatesPerCrudOperation, userEntity?.Id);
            }

            _unitOfWork.Context.DeleteAll<CriterionLibraryDeficientConditionGoalEntity>(_ =>
                _.DeficientConditionGoal.DeficientConditionGoalLibraryId == libraryId);

            if (deficientConditionGoals.Any((_ =>
                _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression))))
            {
                var criterionLibraryJoinsToAdd = deficientConditionGoals
                    .Where(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)).Select(_ =>
                        new CriterionLibraryDeficientConditionGoalEntity
                        {
                            CriterionLibraryId = _.CriterionLibrary.Id,
                            DeficientConditionGoalId = _.Id
                        }).ToList();

                _unitOfWork.Context.AddAll(criterionLibraryJoinsToAdd, userEntity?.Id);
            }
        }

        public void DeleteDeficientConditionGoalLibrary(Guid libraryId)
        {
            if (!_unitOfWork.Context.DeficientConditionGoalLibrary.Any(_ => _.Id == libraryId))
            {
                return;
            }

            _unitOfWork.Context.DeleteEntity<DeficientConditionGoalLibraryEntity>(_ => _.Id == libraryId);
        }
    }
}
