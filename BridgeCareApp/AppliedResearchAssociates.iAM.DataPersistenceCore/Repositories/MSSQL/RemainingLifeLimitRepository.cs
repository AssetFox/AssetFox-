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
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class RemainingLifeLimitRepository : IRemainingLifeLimitRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public RemainingLifeLimitRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfDataPersistenceWork)
        {
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
        }

        public void CreateRemainingLifeLimitLibrary(string name, Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var remainingLifeLimitLibraryEntity = new RemainingLifeLimitLibraryEntity { Id = Guid.NewGuid(), Name = name };

            _unitOfDataPersistenceWork.Context.RemainingLifeLimitLibrary.Add(remainingLifeLimitLibraryEntity);

            _unitOfDataPersistenceWork.Context.RemainingLifeLimitLibrarySimulation.Add(new RemainingLifeLimitLibrarySimulationEntity
            {
                RemainingLifeLimitLibraryId = remainingLifeLimitLibraryEntity.Id,
                SimulationId = simulationId
            });
        }

        public void CreateRemainingLifeLimits(List<RemainingLifeLimit> remainingLifeLimits, Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var simulationEntity = _unitOfDataPersistenceWork.Context.Simulation
                .Include(_ => _.RemainingLifeLimitLibrarySimulationJoin)
                .Single(_ => _.Id == simulationId);

            var attributeNames = remainingLifeLimits.Select(_ => _.Attribute.Name).Distinct().ToList();
            var attributeEntities = _unitOfDataPersistenceWork.Context.Attribute
                .Where(_ => attributeNames.Contains(_.Name)).ToList();

            if (!attributeEntities.Any())
            {
                throw new RowNotInTableException("Could not find matching attributes for given performance curves.");
            }

            var attributeNamesFromDataSource = attributeEntities.Select(_ => _.Name).ToList();
            if (!attributeNames.All(ruleName => attributeNamesFromDataSource.Contains(ruleName)))
            {
                var attributeNamesNotFound = attributeNames.Except(attributeNamesFromDataSource).ToList();
                if (attributeNamesNotFound.Count() == 1)
                {
                    throw new RowNotInTableException($"No attribute found having name {attributeNamesNotFound[0]}.");
                }

                throw new RowNotInTableException(
                    $"No attributes found found having names: {string.Join(", ", attributeNamesNotFound)}.");
            }

            var remainingLifeLimitEntities = remainingLifeLimits
                .Select(_ => _.ToEntity(simulationEntity.RemainingLifeLimitLibrarySimulationJoin.RemainingLifeLimitLibraryId,
                    attributeEntities.Single(__ => __.Name == _.Attribute.Name).Id))
                .ToList();

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.RemainingLifeLimit.AddRange(remainingLifeLimitEntities);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkInsert(remainingLifeLimitEntities);
            }

            if (remainingLifeLimits.Any(_ => !_.Criterion.ExpressionIsBlank))
            {
                var limitIdsPerExpression = remainingLifeLimits
                    .Where(_ => !_.Criterion.ExpressionIsBlank)
                    .GroupBy(_ => _.Criterion.Expression, _ => _)
                    .ToDictionary(_ => _.Key, _ => _.Select(__ => __.Id).ToList());

                _unitOfDataPersistenceWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(limitIdsPerExpression,
                    DataPersistenceConstants.CriterionLibraryJoinEntities.RemainingLifeLimit, simulationEntity.Name);
            }
        }

        public Task<List<RemainingLifeLimitLibraryDTO>> RemainingLifeLimitLibrariesWithRemainingLifeLimits()
        {
            if (!_unitOfDataPersistenceWork.Context.RemainingLifeLimitLibrary.Any())
            {
                return Task.Factory.StartNew(() => new List<RemainingLifeLimitLibraryDTO>());
            }

            return Task.Factory.StartNew(() => _unitOfDataPersistenceWork.Context.RemainingLifeLimitLibrary
                .Include(_ => _.RemainingLifeLimits)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.RemainingLifeLimits)
                .ThenInclude(_ => _.CriterionLibraryRemainingLifeLimitJoin)
                .Include(_ => _.RemainingLifeLimitLibrarySimulationJoins)
                .Select(_ => _.ToDto())
                .ToList());
        }

        public void AddOrUpdateRemainingLifeLimitLibrary(RemainingLifeLimitLibraryDTO dto, Guid simulationId)
        {
            var remainingLifeLimitLibraryEntity = dto.ToEntity();

            _unitOfDataPersistenceWork.Context.AddOrUpdate(remainingLifeLimitLibraryEntity, dto.Id);

            if (simulationId != Guid.Empty)
            {
                if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
                {
                    throw new RowNotInTableException($"No simulation found having id {simulationId}.");
                }

                _unitOfDataPersistenceWork.Context.Delete<RemainingLifeLimitLibrarySimulationEntity>(_ => _.SimulationId == simulationId);

                _unitOfDataPersistenceWork.Context.RemainingLifeLimitLibrarySimulation.Add(
                    new RemainingLifeLimitLibrarySimulationEntity
                    {
                        RemainingLifeLimitLibraryId = remainingLifeLimitLibraryEntity.Id,
                        SimulationId = simulationId
                    });
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        public void AddOrUpdateOrDeleteRemainingLifeLimits(List<RemainingLifeLimitDTO> remainingLifeLimits,
            Guid libraryId)
        {
            if (!_unitOfDataPersistenceWork.Context.RemainingLifeLimitLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException($"No remaining life limit library found having id {libraryId}.");
            }

            if (remainingLifeLimits.Any(_ => string.IsNullOrEmpty(_.Attribute)))
            {
                throw new InvalidOperationException("All remaining life limits must have an attribute.");
            }

            var attributeEntities = _unitOfDataPersistenceWork.Context.Attribute.ToList();
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

            var existingEntityIds = _unitOfDataPersistenceWork.Context.RemainingLifeLimit
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
                _unitOfDataPersistenceWork.Context.AddOrUpdateOrDelete(remainingLifeLimitEntities, predicatesPerCrudOperation);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkAddOrUpdateOrDelete(remainingLifeLimitEntities, predicatesPerCrudOperation);
            }

            _unitOfDataPersistenceWork.Context.DeleteAll<CriterionLibraryRemainingLifeLimitEntity>(_ =>
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

                if (IsRunningFromXUnit)
                {
                    _unitOfDataPersistenceWork.Context.CriterionLibraryRemainingLifeLimit.AddRange(criterionLibraryJoinsToAdd);
                }
                else
                {
                    _unitOfDataPersistenceWork.Context.BulkInsert(criterionLibraryJoinsToAdd);
                }
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        public void DeleteRemainingLifeLimitLibrary(Guid libraryId)
        {
            if (!_unitOfDataPersistenceWork.Context.RemainingLifeLimitLibrary.Any(_ => _.Id == libraryId))
            {
                return;
            }

            var libraryToDelete =
                _unitOfDataPersistenceWork.Context.RemainingLifeLimitLibrary.Single(_ => _.Id == libraryId);

            // deleting the library should start a delete cascade where the remaining life limits
            // will be deleted along with any join table records
            _unitOfDataPersistenceWork.Context.RemainingLifeLimitLibrary.Remove(libraryToDelete);

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }
    }
}
