using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Deficient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Deficient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.RemainingLifeLimit;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class DeficientConditionGoalRepository : IDeficientConditionGoalRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public DeficientConditionGoalRepository(UnitOfDataPersistenceWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public DateTime GetLibraryModifiedDate(Guid deficientLibraryId)
        {
            var dtos = _unitOfWork.Context.DeficientConditionGoalLibrary.Where(_ => _.Id == deficientLibraryId).FirstOrDefault().LastModifiedDate;
            return dtos;
        }

        public List<DeficientConditionGoalLibraryDTO> GetDeficientConditionGoalLibrariesWithDeficientConditionGoals()
        {
            if (!_unitOfWork.Context.DeficientConditionGoalLibrary.Any())
            {
                return new List<DeficientConditionGoalLibraryDTO>();
            }

            return _unitOfWork.Context.DeficientConditionGoalLibrary.AsNoTracking()
                .Include(_ => _.DeficientConditionGoals)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.DeficientConditionGoals)
                .ThenInclude(_ => _.CriterionLibraryDeficientConditionGoalJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Select(_ => _.ToDto())
                .ToList();
        }

        public List<DeficientConditionGoalLibraryDTO> GetDeficientConditionGoalLibrariesNoChildren()
        {
            if (!_unitOfWork.Context.DeficientConditionGoalLibrary.Any())
            {
                return new List<DeficientConditionGoalLibraryDTO>();
            }

            return _unitOfWork.Context.DeficientConditionGoalLibrary.AsNoTracking()
                .Select(_ => _.ToDto())
                .ToList();
        }
        public List<DeficientConditionGoalLibraryDTO> GetDeficientConditionGoalLibrariesNoChildrenAccessibleToUser(Guid userId)
        {
            return _unitOfWork.Context.DeficientConditionGoalLibraryUser
                .AsNoTracking()
                .Include(u => u.DeficientConditionGoalLibrary)
                .Where(u => u.UserId == userId)
                .Select(u => u.DeficientConditionGoalLibrary.ToDto())
                .ToList();
        }
        public void UpsertOrDeleteUsers(Guid deficientConditionGoalLibraryId, IList<LibraryUserDTO> libraryUsers)
        {
            var existingEntities = _unitOfWork.Context.DeficientConditionGoalLibraryUser.Where(u => u.LibraryId == deficientConditionGoalLibraryId).ToList();
            var existingUserIds = existingEntities.Select(u => u.UserId).ToList();
            var desiredUserIDs = libraryUsers.Select(lu => lu.UserId).ToList();
            var userIdsToDelete = existingUserIds.Except(desiredUserIDs).ToList();
            var userIdsToUpdate = existingUserIds.Intersect(desiredUserIDs).ToList();
            var userIdsToAdd = desiredUserIDs.Except(existingUserIds).ToList();
            var entitiesToAdd = libraryUsers.Where(u => userIdsToAdd.Contains(u.UserId)).Select(u => LibraryUserMapper.ToDeficientConditionGoalLibraryUserEntity(u, deficientConditionGoalLibraryId)).ToList();
            var dtosToUpdate = libraryUsers.Where(u => userIdsToUpdate.Contains(u.UserId)).ToList();
            var entitiesToMaybeUpdate = existingEntities.Where(u => userIdsToUpdate.Contains(u.UserId)).ToList();
            var entitiesToUpdate = new List<DeficientConditionGoalLibraryUserEntity>();
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
        private List<LibraryUserDTO> GetAccessForUser(Guid deficientConditionGoalLibraryId, Guid userId)
        {
            var dtos = _unitOfWork.Context.DeficientConditionGoalLibraryUser
                .Where(u => u.LibraryId == deficientConditionGoalLibraryId && u.UserId == userId)
                .Select(LibraryUserMapper.ToDto)
                .ToList();
            return dtos;
        }

        public List<LibraryUserDTO> GetLibraryUsers(Guid deficientConditionGoalLibraryId)
        {
            var dtos = _unitOfWork.Context.DeficientConditionGoalLibraryUser
                .Include(u => u.User)
                .Where(u => u.LibraryId == deficientConditionGoalLibraryId)
                .Select(LibraryUserMapper.ToDto)
                .ToList();
            return dtos;
        }

        public void UpsertDeficientConditionGoalLibrary(DeficientConditionGoalLibraryDTO dto)
        {
            var deficientConditionGoalLibraryEntity = dto.ToEntity();

            _unitOfWork.Context.Upsert(deficientConditionGoalLibraryEntity, dto.Id, _unitOfWork.UserEntity?.Id);
        }


        public void UpsertDeficientConditionGoalLibraryAndGoals(DeficientConditionGoalLibraryDTO dto)
        {
            _unitOfWork.AsTransaction(() =>
            {
                UpsertDeficientConditionGoalLibrary(dto);
                UpsertOrDeleteDeficientConditionGoals(dto.DeficientConditionGoals, dto.Id);
            });
        }

        public void UpsertOrDeleteDeficientConditionGoals(List<DeficientConditionGoalDTO> deficientConditionGoals,
            Guid libraryId)
        {
            if (!_unitOfWork.Context.DeficientConditionGoalLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException("The specified deficient condition goal library was not found.");
            }

            if (deficientConditionGoals.Any(_ => string.IsNullOrEmpty(_.Attribute)))
            {
                throw new InvalidOperationException("All deficient condition goals must have an attribute.");
            }

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
                .Select(_ => _.ToLibraryEntity(libraryId, attributeEntities.Single(__ => __.Name == _.Attribute).Id)).ToList();

            var entityIds = deficientConditionGoalEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.DeficientConditionGoal
                .Where(_ => _.DeficientConditionGoalLibraryId == libraryId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            _unitOfWork.Context.DeleteAll<DeficientConditionGoalEntity>(_ =>
                _.DeficientConditionGoalLibraryId == libraryId && !entityIds.Contains(_.Id));

            _unitOfWork.Context.UpdateAll(
                deficientConditionGoalEntities.Where(_ => existingEntityIds.Contains(_.Id)).ToList(),
                _unitOfWork.UserEntity?.Id);

            _unitOfWork.Context.AddAll(
                deficientConditionGoalEntities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList(),
                _unitOfWork.UserEntity?.Id);

            _unitOfWork.Context.DeleteAll<CriterionLibraryDeficientConditionGoalEntity>(_ =>
                _.DeficientConditionGoal.DeficientConditionGoalLibraryId == libraryId);

            if (deficientConditionGoals.Any((_ =>
                _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression))))
            {
                var criterionLibraryEntities = new List<CriterionLibraryEntity>();
                var criterionLibraryJoinEntities = new List<CriterionLibraryDeficientConditionGoalEntity>();

                deficientConditionGoals.Where(curve =>
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
                        criterionLibraryJoinEntities.Add(new CriterionLibraryDeficientConditionGoalEntity
                        {
                            CriterionLibraryId = criterionLibraryEntity.Id,
                            DeficientConditionGoalId = goal.Id
                        });
                    });

                _unitOfWork.Context.AddAll(criterionLibraryEntities, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criterionLibraryJoinEntities, _unitOfWork.UserEntity?.Id);
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

        public List<DeficientConditionGoalDTO> GetScenarioDeficientConditionGoals(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found for the given scenario.");
            }

            var res = _unitOfWork.Context.ScenarioDeficientConditionGoal.Where(_ => _.SimulationId == simulationId)
                .Include(_ => _.CriterionLibraryScenarioDeficientConditionGoalJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Attribute)
                .Select(_ => _.ToDto())
                .AsNoTracking()
                .ToList();
            return res;
        }

        public List<DeficientConditionGoalDTO> GetDeficientConditionGoalsByLibraryId(Guid libraryId)
        {
            if (!_unitOfWork.Context.DeficientConditionGoalLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException($"The given deficient condition library was not found.");
            }

            var res = _unitOfWork.Context.DeficientConditionGoal.Where(_ => _.DeficientConditionGoalLibraryId == libraryId)
                .Include(_ => _.CriterionLibraryDeficientConditionGoalJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Attribute)
                .Select(_ => _.ToDto())
                .AsNoTracking()
                .ToList();
            return res;
        }

        public LibraryUserAccessModel GetLibraryAccess(Guid libraryId, Guid userId)
        {
            var exists = _unitOfWork.Context.DeficientConditionGoalLibrary.Any(bl => bl.Id == libraryId);
            if (!exists)
            {
                return LibraryAccessModels.LibraryDoesNotExist();
            }
            var users = GetAccessForUser(libraryId, userId);
            var user = users.FirstOrDefault();
            return LibraryAccessModels.LibraryExistsWithUsers(userId, user);
        }

        public void UpsertOrDeleteScenarioDeficientConditionGoals(List<DeficientConditionGoalDTO> scenarioDeficientConditionGoal, Guid simulationId)
        {
            _unitOfWork.AsTransaction(() =>
            {
                if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
                {
                    throw new RowNotInTableException($"No simulation found for the given scenario.");
                }
                if (scenarioDeficientConditionGoal.Any(_ => string.IsNullOrEmpty(_.Attribute)))
                {
                    throw new InvalidOperationException("All deficient conditions must have an attribute.");
                }
                var attributeEntities = _unitOfWork.Context.Attribute.ToList();
                var attributeNames = attributeEntities.Select(_ => _.Name).ToList();

                if (!scenarioDeficientConditionGoal.All(_ => attributeNames.Contains(_.Attribute)))
                {
                    var missingAttributes = scenarioDeficientConditionGoal.Select(_ => _.Attribute)
                        .Except(attributeNames).ToList();
                    if (missingAttributes.Count == 1)
                    {
                        throw new RowNotInTableException($"No attribute found having name {missingAttributes[0]}.");
                    }

                    throw new RowNotInTableException(
                        $"No attributes found having names: {string.Join(", ", missingAttributes)}.");
                }
                var scenarioDeficientConditionGoalEntities = scenarioDeficientConditionGoal
                    .Select(_ =>
                        _.ToScenarioEntity(simulationId, attributeEntities.Single(__ => __.Name == _.Attribute).Id))
                    .ToList();
                var entityIds = scenarioDeficientConditionGoal.Select(_ => _.Id).ToList();

                var existingEntityIds = _unitOfWork.Context.ScenarioDeficientConditionGoal
                    .Where(_ => _.SimulationId == simulationId && entityIds.Contains(_.Id))
                    .Select(_ => _.Id).ToList();

                _unitOfWork.Context.DeleteAll<ScenarioDeficientConditionGoalEntity>(_ =>
                    _.SimulationId == simulationId && !entityIds.Contains(_.Id));

                _unitOfWork.Context.UpdateAll(scenarioDeficientConditionGoalEntities.Where(_ => existingEntityIds.Contains(_.Id))
                    .ToList(), _unitOfWork.UserEntity?.Id);

                _unitOfWork.Context.AddAll(scenarioDeficientConditionGoalEntities.Where(_ => !existingEntityIds.Contains(_.Id))
                    .ToList(), _unitOfWork.UserEntity?.Id);

                _unitOfWork.Context.DeleteAll<CriterionLibraryScenarioDeficientConditionGoalEntity>(_ =>
                    _.ScenarioDeficientConditionGoal.SimulationId == simulationId);

                if (scenarioDeficientConditionGoal.Any(_ =>
                    _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                    !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
                {
                    var criterionLibraryEntities = new List<CriterionLibraryEntity>();
                    var criterionLibraryJoinEntities = new List<CriterionLibraryScenarioDeficientConditionGoalEntity>();

                    scenarioDeficientConditionGoal.Where(curve =>
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
                            criterionLibraryJoinEntities.Add(new CriterionLibraryScenarioDeficientConditionGoalEntity
                            {
                                CriterionLibraryId = criterionLibraryEntity.Id,
                                ScenarioDeficientConditionGoalId = goal.Id
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

        public void AddLibraryIdToScenarioDeficientConditionGoal(List<DeficientConditionGoalDTO> deficientConditionGoalDTOs, Guid? libraryId)
        {
            if (libraryId == null) return;
            foreach (var dto in deficientConditionGoalDTOs)
            {
                dto.LibraryId = (Guid)libraryId;
            }
        }

        public void AddModifiedToScenarioDeficientConditionGoal(List<DeficientConditionGoalDTO> deficientConditionGoalDTOs, bool IsModified)
        {
            foreach (var dto in deficientConditionGoalDTOs)
            {
                dto.IsModified = IsModified;
            }
        }

    }
}
