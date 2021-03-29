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
    public class BudgetPriorityRepository : IBudgetPriorityRepository
    {
        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfWork;

        public BudgetPriorityRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateBudgetPriorityLibrary(string name, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var budgetPriorityLibraryEntity = new BudgetPriorityLibraryEntity { Id = Guid.NewGuid(), Name = name };

            _unitOfWork.Context.AddEntity(budgetPriorityLibraryEntity);

            _unitOfWork.Context.AddEntity(new BudgetPriorityLibrarySimulationEntity
            {
                BudgetPriorityLibraryId = budgetPriorityLibraryEntity.Id,
                SimulationId = simulationId
            });
        }

        public void CreateBudgetPriorities(List<BudgetPriority> budgetPriorities, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var simulationEntity = _unitOfWork.Context.Simulation
                .Include(_ => _.BudgetPriorityLibrarySimulationJoin)
                .Single(_ => _.Id == simulationId);

            if (simulationEntity.BudgetPriorityLibrarySimulationJoin == null)
            {
                throw new RowNotInTableException($"No budget priority library found for simulation having id {simulationId}");
            }

            var budgetPriorityEntities = budgetPriorities
                .Select(_ => _.ToEntity(simulationEntity.BudgetPriorityLibrarySimulationJoin.BudgetPriorityLibraryId))
                .ToList();

            _unitOfWork.Context.AddAll(budgetPriorityEntities);

            if (budgetPriorities.Any(_ => !_.Criterion.ExpressionIsBlank))
            {
                var priorityIdsPerExpression = budgetPriorities
                    .Where(_ => !_.Criterion.ExpressionIsBlank)
                    .GroupBy(_ => _.Criterion.Expression, _ => _)
                    .ToDictionary(_ => _.Key, _ => _.Select(__ => __.Id).ToList());

                _unitOfWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(priorityIdsPerExpression,
                    DataPersistenceConstants.CriterionLibraryJoinEntities.BudgetPriority, simulationEntity.Name);
            }

            if (budgetPriorities.Any(_ => _.BudgetPercentagePairs.Any()))
            {
                var percentagePairPerBudgetIdPerPriorityId = budgetPriorities
                    .Where(_ => _.BudgetPercentagePairs.Any())
                    .ToDictionary(_ => _.Id, _ => _.BudgetPercentagePairs.Select(__ => (__.Budget.Id, __)).ToList());

                _unitOfWork.BudgetPercentagePairRepo.CreateBudgetPercentagePairs(percentagePairPerBudgetIdPerPriorityId);
            }
        }

        public List<BudgetPriorityLibraryDTO> BudgetPriorityLibrariesWithBudgetPriorities()
        {
            if (!_unitOfWork.Context.BudgetPriorityLibrary.Any())
            {
                return new List<BudgetPriorityLibraryDTO>();
            }

            return _unitOfWork.Context.BudgetPriorityLibrary
                .Include(_ => _.BudgetPriorities)
                .ThenInclude(_ => _.CriterionLibraryBudgetPriorityJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.BudgetPriorities)
                .ThenInclude(_ => _.BudgetPercentagePairs)
                .ThenInclude(_ => _.Budget)
                .Include(_ => _.BudgetPriorityLibrarySimulationJoins)
                .Select(_ => _.ToDto())
                .ToList();
        }

        public void UpsertPermitted(Guid simulationId, BudgetPriorityLibraryDTO dto)
        {
            if (simulationId != Guid.Empty)
            {
                if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
                {
                    throw new RowNotInTableException($"No simulation found having id {dto.Id}");
                }

                if (!_unitOfWork.Context.Simulation.Any(_ =>
                    _.Id == dto.Id && _.SimulationUserJoins.Any(__ => __.UserId == _unitOfWork.UserEntity.Id && __.CanModify)))
                {
                    throw new UnauthorizedAccessException("You are not authorized to modify this simulation.");
                }
            }

            UpsertBudgetPriorityLibrary(dto, simulationId);
            UpsertOrDeleteBudgetPriorities(dto.BudgetPriorities, dto.Id);
        }

        public void UpsertBudgetPriorityLibrary(BudgetPriorityLibraryDTO dto, Guid simulationId)
        {
            var budgetPriorityLibraryEntity = dto.ToEntity();

            _unitOfWork.Context.Upsert(budgetPriorityLibraryEntity, dto.Id, _unitOfWork.UserEntity?.Id);

            if (simulationId != Guid.Empty)
            {
                if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
                {
                    throw new RowNotInTableException($"No simulation found having id {simulationId}.");
                }

                _unitOfWork.Context.DeleteEntity<BudgetPriorityLibrarySimulationEntity>(_ => _.SimulationId == simulationId);

                _unitOfWork.Context.AddEntity(new BudgetPriorityLibrarySimulationEntity
                {
                    BudgetPriorityLibraryId = dto.Id,
                    SimulationId = simulationId
                }, _unitOfWork.UserEntity?.Id);
            }
        }

        public void UpsertOrDeleteBudgetPriorities(List<BudgetPriorityDTO> budgetPriorities, Guid libraryId)
        {
            if (!_unitOfWork.Context.BudgetPriorityLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException($"No budget priority library found having id {libraryId}.");
            }

            var budgetPriorityEntities = budgetPriorities
                .Select(_ => _.ToEntity(libraryId))
                .ToList();

            var entityIds = budgetPriorityEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.BudgetPriority
                .Where(_ => _.BudgetPriorityLibraryId == libraryId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            _unitOfWork.Context.DeleteAll<BudgetPriorityEntity>(_ =>
                _.BudgetPriorityLibraryId == libraryId && !entityIds.Contains(_.Id));

            _unitOfWork.Context.UpdateAll(budgetPriorityEntities.Where(_ => existingEntityIds.Contains(_.Id)).ToList());

            _unitOfWork.Context.AddAll(budgetPriorityEntities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList());

            _unitOfWork.Context.DeleteAll<CriterionLibraryBudgetPriorityEntity>(_ =>
                _.BudgetPriority.BudgetPriorityLibraryId == libraryId);

            if (budgetPriorities.Any(_ => _.BudgetPercentagePairs.Any()))
            {
                var percentagePairsPerPriorityId = budgetPriorities.Where(_ => _.BudgetPercentagePairs.Any())
                    .ToDictionary(_ => _.Id, _ => _.BudgetPercentagePairs);
                _unitOfWork.BudgetPercentagePairRepo.UpsertOrDeleteBudgetPercentagePairs(percentagePairsPerPriorityId, libraryId);
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

                _unitOfWork.Context.AddAll(criterionJoinsToAdd, _unitOfWork.UserEntity?.Id);
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
    }
}
