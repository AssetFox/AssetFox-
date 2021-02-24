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
    public class TargetConditionGoalRepository : ITargetConditionGoalRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public TargetConditionGoalRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfDataPersistenceWork)
        {
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
        }

        public void CreateTargetConditionGoalLibrary(string name, Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var targetConditionGoalLibraryEntity = new TargetConditionGoalLibraryEntity { Id = Guid.NewGuid(), Name = name };

            _unitOfDataPersistenceWork.Context.TargetConditionGoalLibrary.Add(targetConditionGoalLibraryEntity);

            _unitOfDataPersistenceWork.Context.TargetConditionGoalLibrarySimulation.Add(new TargetConditionGoalLibrarySimulationEntity
            {
                TargetConditionGoalLibraryId = targetConditionGoalLibraryEntity.Id, SimulationId = simulationId
            });

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        public void CreateTargetConditionGoals(List<TargetConditionGoal> targetConditionGoals, Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var simulationEntity = _unitOfDataPersistenceWork.Context.Simulation
                .Include(_ => _.TargetConditionGoalLibrarySimulationJoin)
                .Single(_ => _.Id == simulationId);

            var attributeNames = targetConditionGoals.Select(_ => _.Attribute.Name).Distinct().ToList();
            var attributeEntities = _unitOfDataPersistenceWork.Context.Attribute.Where(_ => attributeNames.Contains(_.Name)).ToList();

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

            var targetConditionGoalEntities = targetConditionGoals
                .Select(_ => _.ToEntity(simulationEntity.TargetConditionGoalLibrarySimulationJoin.TargetConditionGoalLibraryId,
                    attributeEntities.Single(__ => __.Name == _.Attribute.Name).Id))
                .ToList();

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.TargetConditionGoal.AddRange(targetConditionGoalEntities);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkInsert(targetConditionGoalEntities);
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();

            if (targetConditionGoals.Any(_ => !_.Criterion.ExpressionIsBlank))
            {
                var targetIdsPerExpression = targetConditionGoals
                    .Where(_ => !_.Criterion.ExpressionIsBlank)
                    .GroupBy(_ => _.Criterion.Expression, _ => _)
                    .ToDictionary(_ => _.Key, _ => _.Select(__ => __.Id).ToList());

                _unitOfDataPersistenceWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(targetIdsPerExpression, "TargetConditionGoalEntity", simulationEntity.Name);
            }
        }

        public Task<List<TargetConditionGoalLibraryDTO>> TargetConditionGoalLibrariesWithTargetConditionGoals()
        {
            if (!_unitOfDataPersistenceWork.Context.TargetConditionGoalLibrary.Any())
            {
                return Task.Factory.StartNew(() => new List<TargetConditionGoalLibraryDTO>());
            }

            return Task.Factory.StartNew(() => _unitOfDataPersistenceWork.Context.TargetConditionGoalLibrary
                .Include(_ => _.TargetConditionGoals)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.TargetConditionGoals)
                .ThenInclude(_ => _.CriterionLibraryTargetConditionGoalJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.TargetConditionGoalLibrarySimulationJoins)
                .Select(_ => _.ToDto())
                .ToList());
        }

        public void AddOrUpdateTargetConditionGoalLibrary(TargetConditionGoalLibraryDTO dto, Guid simulationId)
        {
            var entity = dto.ToEntity();

            _unitOfDataPersistenceWork.Context.AddOrUpdate(entity, dto.Id);

            if (simulationId != Guid.Empty)
            {
                if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
                {
                    throw new RowNotInTableException($"No simulation found having id {simulationId}.");
                }

                _unitOfDataPersistenceWork.Context.Delete<TargetConditionGoalLibrarySimulationEntity>(_ => _.SimulationId == simulationId);

                _unitOfDataPersistenceWork.Context.TargetConditionGoalLibrarySimulation.Add(
                    new TargetConditionGoalLibrarySimulationEntity
                    {
                        TargetConditionGoalLibraryId = dto.Id, SimulationId = simulationId
                    });
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        public void AddOrUpdateOrDeleteTargetConditionGoals(List<TargetConditionGoalDTO> targetConditionGoals,
            Guid libraryId)
        {
            if (!_unitOfDataPersistenceWork.Context.TargetConditionGoalLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException($"No target condition goal library found having id {libraryId}.");
            }

            if (targetConditionGoals.Any(_ => string.IsNullOrEmpty(_.Attribute)))
            {
                throw new InvalidOperationException("All target condition goals must have an attribute.");
            }

            var attributeEntities = _unitOfDataPersistenceWork.Context.Attribute.ToList();
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

            var entities = targetConditionGoals
                .Select(_ => _.ToEntity(libraryId, attributeEntities.Single(__ => __.Name == _.Attribute).Id)).ToList();

            var entityIds = entities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfDataPersistenceWork.Context.TargetConditionGoal
                .Where(_ => _.TargetConditionGoalLibraryId == libraryId && entityIds.Contains(_.Id)).Select(_ => _.Id)
                .ToList();

            var predicatesPerCrudOperation = new Dictionary<string, Expression<Func<TargetConditionGoalEntity, bool>>>
            {
                {"delete", _ => _.TargetConditionGoalLibraryId == libraryId && !entityIds.Contains(_.Id)},
                {"update", _ => existingEntityIds.Contains(_.Id)},
                {"add", _ => !existingEntityIds.Contains(_.Id)}
            };

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.AddOrUpdateOrDelete(entities, predicatesPerCrudOperation);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkAddOrUpdateOrDelete(entities, predicatesPerCrudOperation);
            }

            _unitOfDataPersistenceWork.Context.DeleteAll<CriterionLibraryTargetConditionGoalEntity>(_ =>
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

                if (IsRunningFromXUnit)
                {
                    _unitOfDataPersistenceWork.Context.CriterionLibraryTargetConditionGoal.AddRange(criterionLibraryJoinsToAdd);
                }
                else
                {
                    _unitOfDataPersistenceWork.Context.BulkInsert(criterionLibraryJoinsToAdd);
                }
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        public void DeleteTargetConditionGoalLibrary(Guid libraryId)
        {
            if (!_unitOfDataPersistenceWork.Context.TargetConditionGoalLibrary.Any(_ => _.Id == libraryId))
            {
                return;
            }

            var libraryToDelete =
                _unitOfDataPersistenceWork.Context.TargetConditionGoalLibrary.Single(_ => _.Id == libraryId);

            _unitOfDataPersistenceWork.Context.TargetConditionGoalLibrary.Remove(libraryToDelete);

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }
    }
}
