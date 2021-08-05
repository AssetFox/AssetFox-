﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class RemainingLifeLimitRepository : IRemainingLifeLimitRepository
    {
        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfWork;

        public RemainingLifeLimitRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateRemainingLifeLimitLibrary(string name, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found for given scenario.");
            }

            var remainingLifeLimitLibraryEntity = new RemainingLifeLimitLibraryEntity { Id = Guid.NewGuid(), Name = name };

            _unitOfWork.Context.AddEntity(remainingLifeLimitLibraryEntity);

            _unitOfWork.Context.AddEntity(new RemainingLifeLimitLibrarySimulationEntity
            {
                RemainingLifeLimitLibraryId = remainingLifeLimitLibraryEntity.Id,
                SimulationId = simulationId
            });

            // Update last modified date
            var simulationEntity = _unitOfWork.Context.Simulation.Where(_ => _.Id == simulationId).FirstOrDefault();
            _unitOfWork.SimulationRepo.UpdateLastModifiedDate(simulationEntity);
        }

        public void CreateRemainingLifeLimits(List<RemainingLifeLimit> remainingLifeLimits, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found for given scenario.");
            }

            var simulationEntity = _unitOfWork.Context.Simulation
                .Include(_ => _.RemainingLifeLimitLibrarySimulationJoin)
                .Single(_ => _.Id == simulationId);

            var attributeEntities = _unitOfWork.Context.Attribute.ToList();
            var attributeNames = attributeEntities.Select(_ => _.Name).ToList();
            if (!remainingLifeLimits.All(_ => attributeNames.Contains(_.Attribute.Name)))
            {
                var missingAttributes = remainingLifeLimits.Select(_ => _.Attribute.Name)
                    .Except(attributeNames).ToList();
                if (missingAttributes.Count == 1)
                {
                    throw new RowNotInTableException($"No attribute found having name {missingAttributes[0]}.");
                }

                throw new RowNotInTableException(
                    $"No attributes found having the names: {string.Join(", ", missingAttributes)}.");
            }

            var remainingLifeLimitEntities = remainingLifeLimits
                .Select(_ => _.ToEntity(simulationEntity.RemainingLifeLimitLibrarySimulationJoin.RemainingLifeLimitLibraryId,
                    attributeEntities.Single(__ => __.Name == _.Attribute.Name).Id))
                .ToList();

            _unitOfWork.Context.AddAll(remainingLifeLimitEntities);

            if (remainingLifeLimits.Any(_ => !_.Criterion.ExpressionIsBlank))
            {
                var limitIdsPerExpression = remainingLifeLimits
                    .Where(_ => !_.Criterion.ExpressionIsBlank)
                    .GroupBy(_ => _.Criterion.Expression, _ => _)
                    .ToDictionary(_ => _.Key, _ => _.Select(__ => __.Id).ToList());

                _unitOfWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(limitIdsPerExpression,
                    DataPersistenceConstants.CriterionLibraryJoinEntities.RemainingLifeLimit, simulationEntity.Name);
            }

            // Update last modified date
            _unitOfWork.SimulationRepo.UpdateLastModifiedDate(simulationEntity);
        }

        public List<RemainingLifeLimitLibraryDTO> RemainingLifeLimitLibrariesWithRemainingLifeLimits()
        {
            if (!_unitOfWork.Context.RemainingLifeLimitLibrary.Any())
            {
                return new List<RemainingLifeLimitLibraryDTO>();
            }

            return _unitOfWork.Context.RemainingLifeLimitLibrary
                .Include(_ => _.RemainingLifeLimits)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.RemainingLifeLimits)
                .ThenInclude(_ => _.CriterionLibraryRemainingLifeLimitJoin)
                .Include(_ => _.RemainingLifeLimitLibrarySimulationJoins)
                .Select(_ => _.ToDto())
                .ToList();
        }

        public void UpsertRemainingLifeLimitLibrary(RemainingLifeLimitLibraryDTO dto, Guid simulationId)
        {
            var remainingLifeLimitLibraryEntity = dto.ToEntity();

            _unitOfWork.Context.Upsert(remainingLifeLimitLibraryEntity, dto.Id, _unitOfWork.UserEntity?.Id);

            if (simulationId != Guid.Empty)
            {
                if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
                {
                    throw new RowNotInTableException($"No simulation found for the given scenario.");
                }

                _unitOfWork.Context.DeleteEntity<RemainingLifeLimitLibrarySimulationEntity>(_ =>
                    _.SimulationId == simulationId);

                _unitOfWork.Context.AddEntity(
                    new RemainingLifeLimitLibrarySimulationEntity
                    {
                        RemainingLifeLimitLibraryId = remainingLifeLimitLibraryEntity.Id,
                        SimulationId = simulationId
                    }, _unitOfWork.UserEntity?.Id);

                // Update last modified date
                var simulationEntity = _unitOfWork.Context.Simulation.Where(_ => _.Id == simulationId).FirstOrDefault();
                _unitOfWork.SimulationRepo.UpdateLastModifiedDate(simulationEntity);
            }
        }

        public void UpsertOrDeleteRemainingLifeLimits(List<RemainingLifeLimitDTO> remainingLifeLimits,
            Guid libraryId)
        {
            if (!_unitOfWork.Context.RemainingLifeLimitLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException($"No remaining life limit library found having id {libraryId}.");
            }

            if (remainingLifeLimits.Any(_ => string.IsNullOrEmpty(_.Attribute)))
            {
                throw new InvalidOperationException("All remaining life limits must have an attribute.");
            }

            var attributeEntities = _unitOfWork.Context.Attribute.ToList();
            var attributeNames = attributeEntities.Select(_ => _.Name).ToList();
            if (!remainingLifeLimits.All(_ => attributeNames.Contains(_.Attribute)))
            {
                var missingAttributes = remainingLifeLimits.Select(_ => _.Attribute)
                    .Except(attributeNames).ToList();
                if (missingAttributes.Count == 1)
                {
                    throw new RowNotInTableException($"No attribute found having name {missingAttributes[0]}.");
                }

                throw new RowNotInTableException(
                    $"No attributes found having the names: {string.Join(", ", missingAttributes)}.");
            }

            var remainingLifeLimitEntities = remainingLifeLimits.Select(_ =>
                _.ToEntity(libraryId, attributeEntities.Single(__ => __.Name == _.Attribute).Id)).ToList();

            var entityIds = remainingLifeLimitEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.RemainingLifeLimit
                .Where(_ => _.RemainingLifeLimitLibraryId == libraryId && entityIds.Contains(_.Id)).Select(_ => _.Id)
                .ToList();

            _unitOfWork.Context.DeleteAll<RemainingLifeLimitEntity>(_ =>
                _.RemainingLifeLimitLibraryId == libraryId && !entityIds.Contains(_.Id));

            _unitOfWork.Context.UpdateAll(remainingLifeLimitEntities.Where(_ => existingEntityIds.Contains(_.Id)).ToList());

            _unitOfWork.Context.AddAll(remainingLifeLimitEntities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList());

            _unitOfWork.Context.DeleteAll<CriterionLibraryRemainingLifeLimitEntity>(_ =>
                _.RemainingLifeLimit.RemainingLifeLimitLibraryId == libraryId);

            if (remainingLifeLimits.Any(_ =>
                _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
            {
                var criterionLibraryJoinsToAdd = remainingLifeLimits
                    .Where(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)).Select(_ =>
                        new CriterionLibraryRemainingLifeLimitEntity
                        {
                            CriterionLibraryId = _.CriterionLibrary.Id,
                            RemainingLifeLimitId = _.Id
                        }).ToList();

                _unitOfWork.Context.AddAll(criterionLibraryJoinsToAdd, _unitOfWork.UserEntity?.Id);
            }
        }

        public void DeleteRemainingLifeLimitLibrary(Guid libraryId)
        {
            if (!_unitOfWork.Context.RemainingLifeLimitLibrary.Any(_ => _.Id == libraryId))
            {
                return;
            }

            _unitOfWork.Context.DeleteEntity<RemainingLifeLimitLibraryEntity>(_ => _.Id == libraryId);
        }
    }
}
