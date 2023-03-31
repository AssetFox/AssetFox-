using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.TargetConditionGoal;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.TargetConditionGoal;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class TargetConditionGoalRepository : ITargetConditionGoalRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public TargetConditionGoalRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateTargetConditionGoals(List<TargetConditionGoal> targetConditionGoals, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation found for given scenario.");
            }

            var simulationEntity = _unitOfWork.Context.Simulation
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
                .Select(_ => _.ToScenarioEntity(simulationId,
                    attributeEntities.Single(__ => __.Name == _.Attribute.Name).Id))
                .ToList();

            _unitOfWork.Context.AddAll(targetConditionGoalEntities, _unitOfWork.UserEntity?.Id);

            if (targetConditionGoals.Any(_ => !_.Criterion.ExpressionIsBlank))
            {
                var criterionLibraryEntities = new List<CriterionLibraryEntity>();
                var criterionLibraryJoinEntities = new List<CriterionLibraryScenarioTargetConditionGoalEntity>();

                targetConditionGoals.Where(goal => !goal.Criterion.ExpressionIsBlank)
                    .ForEach(goal =>
                    {
                        var criterionLibraryEntity = new CriterionLibraryEntity
                        {
                            Id = Guid.NewGuid(),
                            MergedCriteriaExpression = goal.Criterion.Expression,
                            Name = $"{goal.Name} {goal.Attribute} Criterion",
                            IsSingleUse = true
                        };
                        criterionLibraryEntities.Add(criterionLibraryEntity);
                        criterionLibraryJoinEntities.Add(new CriterionLibraryScenarioTargetConditionGoalEntity
                        {
                            CriterionLibraryId = criterionLibraryEntity.Id,
                            ScenarioTargetConditionGoalId = goal.Id
                        });
                    });

                _unitOfWork.Context.AddAll(criterionLibraryEntities, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criterionLibraryJoinEntities, _unitOfWork.UserEntity?.Id);
            }
            // Update last modified date
            _unitOfWork.SimulationRepo.UpdateLastModifiedDate(simulationEntity);
        }

        public List<TargetConditionGoalLibraryDTO> GetTargetConditionGoalLibrariesWithTargetConditionGoals()
        {
            if (!_unitOfWork.Context.TargetConditionGoalLibrary.Any())
            {
                return new List<TargetConditionGoalLibraryDTO>();
            }

            return _unitOfWork.Context.TargetConditionGoalLibrary.AsNoTracking()
                .Include(_ => _.TargetConditionGoals)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.TargetConditionGoals)
                .ThenInclude(_ => _.CriterionLibraryTargetConditionGoalJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Select(_ => _.ToDto())
                .ToList();
        }

        public List<TargetConditionGoalLibraryDTO> GetTargetConditionGoalLibrariesNoChildren()
        {
            if (!_unitOfWork.Context.TargetConditionGoalLibrary.Any())
            {
                return new List<TargetConditionGoalLibraryDTO>();
            }

            return _unitOfWork.Context.TargetConditionGoalLibrary.AsNoTracking()
                .Select(_ => _.ToDto())
                .ToList();
        }

        public void UpsertTargetConditionGoalLibrary(TargetConditionGoalLibraryDTO dto)
        {
            var targetConditionGoalLibraryEntity = dto.ToEntity();
            var libraryExists = _unitOfWork.Context.TargetConditionGoalLibrary.Any(t1 => t1.Id == dto.Id);
            _unitOfWork.Context.Upsert(targetConditionGoalLibraryEntity, dto.Id, _unitOfWork.UserEntity?.Id);
            _unitOfWork.Context.SaveChanges();
        }

        public void UpsertOrDeleteTargetConditionGoals(List<TargetConditionGoalDTO> targetConditionGoals,
            Guid libraryId)
        {
            if (!_unitOfWork.Context.TargetConditionGoalLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException("The specified target condition goal library was not found.");
            }

            if (targetConditionGoals.Any(_ => string.IsNullOrEmpty(_.Attribute)))
            {
                throw new InvalidOperationException("All target condition goals must have an attribute.");
            }

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
                .Select(_ => _.ToLibraryEntity(libraryId, attributeEntities.Single(__ => __.Name == _.Attribute).Id)).ToList();

            var entityIds = targetConditionGoalEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.TargetConditionGoal
                .Where(_ => _.TargetConditionGoalLibraryId == libraryId && entityIds.Contains(_.Id)).Select(_ => _.Id)
                .ToList();

            _unitOfWork.Context.DeleteAll<TargetConditionGoalEntity>(_ =>
                _.TargetConditionGoalLibraryId == libraryId && !entityIds.Contains(_.Id));

            var entitiesToUpdate = targetConditionGoalEntities.Where(_ => existingEntityIds.Contains(_.Id)).ToList();
            _unitOfWork.Context.UpdateAll(
                entitiesToUpdate, _unitOfWork.UserEntity?.Id);

            _unitOfWork.Context.AddAll(targetConditionGoalEntities.Where(_ => !existingEntityIds.Contains(_.Id))
                .ToList(), _unitOfWork.UserEntity?.Id);

            _unitOfWork.Context.DeleteAll<CriterionLibraryTargetConditionGoalEntity>(_ =>
                _.TargetConditionGoal.TargetConditionGoalLibraryId == libraryId);

            if (targetConditionGoals.Any(_ =>
                _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
            {
                var criterionLibraryEntities = new List<CriterionLibraryEntity>();
                var criterionLibraryJoinEntities = new List<CriterionLibraryTargetConditionGoalEntity>();

                targetConditionGoals.Where(curve =>
                        curve.CriterionLibrary?.Id != null && curve.CriterionLibrary?.Id != Guid.Empty &&
                        !string.IsNullOrEmpty(curve.CriterionLibrary.MergedCriteriaExpression))
                    .ForEach(goal =>
                    {
                        var criterionLibraryEntity = new CriterionLibraryEntity
                        {
                            Id = Guid.NewGuid(),
                            MergedCriteriaExpression = goal.CriterionLibrary.MergedCriteriaExpression,
                            Name = $"{goal.Name} {goal.Attribute} Criterion",
                            IsSingleUse = true
                        };
                        criterionLibraryEntities.Add(criterionLibraryEntity);
                        criterionLibraryJoinEntities.Add(new CriterionLibraryTargetConditionGoalEntity
                        {
                            CriterionLibraryId = criterionLibraryEntity.Id,
                            TargetConditionGoalId = goal.Id
                        });
                    });

                _unitOfWork.Context.AddAll(criterionLibraryEntities, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criterionLibraryJoinEntities, _unitOfWork.UserEntity?.Id);
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

        public List<TargetConditionGoalDTO> GetScenarioTargetConditionGoals(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found for the given scenario.");
            }

            var res = _unitOfWork.Context.ScenarioTargetConditionGoals.AsNoTracking()
                .Where(_ => _.SimulationId == simulationId)
                .Include(_ => _.CriterionLibraryScenarioTargetConditionGoalJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Attribute)
                .Select(_ => _.ToDto())
                .AsNoTracking()
                .ToList();
            return res;
        }

        public List<TargetConditionGoalDTO> GetTargetConditionGoalsByLibraryId(Guid libraryId)
        {
            if (!_unitOfWork.Context.TargetConditionGoalLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException($"The specified target condition goal library was not found.");
            }

            var res = _unitOfWork.Context.TargetConditionGoal.AsNoTracking()
                .Where(_ => _.TargetConditionGoalLibraryId == libraryId)
                .Include(_ => _.CriterionLibraryTargetConditionGoalJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Attribute)
                .Select(_ => _.ToDto())
                .AsNoTracking()
                .ToList();
            return res;
        }

        public void UpsertOrDeleteScenarioTargetConditionGoals(List<TargetConditionGoalDTO> scenarioTargetConditionGoal, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found for the given scenario.");
            }
            if (scenarioTargetConditionGoal.Any(_ => string.IsNullOrEmpty(_.Attribute)))
            {
                throw new InvalidOperationException("All target conditions must have an attribute.");
            }
            _unitOfWork.AsTransaction(() =>
            {
                var attributeEntities = _unitOfWork.Context.Attribute.ToList();
                var attributeNames = attributeEntities.Select(_ => _.Name).ToList();

                if (!scenarioTargetConditionGoal.All(_ => attributeNames.Contains(_.Attribute)))
                {
                    var missingAttributes = scenarioTargetConditionGoal.Select(_ => _.Attribute)
                        .Except(attributeNames).ToList();
                    if (missingAttributes.Count == 1)
                    {
                        throw new RowNotInTableException($"No attribute found having name {missingAttributes[0]}.");
                    }

                    throw new RowNotInTableException(
                        $"No attributes found having names: {string.Join(", ", missingAttributes)}.");
                }
                var scenarioTargetConditionGoalEntities = scenarioTargetConditionGoal
                    .Select(_ =>
                        _.ToScenarioEntity(simulationId, attributeEntities.Single(__ => __.Name == _.Attribute).Id))
                    .ToList();
                var entityIds = scenarioTargetConditionGoal.Select(_ => _.Id).ToList();

                var existingEntityIds = _unitOfWork.Context.ScenarioTargetConditionGoals
                    .Where(_ => _.SimulationId == simulationId && entityIds.Contains(_.Id))
                    .Select(_ => _.Id).ToList();

                _unitOfWork.Context.DeleteAll<ScenarioTargetConditionGoalEntity>(_ =>
                    _.SimulationId == simulationId && !entityIds.Contains(_.Id));

                _unitOfWork.Context.UpdateAll(scenarioTargetConditionGoalEntities.Where(_ => existingEntityIds.Contains(_.Id))
                    .ToList(), _unitOfWork.UserEntity?.Id);

                _unitOfWork.Context.AddAll(scenarioTargetConditionGoalEntities.Where(_ => !existingEntityIds.Contains(_.Id))
                    .ToList(), _unitOfWork.UserEntity?.Id);

                _unitOfWork.Context.DeleteAll<CriterionLibraryScenarioTargetConditionGoalEntity>(_ =>
                    _.ScenarioTargetConditionGoal.SimulationId == simulationId);

                if (scenarioTargetConditionGoal.Any(_ =>
                    _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                    !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
                {
                    var criterionLibraryEntities = new List<CriterionLibraryEntity>();
                    var criterionLibraryJoinEntities = new List<CriterionLibraryScenarioTargetConditionGoalEntity>();

                    scenarioTargetConditionGoal.Where(curve =>
                            curve.CriterionLibrary?.Id != null && curve.CriterionLibrary?.Id != Guid.Empty &&
                            !string.IsNullOrEmpty(curve.CriterionLibrary.MergedCriteriaExpression))
                        .ForEach(goal =>
                        {
                            var criterionLibraryEntity = new CriterionLibraryEntity
                            {
                                Id = Guid.NewGuid(),
                                MergedCriteriaExpression = goal.CriterionLibrary.MergedCriteriaExpression,
                                Name = $"{goal.Name} {goal.Attribute} Criterion",
                                IsSingleUse = true
                            };
                            criterionLibraryEntities.Add(criterionLibraryEntity);
                            criterionLibraryJoinEntities.Add(new CriterionLibraryScenarioTargetConditionGoalEntity
                            {
                                CriterionLibraryId = criterionLibraryEntity.Id,
                                ScenarioTargetConditionGoalId = goal.Id
                            });
                        });

                    _unitOfWork.Context.AddAll(criterionLibraryEntities, _unitOfWork.UserEntity?.Id);
                    _unitOfWork.Context.AddAll(criterionLibraryJoinEntities, _unitOfWork.UserEntity?.Id);
                }
                // Update last modified date
                var simulationEntity = _unitOfWork.Context.Simulation.Single(_ => _.Id == simulationId);
                _unitOfWork.Context.Upsert(simulationEntity, simulationId, _unitOfWork.UserEntity?.Id);
            });
        }

        public List<TargetConditionGoalLibraryDTO> GetTargetConditionGoalLibrariesNoChildrenAccessibleToUser(Guid userId)
        {
            return _unitOfWork.Context.TargetConditionGoalLibraryUser
                .AsNoTracking()
                .Include(u => u.TargetConditionGoalLibrary)
                .Where(u => u.UserId == userId)
                .Select(u => u.TargetConditionGoalLibrary.ToDto())
                .ToList();
        }
        public void UpsertOrDeleteUsers(Guid targetConditionGoalLibraryId, IList<LibraryUserDTO> libraryUsers)
        {
            var existingEntities = _unitOfWork.Context.TargetConditionGoalLibraryUser.Where(u => u.LibraryId == targetConditionGoalLibraryId).ToList();
            var existingUserIds = existingEntities.Select(u => u.UserId).ToList();
            var desiredUserIDs = libraryUsers.Select(lu => lu.UserId).ToList();
            var userIdsToDelete = existingUserIds.Except(desiredUserIDs).ToList();
            var userIdsToUpdate = existingUserIds.Intersect(desiredUserIDs).ToList();
            var userIdsToAdd = desiredUserIDs.Except(existingUserIds).ToList();
            var entitiesToAdd = libraryUsers.Where(u => userIdsToAdd.Contains(u.UserId)).Select(u => LibraryUserMapper.ToTargetConditionGoalLibraryUserEntity(u, targetConditionGoalLibraryId)).ToList();
            var dtosToUpdate = libraryUsers.Where(u => userIdsToUpdate.Contains(u.UserId)).ToList();
            var entitiesToMaybeUpdate = existingEntities.Where(u => userIdsToUpdate.Contains(u.UserId)).ToList();
            var entitiesToUpdate = new List<TargetConditionGoalLibraryUserEntity>();
            foreach (var dto in dtosToUpdate)
            {
                var entityToUpdate = entitiesToMaybeUpdate.FirstOrDefault(e => e.UserId == dto.UserId);
                if (entityToUpdate != null && entityToUpdate.AccessLevel != (int)dto.AccessLevel)
                {
                    entityToUpdate.AccessLevel = (int)dto.AccessLevel;
                    entitiesToUpdate.Add(entityToUpdate);
                }
            }
            _unitOfWork.Context.AddRange(entitiesToAdd);
            _unitOfWork.Context.UpdateRange(entitiesToUpdate);
            var entitiesToDelete = existingEntities.Where(u => userIdsToDelete.Contains(u.UserId)).ToList();
            _unitOfWork.Context.RemoveRange(entitiesToDelete);
            _unitOfWork.Context.SaveChanges();
        }
        private List<LibraryUserDTO> GetAccessForUser(Guid targetConditionGoalLibraryId, Guid userId)
        {
            var dtos = _unitOfWork.Context.TargetConditionGoalLibraryUser
                .Where(u => u.LibraryId == targetConditionGoalLibraryId && u.UserId == userId)
                .Select(LibraryUserMapper.ToDto)
                .ToList();
            return dtos;
        }
        public List<LibraryUserDTO> GetLibraryUsers(Guid targetConditionGoalLibraryId)
        {
            var dtos = _unitOfWork.Context.TargetConditionGoalLibraryUser
                .Include(u => u.User)
                .Where(u => u.LibraryId == targetConditionGoalLibraryId)
                .Select(LibraryUserMapper.ToDto)
                .ToList();
            return dtos;
        }
        public LibraryUserAccessModel GetLibraryAccess(Guid libraryId, Guid userId)
        {
            var exists = _unitOfWork.Context.BudgetLibrary.Any(bl => bl.Id == libraryId);
            if (!exists)
            {
                return LibraryAccessModels.LibraryDoesNotExist();
            }
            var users = GetAccessForUser(libraryId, userId);
            var user = users.FirstOrDefault();
            return LibraryAccessModels.LibraryExistsWithUsers(userId, user);
        }

        public void UpsertTargetConditionGoalLibraryGoalsAndPossiblyUser(TargetConditionGoalLibraryDTO dto, bool isNewLibrary, Guid ownerIdForNewLibrary)
        {
            _unitOfWork.AsTransaction(() =>
            {
                _unitOfWork.TargetConditionGoalRepo.UpsertTargetConditionGoalLibrary(dto);
                _unitOfWork.TargetConditionGoalRepo.UpsertOrDeleteTargetConditionGoals(dto.TargetConditionGoals, dto.Id);
                if (isNewLibrary)
                {

                    var users = LibraryUserDtolists.OwnerAccess(ownerIdForNewLibrary);
                    _unitOfWork.TargetConditionGoalRepo.UpsertOrDeleteUsers(dto.Id, users);
                }
            });
        }
    }
}
