﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class BudgetPriorityRepository : IBudgetPriorityRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public BudgetPriorityRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfDataPersistenceWork)
        {
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
        }

        public void CreateBudgetPriorityLibrary(string name, Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var budgetPriorityLibraryEntity = new BudgetPriorityLibraryEntity { Id = Guid.NewGuid(), Name = name };

            _unitOfDataPersistenceWork.Context.BudgetPriorityLibrary.Add(budgetPriorityLibraryEntity);

            _unitOfDataPersistenceWork.Context.BudgetPriorityLibrarySimulation.Add(new BudgetPriorityLibrarySimulationEntity
            {
                BudgetPriorityLibraryId = budgetPriorityLibraryEntity.Id,
                SimulationId = simulationId
            });

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        public void CreateBudgetPriorities(List<BudgetPriority> budgetPriorities, Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var simulationEntity = _unitOfDataPersistenceWork.Context.Simulation
                .Include(_ => _.BudgetPriorityLibrarySimulationJoin)
                .Single(_ => _.Id == simulationId);

            if (simulationEntity.BudgetPriorityLibrarySimulationJoin == null)
            {
                throw new RowNotInTableException($"No budget priority library found for simulation having id {simulationId}");
            }

            var budgetPriorityEntities = budgetPriorities
                .Select(_ => _.ToEntity(simulationEntity.BudgetPriorityLibrarySimulationJoin.BudgetPriorityLibraryId))
                .ToList();

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.BudgetPriority.AddRange(budgetPriorityEntities);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkInsert(budgetPriorityEntities);
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();

            if (budgetPriorities.Any(_ => !_.Criterion.ExpressionIsBlank))
            {
                var priorityIdsPerExpression = budgetPriorities
                    .Where(_ => !_.Criterion.ExpressionIsBlank)
                    .GroupBy(_ => _.Criterion.Expression, _ => _)
                    .ToDictionary(_ => _.Key, _ => _.Select(__ => __.Id).ToList());

                _unitOfDataPersistenceWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(priorityIdsPerExpression,
                    DataPersistenceConstants.CriterionLibraryJoinEntities.BudgetPriority, simulationEntity.Name);
            }

            if (budgetPriorities.Any(_ => _.BudgetPercentagePairs.Any()))
            {
                var percentagePairPerBudgetIdPerPriorityId = budgetPriorities
                    .Where(_ => _.BudgetPercentagePairs.Any())
                    .ToDictionary(_ => _.Id, _ => _.BudgetPercentagePairs.Select(__ => (__.Budget.Id, __)).ToList());

                _unitOfDataPersistenceWork.BudgetPercentagePairRepo.CreateBudgetPercentagePairs(percentagePairPerBudgetIdPerPriorityId);
            }
        }

        public Task<List<BudgetPriorityLibraryDTO>> BudgetPriorityLibrariesWithBudgetPriorities()
        {
            if (!_unitOfDataPersistenceWork.Context.BudgetPriorityLibrary.Any())
            {
                return Task.Factory.StartNew(() => new List<BudgetPriorityLibraryDTO>());
            }

            return Task.Factory.StartNew(() => _unitOfDataPersistenceWork.Context.BudgetPriorityLibrary
                .Include(_ => _.BudgetPriorities)
                .ThenInclude(_ => _.CriterionLibraryBudgetPriorityJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.BudgetPriorities)
                .ThenInclude(_ => _.BudgetPercentagePairs)
                .ThenInclude(_ => _.Budget)
                .Include(_ => _.BudgetPriorityLibrarySimulationJoins)
                .Select(_ => _.ToDto())
                .ToList());
        }

        public void UpsertPermitted(UserInfoDTO userInfo, Guid simulationId, BudgetPriorityLibraryDTO dto)
        {
            if (simulationId != Guid.Empty)
            {
                if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
                {
                    throw new RowNotInTableException($"No simulation found having id {dto.Id}");
                }

                if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ =>
                    _.Id == dto.Id && _.SimulationUserJoins.Any(__ => __.User.Username == userInfo.Sub && __.CanModify)))
                {
                    throw new UnauthorizedAccessException("You are not authorized to modify this simulation.");
                }
            }

            UpsertBudgetPriorityLibrary(dto, simulationId, userInfo);
            UpsertOrDeleteBudgetPriorities(dto.BudgetPriorities, dto.Id, userInfo);
        }

        public void UpsertBudgetPriorityLibrary(BudgetPriorityLibraryDTO dto, Guid simulationId, UserInfoDTO userInfo)
        {
            var userEntity = _unitOfDataPersistenceWork.Context.User.SingleOrDefault(_ => _.Username == userInfo.Sub);

            var budgetPriorityLibraryEntity = dto.ToEntity();

            _unitOfDataPersistenceWork.Context.Upsert(budgetPriorityLibraryEntity, dto.Id, userEntity?.Id);

            if (simulationId != Guid.Empty)
            {
                if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
                {
                    throw new RowNotInTableException($"No simulation found having id {simulationId}.");
                }

                _unitOfDataPersistenceWork.Context.Delete<BudgetPriorityLibrarySimulationEntity>(_ => _.SimulationId == simulationId);

                _unitOfDataPersistenceWork.Context.AddEntity(new BudgetPriorityLibrarySimulationEntity
                {
                    BudgetPriorityLibraryId = dto.Id,
                    SimulationId = simulationId
                }, userEntity?.Id);
            }
        }

        public void UpsertOrDeleteBudgetPriorities(List<BudgetPriorityDTO> budgetPriorities, Guid libraryId, UserInfoDTO userInfo)
        {
            if (!_unitOfDataPersistenceWork.Context.BudgetPriorityLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException($"No budget priority library found having id {libraryId}.");
            }

            var userEntity = _unitOfDataPersistenceWork.Context.User.SingleOrDefault(_ => _.Username == userInfo.Sub);

            var budgetPriorityEntities = budgetPriorities
                .Select(_ => _.ToEntity(libraryId))
                .ToList();

            var entityIds = budgetPriorityEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfDataPersistenceWork.Context.BudgetPriority
                .Where(_ => _.BudgetPriorityLibraryId == libraryId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            var predicatesPerCrudOperation = new Dictionary<string, Expression<Func<BudgetPriorityEntity, bool>>>
            {
                {"delete", _ => _.BudgetPriorityLibraryId == libraryId && !entityIds.Contains(_.Id)},
                {"update", _ => existingEntityIds.Contains(_.Id)},
                {"add", _ => !existingEntityIds.Contains(_.Id)}
            };

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.UpsertOrDelete(budgetPriorityEntities, predicatesPerCrudOperation, userEntity?.Id);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkUpsertOrDelete(budgetPriorityEntities, predicatesPerCrudOperation, userEntity?.Id);
            }

            _unitOfDataPersistenceWork.Context.DeleteAll<CriterionLibraryBudgetPriorityEntity>(_ =>
                _.BudgetPriority.BudgetPriorityLibraryId == libraryId);

            if (budgetPriorities.Any(_ => _.BudgetPercentagePairs.Any()))
            {
                var percentagePairsPerPriorityId = budgetPriorities.Where(_ => _.BudgetPercentagePairs.Any())
                    .ToDictionary(_ => _.Id, _ => _.BudgetPercentagePairs);
                _unitOfDataPersistenceWork.BudgetPercentagePairRepo.UpsertOrDeleteBudgetPercentagePairs(percentagePairsPerPriorityId, libraryId, userEntity?.Id);
            }

            if (budgetPriorities.Any(_ =>
                _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
            {
                var criterionJoinsToAdd = budgetPriorities
                    .Where(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression))
                    .Select(_ => new CriterionLibraryBudgetPriorityEntity
                    {
                        CriterionLibraryId = _.CriterionLibrary.Id,
                        BudgetPriorityId = _.Id
                    })
                    .ToList();

                _unitOfDataPersistenceWork.Context.BulkAddAll(criterionJoinsToAdd, userEntity?.Id);
            }
        }

        public void DeleteBudgetPriorityLibrary(Guid libraryId)
        {
            if (!_unitOfDataPersistenceWork.Context.BudgetPriorityLibrary.Any(_ => _.Id == libraryId))
            {
                return;
            }

            _unitOfDataPersistenceWork.Context.Delete<BudgetPriorityLibraryEntity>(_ => _.Id == libraryId);
        }
    }
}
