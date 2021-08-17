using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class RemainingLifeLimitRepository : IRemainingLifeLimitRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public RemainingLifeLimitRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateRemainingLifeLimitLibrary(string name)
        {
            var remainingLifeLimitLibraryEntity = new RemainingLifeLimitLibraryEntity { Id = Guid.NewGuid(), Name = name };

            _unitOfWork.Context.AddEntity(remainingLifeLimitLibraryEntity);
        }

        public void CreateRemainingLifeLimits(List<RemainingLifeLimit> remainingLifeLimits, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation found for given scenario.");
            }

            var simulationEntity = _unitOfWork.Context.Simulation
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
                .Select(_ => _.ToScenarioEntity(simulationId,
                    attributeEntities.Single(__ => __.Name == _.Attribute.Name).Id))
                .ToList();

            _unitOfWork.Context.AddAll(remainingLifeLimitEntities);

            if (remainingLifeLimits.Any(_ => !_.Criterion.ExpressionIsBlank))
            {
                var criterionLibraryEntities = new List<CriterionLibraryEntity>();
                var criterionLibraryJoinEntities = new List<CriterionLibraryScenarioRemainingLifeLimitEntity>();

                remainingLifeLimits.Where(goal => !goal.Criterion.ExpressionIsBlank)
                    .ForEach(goal =>
                    {
                        var criterionLibraryEntity = new CriterionLibraryEntity
                        {
                            Id = Guid.NewGuid(),
                            MergedCriteriaExpression = goal.Criterion.Expression,
                            Name = $"Remaining life limit {goal.Attribute} Criterion",
                            IsSingleUse = true
                        };
                        criterionLibraryEntities.Add(criterionLibraryEntity);
                        criterionLibraryJoinEntities.Add(new CriterionLibraryScenarioRemainingLifeLimitEntity
                        {
                            CriterionLibraryId = criterionLibraryEntity.Id,
                            ScenarioRemainingLifeLimitId = goal.Id
                        });
                    });

                _unitOfWork.Context.AddAll(criterionLibraryEntities, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criterionLibraryJoinEntities, _unitOfWork.UserEntity?.Id);
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
                .ThenInclude(_ => _.CriterionLibrary)
                .Select(_ => _.ToDto())
                .ToList();
        }

        public void UpsertRemainingLifeLimitLibrary(RemainingLifeLimitLibraryDTO dto)
        {
            var remainingLifeLimitLibraryEntity = dto.ToEntity();

            _unitOfWork.Context.Upsert(remainingLifeLimitLibraryEntity, dto.Id, _unitOfWork.UserEntity?.Id);
        }

        public void UpsertOrDeleteRemainingLifeLimits(List<RemainingLifeLimitDTO> remainingLifeLimits,
            Guid libraryId)
        {
            if (!_unitOfWork.Context.RemainingLifeLimitLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException("The specified remaining life limit library was not found.");
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
                _.ToLibraryEntity(libraryId, attributeEntities.Single(__ => __.Name == _.Attribute).Id)).ToList();

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
                var criterionLibraryEntities = new List<CriterionLibraryEntity>();
                var criterionLibraryJoinEntities = new List<CriterionLibraryRemainingLifeLimitEntity>();

                remainingLifeLimits.Where(curve =>
                        curve.CriterionLibrary?.Id != null && curve.CriterionLibrary?.Id != Guid.Empty &&
                        !string.IsNullOrEmpty(curve.CriterionLibrary.MergedCriteriaExpression))
                    .ForEach(goal =>
                    {
                        var criterionLibraryEntity = new CriterionLibraryEntity
                        {
                            Id = Guid.NewGuid(),
                            MergedCriteriaExpression = goal.CriterionLibrary.MergedCriteriaExpression,
                            Name = $"Remaining life limit {goal.Attribute} Criterion",
                            IsSingleUse = true
                        };
                        criterionLibraryEntities.Add(criterionLibraryEntity);
                        criterionLibraryJoinEntities.Add(new CriterionLibraryRemainingLifeLimitEntity
                        {
                            CriterionLibraryId = criterionLibraryEntity.Id,
                            RemainingLifeLimitId = goal.Id
                        });
                    });

                _unitOfWork.Context.AddAll(criterionLibraryEntities, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criterionLibraryJoinEntities, _unitOfWork.UserEntity?.Id);
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

        public List<RemainingLifeLimitDTO> GetScenarioRemainingLifeLimits(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found for the given scenario.");
            }

            var res = _unitOfWork.Context.ScenarioRemainingLifeLimit.Where(_ => _.SimulationId == simulationId)
                .Include(_ => _.CriterionLibraryScenarioRemainingLifeLimitJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Attribute)
                .Select(_ => _.ToDto())
                .AsNoTracking()
                .ToList();
            return res;
        }
        public void UpsertOrDeleteScenarioRemainingLifeLimits(List<RemainingLifeLimitDTO> scenarioRemainingLifeLimit, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found for the given scenario.");
            }
            if (scenarioRemainingLifeLimit.Any(_ => string.IsNullOrEmpty(_.Attribute)))
            {
                throw new InvalidOperationException("All target conditions must have an attribute.");
            }
            var attributeEntities = _unitOfWork.Context.Attribute.ToList();
            var attributeNames = attributeEntities.Select(_ => _.Name).ToList();

            if (!scenarioRemainingLifeLimit.All(_ => attributeNames.Contains(_.Attribute)))
            {
                var missingAttributes = scenarioRemainingLifeLimit.Select(_ => _.Attribute)
                    .Except(attributeNames).ToList();
                if (missingAttributes.Count == 1)
                {
                    throw new RowNotInTableException($"No attribute found having name {missingAttributes[0]}.");
                }

                throw new RowNotInTableException(
                    $"No attributes found having names: {string.Join(", ", missingAttributes)}.");
            }
            var scenariRemainingLifeLimitEntities = scenarioRemainingLifeLimit
                .Select(_ =>
                    _.ToScenarioEntity(simulationId, attributeEntities.Single(__ => __.Name == _.Attribute).Id))
                .ToList();
            var entityIds = scenarioRemainingLifeLimit.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.ScenarioRemainingLifeLimit
                .Where(_ => _.SimulationId == simulationId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            _unitOfWork.Context.DeleteAll<ScenarioRemainingLifeLimitEntity>(_ =>
                _.SimulationId == simulationId && !entityIds.Contains(_.Id));

            _unitOfWork.Context.UpdateAll(scenariRemainingLifeLimitEntities.Where(_ => existingEntityIds.Contains(_.Id))
                .ToList());

            _unitOfWork.Context.AddAll(scenariRemainingLifeLimitEntities.Where(_ => !existingEntityIds.Contains(_.Id))
                .ToList());

            _unitOfWork.Context.DeleteAll<CriterionLibraryScenarioRemainingLifeLimitEntity>(_ =>
                _.ScenarioRemainingLifeLimit.SimulationId == simulationId);

            if (scenarioRemainingLifeLimit.Any(_ =>
                _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
            {
                var criterionLibraryEntities = new List<CriterionLibraryEntity>();
                var criterionLibraryJoinEntities = new List<CriterionLibraryScenarioRemainingLifeLimitEntity>();

                scenarioRemainingLifeLimit.Where(curve =>
                        curve.CriterionLibrary?.Id != null && curve.CriterionLibrary?.Id != Guid.Empty &&
                        !string.IsNullOrEmpty(curve.CriterionLibrary.MergedCriteriaExpression))
                    .ForEach(goal =>
                    {
                        var criterionLibraryEntity = new CriterionLibraryEntity
                        {
                            Id = Guid.NewGuid(),
                            MergedCriteriaExpression = goal.CriterionLibrary.MergedCriteriaExpression,
                            Name = $"Remaining life limit {goal.Attribute} Criterion",
                            IsSingleUse = true
                        };
                        criterionLibraryEntities.Add(criterionLibraryEntity);
                        criterionLibraryJoinEntities.Add(new CriterionLibraryScenarioRemainingLifeLimitEntity
                        {
                            CriterionLibraryId = criterionLibraryEntity.Id,
                            ScenarioRemainingLifeLimitId = goal.Id
                        });
                    });

                _unitOfWork.Context.AddAll(criterionLibraryEntities, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criterionLibraryJoinEntities, _unitOfWork.UserEntity?.Id);
            }
            // Update last modified date
            var simulationEntity = _unitOfWork.Context.Simulation.Single(_ => _.Id == simulationId);
            _unitOfWork.Context.Upsert(simulationEntity, simulationId, _unitOfWork.UserEntity?.Id);
        }
    }
}
