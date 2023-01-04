﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public BudgetRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateScenarioBudgets(List<Budget> budgets, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            var budgetEntities = budgets.Select(_ => _.ToScenarioEntity(simulationId))
                .ToList();

            _unitOfWork.Context.AddAll(budgetEntities);

            if (budgets.Any(_ => _.YearlyAmounts.Any()))
            {
                var budgetAmountsPerBudgetId = budgets
                    .Where(_ => _.YearlyAmounts.Any())
                    .ToDictionary(_ => _.Id, _ => _.YearlyAmounts.ToList());

                _unitOfWork.BudgetAmountRepo.CreateScenarioBudgetAmounts(budgetAmountsPerBudgetId, simulationId);
            }
        }

        public List<SimpleBudgetDetailDTO> GetScenarioSimpleBudgetDetails(Guid simulationId)
        {
            if (simulationId == Guid.Empty)
            {
                return new List<SimpleBudgetDetailDTO>();
            }

            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            if (!_unitOfWork.Context.ScenarioBudget.Any(_ => _.SimulationId == simulationId))
            {
                return new List<SimpleBudgetDetailDTO>();
            }

            return _unitOfWork.Context.ScenarioBudget.AsNoTracking()
                .Where(_ => _.SimulationId == simulationId)
                .Select(_ => new SimpleBudgetDetailDTO {Id = _.Id, Name = _.Name})
                .OrderBy(_ => _.Name)
                .ToList();
        }



        public List<BudgetLibraryDTO> GetBudgetLibraries()
        {
            if (!_unitOfWork.Context.BudgetLibrary.Any())
            {
                return new List<BudgetLibraryDTO>();
            }

            return _unitOfWork.Context.BudgetLibrary.AsNoTracking()
                .Include(_ => _.Budgets)
                .ThenInclude(_ => _.BudgetAmounts)
                .Include(_ => _.Budgets)
                .ThenInclude(_ => _.CriterionLibraryBudgetJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Select(_ => _.ToDto())
                .ToList();
        }

        public List<BudgetLibraryDTO> GetBudgetLibrariesNoChildren()
        {
            if (!_unitOfWork.Context.BudgetLibrary.Any())
            {
                return new List<BudgetLibraryDTO>();
            }

            return _unitOfWork.Context.BudgetLibrary.AsNoTracking()
                .Select(_ => _.ToDto())
                .ToList();
        }

        public List<BudgetLibraryDTO> GetBudgetLibrariesNoChildrenAccessibleToUser(Guid userId)
        {
            return _unitOfWork.Context.BudgetLibraryUser
                .AsNoTracking()
                .Include(u => u.BudgetLibrary)
                .Where(u => u.UserId == userId)
                .Select(u => u.BudgetLibrary.ToDto())
                .ToList();
        }

        public void UpsertOrDeleteUsers(Guid budgetLibraryId, IList<LibraryUserDTO> libraryUsers)
        {
            var existingEntities = _unitOfWork.Context.BudgetLibraryUser.Where(u => u.BudgetLibraryId == budgetLibraryId).ToList();
            var existingUserIds = existingEntities.Select(u => u.UserId).ToList();
            var desiredUserIDs = libraryUsers.Select(lu => lu.UserId).ToList();
            var userIdsToDelete = existingUserIds.Except(desiredUserIDs).ToList();
            var userIdsToUpdate = existingUserIds.Intersect(desiredUserIDs).ToList();
            var userIdsToAdd = desiredUserIDs.Except(existingUserIds).ToList();
            var entitiesToAdd = libraryUsers.Where(u => userIdsToAdd.Contains(u.UserId)).Select(u => LibraryUserMapper.ToBudgetLibraryUserEntity(u, budgetLibraryId)).ToList();
            var dtosToUpdate = libraryUsers.Where(u => userIdsToUpdate.Contains(u.UserId)).ToList();
            var entitiesToMaybeUpdate = existingEntities.Where(u => userIdsToUpdate.Contains(u.UserId)).ToList();
            var entitiesToUpdate = new List<BudgetLibraryUserEntity>();
            foreach (var dto in dtosToUpdate)
            {
                var entityToUpdate = entitiesToMaybeUpdate.FirstOrDefault(e => e.UserId == dto.UserId);
                if (entityToUpdate!=null && entityToUpdate.AccessLevel != (int)dto.AccessLevel)
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

        private List<LibraryUserDTO> GetAccessForUser(Guid budgetLibraryId, Guid userId)
        {
            var dtos = _unitOfWork.Context.BudgetLibraryUser
                .Where(u => u.BudgetLibraryId == budgetLibraryId && u.UserId == userId)
                .Select(LibraryUserMapper.ToDto)
                .ToList();
            return dtos;
        }

        public List<LibraryUserDTO> GetLibraryUsers(Guid budgetLibraryId)
        {
            var dtos = _unitOfWork.Context.BudgetLibraryUser
                .Include(u => u.User)
                .Where(u => u.BudgetLibraryId == budgetLibraryId)
                .Select(LibraryUserMapper.ToDto)
                .ToList();
            return dtos;
        }


        public void UpsertBudgetLibrary(BudgetLibraryDTO dto) {
            var libraryExists = _unitOfWork.Context.BudgetLibrary.Any(bl => bl.Id == dto.Id);
            _unitOfWork.Context.Upsert(dto.ToEntity(), dto.Id, _unitOfWork.UserEntity?.Id);
            _unitOfWork.Context.SaveChanges();
        }

        public void UpsertOrDeleteBudgets(List<BudgetDTO> budgets, Guid libraryId)
        {
            if (!_unitOfWork.Context.BudgetLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException("The specified budget library was not found.");
            }

            var budgetEntities = budgets.Select(_ => _.ToLibraryEntity(libraryId)).ToList();

            var entityIds = budgetEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.Budget.AsNoTracking()
                .Where(_ => _.BudgetLibraryId == libraryId && entityIds.Contains(_.Id)).Select(_ => _.Id).ToList();

            _unitOfWork.Context.DeleteAll<BudgetEntity>(_ =>
                _.BudgetLibraryId == libraryId && !entityIds.Contains(_.Id));

            _unitOfWork.Context.UpdateAll(
                budgetEntities.Where(_ => existingEntityIds.Contains(_.Id)).ToList(), _unitOfWork.UserEntity?.Id);

            _unitOfWork.Context.AddAll(
                budgetEntities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList(), _unitOfWork.UserEntity?.Id);

            var budgetAmountsPerBudgetId = budgets.ToDictionary(_ => _.Id, _ => _.BudgetAmounts);

            _unitOfWork.BudgetAmountRepo.UpsertOrDeleteBudgetAmounts(budgetAmountsPerBudgetId, libraryId);

            _unitOfWork.Context.DeleteAll<CriterionLibraryBudgetEntity>(_ =>
                _.Budget.BudgetLibraryId == libraryId);

            if (budgets.Any(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                                 !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
            {
                var criterionJoins = new List<CriterionLibraryBudgetEntity>();

                var criteria = budgets
                    .Where(budget => budget.CriterionLibrary?.Id != null && budget.CriterionLibrary?.Id != Guid.Empty &&
                                     !string.IsNullOrEmpty(budget.CriterionLibrary.MergedCriteriaExpression))
                    .Select(budget =>
                    {
                        var criterion = new CriterionLibraryEntity
                        {
                            Id = Guid.NewGuid(),
                            MergedCriteriaExpression = budget.CriterionLibrary.MergedCriteriaExpression,
                            Name = $"{budget.Name} Criterion",
                            IsSingleUse = true
                        };
                        criterionJoins.Add(new CriterionLibraryBudgetEntity
                        {
                            CriterionLibraryId = criterion.Id, BudgetId = budget.Id
                        });
                        return criterion;
                    }).ToList();

                _unitOfWork.Context.AddAll(criteria, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criterionJoins, _unitOfWork.UserEntity?.Id);
            }
        }

        public void DeleteBudgetLibrary(Guid libraryId)
        {
            if (!_unitOfWork.Context.BudgetLibrary.Any(_ => _.Id == libraryId))
            {
                return;
            }

            _unitOfWork.Context.DeleteEntity<BudgetLibraryEntity>(_ => _.Id == libraryId);
        }

        public List<BudgetEntity> GetLibraryBudgets(Guid libraryId)
        {
            if (!_unitOfWork.Context.BudgetLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException("The specified budget library was not found.");
            }

            return _unitOfWork.Context.Budget.AsNoTracking()
                .Include(_ => _.BudgetAmounts)
                .Where(_ => _.BudgetLibrary.Id == libraryId)
                .ToList();
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

        public BudgetLibraryDTO GetBudgetLibrary(Guid libraryId)
        {
            if (!_unitOfWork.Context.BudgetLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException("The specified budget library was not found.");
            }

            return _unitOfWork.Context.BudgetLibrary.AsNoTracking()
                .Include(_ => _.Budgets)
                .ThenInclude(_ => _.BudgetAmounts)
                .Include(_ => _.Budgets)
                .ThenInclude(_ => _.CriterionLibraryBudgetJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Users)
                .Single(_ => _.Id == libraryId)
                .ToDto();
        }

        public List<BudgetDTO> GetScenarioBudgets(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            return _unitOfWork.Context.ScenarioBudget.AsNoTracking().AsSplitQuery()
                .Where(_ => _.SimulationId == simulationId)
                .Include(_ => _.ScenarioBudgetAmounts)
                .Include(_ => _.CriterionLibraryScenarioBudgetJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Select(_ => _.ToDto())
                .ToList();
        }

        public void UpsertOrDeleteScenarioBudgets(List<BudgetDTO> budgets, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            var budgetEntities = budgets.Select(_ => _.ToScenarioEntity(simulationId)).ToList();

            var entityIds = budgetEntities.Select(_ => _.Id).ToList();
            entityIds.Add(Guid.Empty);

            var existingEntityIds = _unitOfWork.Context.ScenarioBudget.AsNoTracking()
                .Where(_ => _.SimulationId == simulationId && entityIds.Contains(_.Id)).Select(_ => _.Id).ToList();

            var committedProjects = _unitOfWork.Context.CommittedProject.Where(_ =>
                _.SimulationId == simulationId && !entityIds.Contains(_.ScenarioBudgetId ?? Guid.Empty)).ToList();
            if(committedProjects.Count > 0)
            {
                committedProjects.ForEach(_ => _.ScenarioBudgetId = null);
                _unitOfWork.Context.UpdateAll(committedProjects);
            }
            
            _unitOfWork.Context.DeleteAll<ScenarioBudgetEntity>(_ =>
                _.SimulationId == simulationId && !entityIds.Contains(_.Id));

            _unitOfWork.Context.UpdateAll(
                budgetEntities.Where(_ => existingEntityIds.Contains(_.Id)).ToList(), _unitOfWork.UserEntity?.Id);

            _unitOfWork.Context.AddAll(
                budgetEntities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList(), _unitOfWork.UserEntity?.Id);

            var budgetAmountsPerBudgetId = budgets.ToDictionary(_ => _.Id, _ => _.BudgetAmounts);

            _unitOfWork.BudgetAmountRepo.UpsertOrDeleteScenarioBudgetAmounts(budgetAmountsPerBudgetId, simulationId);

            _unitOfWork.Context.DeleteAll<CriterionLibraryScenarioBudgetEntity>(_ =>
                _.ScenarioBudget.SimulationId == simulationId);

            if (budgets.Any(_ =>
                _.CriterionLibrary?.Id != null && !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
            {
                var criterionJoins = new List<CriterionLibraryScenarioBudgetEntity>();

                var criteria = budgets
                    .Where(budget => budget.CriterionLibrary?.Id != null && !string.IsNullOrEmpty(budget.CriterionLibrary.MergedCriteriaExpression))
                    .Select(budget =>
                    {
                        var criterion = new CriterionLibraryEntity
                        {
                            Id = Guid.NewGuid(),
                            MergedCriteriaExpression = budget.CriterionLibrary.MergedCriteriaExpression,
                            Name = $"{budget.Name} Criterion",
                            IsSingleUse = true
                        };
                        criterionJoins.Add(new CriterionLibraryScenarioBudgetEntity
                        {
                            CriterionLibraryId = criterion.Id, ScenarioBudgetId = budget.Id
                        });
                        return criterion;
                    }).ToList();

                _unitOfWork.Context.AddAll(criteria, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criterionJoins, _unitOfWork.UserEntity?.Id);
            }
        }

        public List<int> GetBudgetYearsBySimulationId(Guid simulationId)
        {
            var years = new List<int>();
            var budget = _unitOfWork.Context.ScenarioBudget.Include(_ => _.ScenarioBudgetAmounts).FirstOrDefault(_ => _.SimulationId == simulationId);

            if (budget != null)
            {
                years = budget.ScenarioBudgetAmounts.Select(_ => _.Year).ToList();
            }

            return years;
        }
    }
}
