using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.BudgetPriority;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.BudgetPriority;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class BudgetPriorityRepository : IBudgetPriorityRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public BudgetPriorityRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateBudgetPriorities(List<BudgetPriority> budgetPriorities, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation found for given scenario.");
            }

            var simulationEntity = _unitOfWork.Context.Simulation.AsNoTracking()
                .Single(_ => _.Id == simulationId);

            var budgetPriorityEntities = budgetPriorities
                .Select(_ => _.ToScenarioEntity(simulationId))
                .ToList();

            _unitOfWork.Context.AddAll(budgetPriorityEntities, _unitOfWork.UserEntity?.Id);

            if (budgetPriorities.Any(_ => _.BudgetPercentagePairs.Any()))
            {
                var budgetPercentagePairEntities = budgetPriorities
                    .SelectMany(_ => _.BudgetPercentagePairs.Select(pair => pair.ToEntity(_.Id, pair.Budget.Id)))
                    .ToList();
                _unitOfWork.Context.AddAll(budgetPercentagePairEntities, _unitOfWork.UserEntity?.Id);
            }

            if (budgetPriorities.Any(_ => !_.Criterion.ExpressionIsBlank))
            {
                var criterionJoins = new List<CriterionLibraryScenarioBudgetPriorityEntity>();

                var criteria = budgetPriorities.Where(_ => !_.Criterion.ExpressionIsBlank)
                    .Select(priority =>
                    {
                        var criterion = new CriterionLibraryEntity
                        {
                            Id = Guid.NewGuid(),
                            MergedCriteriaExpression = priority.Criterion.Expression,
                            Name = $"{simulationEntity.Name} Priority {priority.Year}-{priority.PriorityLevel} Criterion",
                            IsSingleUse = true
                        };
                        criterionJoins.Add(new CriterionLibraryScenarioBudgetPriorityEntity
                        {
                            CriterionLibraryId = criterion.Id, ScenarioBudgetPriorityId = priority.Id
                        });
                        return criterion;
                    }).ToList();

                _unitOfWork.Context.AddAll(criteria, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criterionJoins, _unitOfWork.UserEntity?.Id);
            }

            // Update last modified date
            _unitOfWork.SimulationRepo.UpdateLastModifiedDate(simulationEntity);
        }

        public List<BudgetPriorityLibraryDTO> GetBudgetPriorityLibraries()
        {
            if (!_unitOfWork.Context.BudgetPriorityLibrary.Any())
            {
                return new List<BudgetPriorityLibraryDTO>();
            }

            return _unitOfWork.Context.BudgetPriorityLibrary.AsNoTracking()
                .Include(_ => _.BudgetPriorities)
                .ThenInclude(_ => _.CriterionLibraryBudgetPriorityJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Select(_ => _.ToDto())
                .ToList();
        }

        public List<BudgetPriorityLibraryDTO> GetBudgetPriortyLibrariesNoChildren()
        {
            if (!_unitOfWork.Context.BudgetPriorityLibrary.Any())
            {
                return new List<BudgetPriorityLibraryDTO>();
            }

            return _unitOfWork.Context.BudgetPriorityLibrary.AsNoTracking()
                .Select(_ => _.ToDto())
                .ToList();
        }

        public void UpsertBudgetPriorityLibrary(BudgetPriorityLibraryDTO dto) =>
            _unitOfWork.Context.Upsert(dto.ToEntity(), dto.Id, _unitOfWork.UserEntity?.Id);

        public void UpsertOrDeleteBudgetPriorities(List<BudgetPriorityDTO> budgetPriorities, Guid libraryId)
        {
            if (!_unitOfWork.Context.BudgetPriorityLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException("The specified budget priority library was not found.");
            }

            var budgetPriorityEntities = budgetPriorities
                .Select(_ => _.ToLibraryEntity(libraryId))
                .ToList();

            var entityIds = budgetPriorityEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.BudgetPriority.AsNoTracking()
                .Where(_ => _.BudgetPriorityLibraryId == libraryId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            _unitOfWork.Context.DeleteAll<BudgetPriorityEntity>(_ =>
                _.BudgetPriorityLibraryId == libraryId && !entityIds.Contains(_.Id));

            _unitOfWork.Context.UpdateAll(budgetPriorityEntities.Where(_ => existingEntityIds.Contains(_.Id)).ToList(), _unitOfWork.UserEntity?.Id);

            _unitOfWork.Context.AddAll(budgetPriorityEntities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList(), _unitOfWork.UserEntity?.Id);

            _unitOfWork.Context.DeleteAll<CriterionLibraryBudgetPriorityEntity>(_ =>
                _.BudgetPriority.BudgetPriorityLibraryId == libraryId);

            if (budgetPriorities.Any(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                                          !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
            {
                var criterionJoins = new List<CriterionLibraryBudgetPriorityEntity>();

                var criteria = budgetPriorities
                    .Where(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                               !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression))
                    .Select(priority =>
                    {
                        var criterion = new CriterionLibraryEntity
                        {
                            Id = Guid.NewGuid(),
                            MergedCriteriaExpression = priority.CriterionLibrary.MergedCriteriaExpression,
                            Name = $"Priority {priority.Year}-{priority.PriorityLevel} Criterion",
                            IsSingleUse = true
                        };
                        criterionJoins.Add(new CriterionLibraryBudgetPriorityEntity
                        {
                            CriterionLibraryId = criterion.Id, BudgetPriorityId = priority.Id
                        });
                        return criterion;
                    }).ToList();

                _unitOfWork.Context.AddAll(criteria, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criterionJoins, _unitOfWork.UserEntity?.Id);
            }
        }

        public void DeleteBudgetPriorityLibrary(Guid libraryId)
        {
            if (!_unitOfWork.Context.BudgetPriorityLibrary.Any(_ => _.Id == libraryId))
            {
                return;
            }

            _unitOfWork.Context.DeleteEntity<BudgetPriorityLibraryEntity>(_ => _.Id == libraryId);
        }

        public List<BudgetPriorityDTO> GetScenarioBudgetPriorities(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            return _unitOfWork.Context.ScenarioBudgetPriority.AsNoTracking()
                .Include(_ => _.BudgetPercentagePairs)
                .ThenInclude(_ => _.ScenarioBudget)
                .Include(_ => _.CriterionLibraryScenarioBudgetPriorityJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Where(_ => _.SimulationId == simulationId)
                .OrderBy(_ => _.PriorityLevel)
                .Select(_ => _.ToDto())
                .ToList();
        }

        public void UpsertOrDeleteScenarioBudgetPriorities(List<BudgetPriorityDTO> budgetPriorities, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            var simulationEntity = _unitOfWork.Context.Simulation.AsNoTracking()
                .Single(_ => _.Id == simulationId);

            var budgetPriorityEntities = budgetPriorities
                .Select(_ => _.ToScenarioEntity(simulationId))
                .ToList();

            var entityIds = budgetPriorityEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.ScenarioBudgetPriority.AsNoTracking()
                .Where(_ => _.SimulationId == simulationId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            _unitOfWork.Context.DeleteAll<BudgetPercentagePairEntity>(_ => _.ScenarioBudgetPriority.SimulationId == simulationId);

            _unitOfWork.Context.DeleteAll<ScenarioBudgetPriorityEntity>(_ =>
                _.SimulationId == simulationId && !entityIds.Contains(_.Id));

            _unitOfWork.Context.UpdateAll(budgetPriorityEntities.Where(_ => existingEntityIds.Contains(_.Id)).ToList(), _unitOfWork.UserEntity?.Id);

            _unitOfWork.Context.AddAll(budgetPriorityEntities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList(), _unitOfWork.UserEntity?.Id);

            _unitOfWork.Context.DeleteAll<CriterionLibraryScenarioBudgetPriorityEntity>(_ =>
                _.ScenarioBudgetPriority.SimulationId == simulationId);

            if (budgetPriorities.Any(_ => _.BudgetPercentagePairs.Any()))
            {
                _unitOfWork.Context.AddAll(budgetPriorities.Where(_ => _.BudgetPercentagePairs.Any())
                    .SelectMany(_ => _.BudgetPercentagePairs.Select(pair => pair.ToEntity(_.Id))).ToList(), _unitOfWork.UserEntity?.Id);
            }

            if (budgetPriorities.Any(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                                          !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
            {
                var criterionJoins = new List<CriterionLibraryScenarioBudgetPriorityEntity>();

                var criteria = budgetPriorities
                    .Where(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                               !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression))
                    .Select(priority =>
                    {
                        var criterion = new CriterionLibraryEntity
                        {
                            Id = Guid.NewGuid(),
                            MergedCriteriaExpression = priority.CriterionLibrary.MergedCriteriaExpression,
                            Name = $"{simulationEntity.Name} Priority {priority.Year}-{priority.PriorityLevel} Criterion",
                            IsSingleUse = true
                        };
                        criterionJoins.Add(new CriterionLibraryScenarioBudgetPriorityEntity
                        {
                            CriterionLibraryId = criterion.Id, ScenarioBudgetPriorityId = priority.Id
                        });
                        return criterion;
                    }).ToList();

                _unitOfWork.Context.AddAll(criteria, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criterionJoins, _unitOfWork.UserEntity?.Id);
            }

            // Update last modified date
            _unitOfWork.SimulationRepo.UpdateLastModifiedDate(simulationEntity);
        }

        public List<BudgetPriorityDTO> GetBudgetPrioritiesByLibraryId(Guid libraryId)
        {
            if (!_unitOfWork.Context.BudgetPriorityLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException("No Library was found for the given library.");
            }

            return _unitOfWork.Context.BudgetPriority.AsNoTracking()
                .Include(_ => _.CriterionLibraryBudgetPriorityJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Where(_ => _.BudgetPriorityLibraryId == libraryId)
                .Select(_ => _.ToDto())
                .ToList();
        }

        public List<BudgetPriorityLibraryDTO> GetBudgetPriorityLibrariesNoChildrenAccessibleToUser(Guid userId)
        {
            return _unitOfWork.Context.BudgetPriorityLibraryUser
                .AsNoTracking()
                .Include(u => u.BudgetPriorityLibrary)
                .Where(u => u.UserId == userId)
                .Select(u => u.BudgetPriorityLibrary.ToDto())
                .ToList();
        }
        public void UpsertOrDeleteUsers(Guid budgetPriorityLibraryId, IList<LibraryUserDTO> libraryUsers)
        {
            var existingEntities = _unitOfWork.Context.BudgetPriorityLibraryUser.Where(u => u.LibraryId == budgetPriorityLibraryId).ToList();
            var existingUserIds = existingEntities.Select(u => u.UserId).ToList();
            var desiredUserIDs = libraryUsers.Select(lu => lu.UserId).ToList();
            var userIdsToDelete = existingUserIds.Except(desiredUserIDs).ToList();
            var userIdsToUpdate = existingUserIds.Intersect(desiredUserIDs).ToList();
            var userIdsToAdd = desiredUserIDs.Except(existingUserIds).ToList();
            var entitiesToAdd = libraryUsers.Where(u => userIdsToAdd.Contains(u.UserId)).Select(u => LibraryUserMapper.ToBudgetPriorityLibraryUserEntity(u, budgetPriorityLibraryId)).ToList();
            var dtosToUpdate = libraryUsers.Where(u => userIdsToUpdate.Contains(u.UserId)).ToList();
            var entitiesToMaybeUpdate = existingEntities.Where(u => userIdsToUpdate.Contains(u.UserId)).ToList();
            var entitiesToUpdate = new List<BudgetPriorityLibraryUserEntity>();
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
        private List<LibraryUserDTO> GetAccessForUser(Guid budgetPriorityLibraryId, Guid userId)
        {
            var dtos = _unitOfWork.Context.BudgetPriorityLibraryUser
                .Where(u => u.LibraryId == budgetPriorityLibraryId && u.UserId == userId)
                .Select(LibraryUserMapper.ToDto)
                .ToList();
            return dtos;
        }
        public List<LibraryUserDTO> GetLibraryUsers(Guid budgetPriorityLibraryId)
        {
            var dtos = _unitOfWork.Context.BudgetPriorityLibraryUser
                .Include(u => u.User)
                .Where(u => u.LibraryId == budgetPriorityLibraryId)
                .Select(LibraryUserMapper.ToDto)
                .ToList();
            return dtos;
        }
        public LibraryUserAccessModel GetLibraryAccess(Guid libraryId, Guid userId)
        {
            var exists = _unitOfWork.Context.BudgetPriorityLibrary.Any(bl => bl.Id == libraryId);
            if (!exists)
            {
                return LibraryAccessModels.LibraryDoesNotExist();
            }
            var users = GetAccessForUser(libraryId, userId);
            var user = users.FirstOrDefault();
            return LibraryAccessModels.LibraryExistsWithUsers(userId, user);
        }
    }
}
