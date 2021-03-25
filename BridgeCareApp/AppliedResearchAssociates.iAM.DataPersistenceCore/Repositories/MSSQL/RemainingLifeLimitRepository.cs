using System;
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
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class RemainingLifeLimitRepository : IRemainingLifeLimitRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfWork;

        public RemainingLifeLimitRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public void CreateRemainingLifeLimitLibrary(string name, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var remainingLifeLimitLibraryEntity = new RemainingLifeLimitLibraryEntity { Id = Guid.NewGuid(), Name = name };

            _unitOfWork.Context.AddEntity(remainingLifeLimitLibraryEntity);

            _unitOfWork.Context.AddEntity(new RemainingLifeLimitLibrarySimulationEntity
            {
                RemainingLifeLimitLibraryId = remainingLifeLimitLibraryEntity.Id,
                SimulationId = simulationId
            });
        }

        public void CreateRemainingLifeLimits(List<RemainingLifeLimit> remainingLifeLimits, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
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
        }

        public Task<List<RemainingLifeLimitLibraryDTO>> RemainingLifeLimitLibrariesWithRemainingLifeLimits()
        {
            if (!_unitOfWork.Context.RemainingLifeLimitLibrary.Any())
            {
                return Task.Factory.StartNew(() => new List<RemainingLifeLimitLibraryDTO>());
            }

            return Task.Factory.StartNew(() => _unitOfWork.Context.RemainingLifeLimitLibrary
                .Include(_ => _.RemainingLifeLimits)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.RemainingLifeLimits)
                .ThenInclude(_ => _.CriterionLibraryRemainingLifeLimitJoin)
                .Include(_ => _.RemainingLifeLimitLibrarySimulationJoins)
                .Select(_ => _.ToDto())
                .ToList());
        }

        public void UpsertRemainingLifeLimitLibrary(RemainingLifeLimitLibraryDTO dto, Guid simulationId, UserInfoDTO userInfo)
        {
            var userEntity = _unitOfWork.Context.User.SingleOrDefault(_ => _.Username == userInfo.Sub);

            var remainingLifeLimitLibraryEntity = dto.ToEntity();

            _unitOfWork.Context.Upsert(remainingLifeLimitLibraryEntity, dto.Id, userEntity?.Id);

            if (simulationId != Guid.Empty)
            {
                if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
                {
                    throw new RowNotInTableException($"No simulation found having id {simulationId}.");
                }

                _unitOfWork.Context.DeleteEntity<RemainingLifeLimitLibrarySimulationEntity>(_ =>
                    _.SimulationId == simulationId);

                _unitOfWork.Context.AddEntity(
                    new RemainingLifeLimitLibrarySimulationEntity
                    {
                        RemainingLifeLimitLibraryId = remainingLifeLimitLibraryEntity.Id,
                        SimulationId = simulationId
                    }, userEntity?.Id);
            }
        }

        public void UpsertOrDeleteRemainingLifeLimits(List<RemainingLifeLimitDTO> remainingLifeLimits,
            Guid libraryId, UserInfoDTO userInfo)
        {
            if (!_unitOfWork.Context.RemainingLifeLimitLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException($"No remaining life limit library found having id {libraryId}.");
            }

            if (remainingLifeLimits.Any(_ => string.IsNullOrEmpty(_.Attribute)))
            {
                throw new InvalidOperationException("All remaining life limits must have an attribute.");
            }

            var userEntity = _unitOfWork.Context.User.SingleOrDefault(_ => _.Username == userInfo.Sub);

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

            var predicatesPerCrudOperation = new Dictionary<string, Expression<Func<RemainingLifeLimitEntity, bool>>>
            {
                {"delete", _ => _.RemainingLifeLimitLibraryId == libraryId && !entityIds.Contains(_.Id)},
                {"update", _ => existingEntityIds.Contains(_.Id)},
                {"add", _ => !existingEntityIds.Contains(_.Id)}
            };

            if (IsRunningFromXUnit)
            {
                _unitOfWork.Context.UpsertOrDelete(remainingLifeLimitEntities, predicatesPerCrudOperation, userEntity?.Id);
            }
            else
            {
                _unitOfWork.Context.BulkUpsertOrDelete(remainingLifeLimitEntities, predicatesPerCrudOperation, userEntity?.Id);
            }

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

                _unitOfWork.Context.AddAll(criterionLibraryJoinsToAdd, userEntity?.Id);
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
