using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class TargetConditionGoalRepository : ITargetConditionGoalRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public TargetConditionGoalRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateTargetConditionGoalLibrary(string name, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation found for given scenario.");
            }

            var targetConditionGoalLibraryEntity = new TargetConditionGoalLibraryEntity { Id = Guid.NewGuid(), Name = name };

            _unitOfWork.Context.AddEntity(targetConditionGoalLibraryEntity);

            _unitOfWork.Context.AddEntity(new TargetConditionGoalLibrarySimulationEntity
            {
                TargetConditionGoalLibraryId = targetConditionGoalLibraryEntity.Id,
                SimulationId = simulationId
            });

            // Update last modified date
            var simulationEntity = _unitOfWork.Context.Simulation.Where(_ => _.Id == simulationId).FirstOrDefault();
            _unitOfWork.SimulationRepo.UpdateLastModifiedDate(simulationEntity);
        }

        public void CreateTargetConditionGoals(List<TargetConditionGoal> targetConditionGoals, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation found for given scenario.");
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
            // Update last modified date
            _unitOfWork.SimulationRepo.UpdateLastModifiedDate(simulationEntity);
        }

        public List<TargetConditionGoalLibraryDTO> TargetConditionGoalLibrariesWithTargetConditionGoals()
        {
            if (!_unitOfWork.Context.TargetConditionGoalLibrary.Any())
            {
                return new List<TargetConditionGoalLibraryDTO>();
            }

            return _unitOfWork.Context.TargetConditionGoalLibrary
                .Include(_ => _.TargetConditionGoals)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.TargetConditionGoals)
                .ThenInclude(_ => _.CriterionLibraryTargetConditionGoalJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.TargetConditionGoalLibrarySimulationJoins)
                .Select(_ => _.ToDto())
                .ToList();
        }

        public void UpsertTargetConditionGoalLibrary(TargetConditionGoalLibraryDTO dto, Guid simulationId)
        {
            var targetConditionGoalLibraryEntity = dto.ToEntity();

            _unitOfWork.Context.Upsert(targetConditionGoalLibraryEntity, dto.Id, _unitOfWork.UserEntity?.Id);

            if (simulationId != Guid.Empty)
            {
                if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
                {
                    throw new RowNotInTableException("No simulation was found for the given scenario.");
                }

                _unitOfWork.Context.DeleteEntity<TargetConditionGoalLibrarySimulationEntity>(_ =>
                    _.SimulationId == simulationId);

                _unitOfWork.Context.AddEntity(
                    new TargetConditionGoalLibrarySimulationEntity
                    {
                        TargetConditionGoalLibraryId = dto.Id,
                        SimulationId = simulationId
                    }, _unitOfWork.UserEntity?.Id);

                // Update last modified date
                var simulationEntity = _unitOfWork.Context.Simulation.Where(_ => _.Id == simulationId).FirstOrDefault();
                _unitOfWork.SimulationRepo.UpdateLastModifiedDate(simulationEntity);
            }
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
                .Select(_ => _.ToEntity(libraryId, attributeEntities.Single(__ => __.Name == _.Attribute).Id)).ToList();

            var entityIds = targetConditionGoalEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.TargetConditionGoal
                .Where(_ => _.TargetConditionGoalLibraryId == libraryId && entityIds.Contains(_.Id)).Select(_ => _.Id)
                .ToList();

            _unitOfWork.Context.DeleteAll<TargetConditionGoalEntity>(_ =>
                _.TargetConditionGoalLibraryId == libraryId && !entityIds.Contains(_.Id));

            _unitOfWork.Context.UpdateAll(targetConditionGoalEntities.Where(_ => existingEntityIds.Contains(_.Id))
                .ToList(), _unitOfWork.UserEntity?.Id);

            _unitOfWork.Context.AddAll(targetConditionGoalEntities.Where(_ => !existingEntityIds.Contains(_.Id))
                .ToList(), _unitOfWork.UserEntity?.Id);

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

                _unitOfWork.Context.AddAll(criterionLibraryJoinsToAdd, _unitOfWork.UserEntity?.Id);
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
