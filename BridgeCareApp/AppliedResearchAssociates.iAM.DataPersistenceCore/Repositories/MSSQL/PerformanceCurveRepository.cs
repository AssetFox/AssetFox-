using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class PerformanceCurveRepository : IPerformanceCurveRepository
    {
        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfWork;

        public PerformanceCurveRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreatePerformanceCurveLibrary(string name, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            var performanceCurveLibraryEntity = new PerformanceCurveLibraryEntity {Id = Guid.NewGuid(), Name = name};

            _unitOfWork.Context.AddEntity(performanceCurveLibraryEntity);

            _unitOfWork.Context.AddEntity(new PerformanceCurveLibrarySimulationEntity
            {
                PerformanceCurveLibraryId = performanceCurveLibraryEntity.Id, SimulationId = simulationId
            });

            // Update last modified date
            var simulationEntity = _unitOfWork.Context.Simulation.Where(_ => _.Id == simulationId).FirstOrDefault();
            _unitOfWork.SimulationRepo.UpdateLastModifiedDate(simulationEntity);
        }

        public void CreatePerformanceCurves(List<PerformanceCurve> performanceCurves, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            var simulationEntity = _unitOfWork.Context.Simulation
                .Include(_ => _.PerformanceCurveLibrarySimulationJoin)
                .Single(_ => _.Id == simulationId);

            if (simulationEntity.PerformanceCurveLibrarySimulationJoin == null)
            {
                throw new RowNotInTableException(
                    $"No performance curve library found for simulation having id {simulationId}.");
            }

            var attributeEntities = _unitOfWork.Context.Attribute.ToList();
            var attributeNames = attributeEntities.Select(_ => _.Name).ToList();
            if (!performanceCurves.All(_ => attributeNames.Contains(_.Attribute.Name)))
            {
                var missingAttributes = performanceCurves.Select(_ => _.Attribute.Name)
                    .Except(attributeNames).ToList();
                if (missingAttributes.Count == 1)
                {
                    throw new RowNotInTableException($"No attribute found having name {missingAttributes[0]}.");
                }

                throw new RowNotInTableException(
                    $"No attributes found having names: {string.Join(", ", missingAttributes)}.");
            }

            var performanceCurveEntities = performanceCurves
                .Select(_ =>
                    _.ToEntity(simulationEntity.PerformanceCurveLibrarySimulationJoin.PerformanceCurveLibraryId,
                        attributeEntities.Single(__ => __.Name == _.Attribute.Name).Id))
                .ToList();

            _unitOfWork.Context.AddAll(performanceCurveEntities);

            if (performanceCurves.Any(_ => !_.Equation.ExpressionIsBlank))
            {
                var equationEntityPerPerformanceCurveEntityId = performanceCurves
                    .Where(_ => !_.Equation.ExpressionIsBlank)
                    .ToDictionary(_ => _.Id, _ => _.Equation.ToEntity());

                _unitOfWork.EquationRepo.CreateEquations(equationEntityPerPerformanceCurveEntityId,
                    DataPersistenceConstants.EquationJoinEntities.PerformanceCurve);
            }

            if (performanceCurves.Any(_ => !_.Criterion.ExpressionIsBlank))
            {
                var curveIdsPerExpression = performanceCurves
                    .Where(_ => !_.Criterion.ExpressionIsBlank)
                    .GroupBy(_ => _.Criterion.Expression, _ => _)
                    .ToDictionary(_ => _.Key, _ => _.Select(__ => __.Id).ToList());

                _unitOfWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(curveIdsPerExpression,
                    DataPersistenceConstants.CriterionLibraryJoinEntities.PerformanceCurve, simulationEntity.Name);
            }

            // Update last modified date
            _unitOfWork.SimulationRepo.UpdateLastModifiedDate(simulationEntity);
        }

        public void SimulationPerformanceCurves(Simulation simulation)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulation.Id))
            {
                throw new RowNotInTableException($"No simulation found having id {simulation.Id}");
            }

            var simulationEntity = _unitOfWork.Context.Simulation
                .Include(_ => _.PerformanceCurveLibrarySimulationJoin)
                .ThenInclude(_ => _.PerformanceCurveLibrary)
                .ThenInclude(_ => _.PerformanceCurves)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.PerformanceCurveLibrarySimulationJoin)
                .ThenInclude(_ => _.PerformanceCurveLibrary)
                .ThenInclude(_ => _.PerformanceCurves)
                .ThenInclude(_ => _.CriterionLibraryPerformanceCurveJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.PerformanceCurveLibrarySimulationJoin)
                .ThenInclude(_ => _.PerformanceCurveLibrary)
                .ThenInclude(_ => _.PerformanceCurves)
                .ThenInclude(_ => _.PerformanceCurveEquationJoin)
                .ThenInclude(_ => _.Equation)
                .AsNoTracking()
                .Single(_ => _.Id == simulation.Id);

            simulationEntity.PerformanceCurveLibrarySimulationJoin?.PerformanceCurveLibrary?.PerformanceCurves?.ForEach(
                _ => _.CreatePerformanceCurve(simulation));
        }

        public List<PerformanceCurveLibraryDTO> GetPerformanceCurveLibrariesWithPerformanceCurves()
        {
            if (!_unitOfWork.Context.PerformanceCurveLibrary.Any())
            {
                return new List<PerformanceCurveLibraryDTO>();
            }

            return _unitOfWork.Context.PerformanceCurveLibrary
                .Include(_ => _.PerformanceCurves)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.PerformanceCurves)
                .ThenInclude(_ => _.CriterionLibraryPerformanceCurveJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.PerformanceCurves)
                .ThenInclude(_ => _.PerformanceCurveEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.PerformanceCurveLibrarySimulationJoins)
                .Select(_ => _.ToDto())
                .ToList();
        }

        public void UpsertPerformanceCurveLibrary(PerformanceCurveLibraryDTO dto)
        {
            var performanceCurveLibraryEntity = dto.ToEntity();

            _unitOfWork.Context.Upsert(performanceCurveLibraryEntity, dto.Id, _unitOfWork.UserEntity?.Id);
        }

        public void UpsertOrDeletePerformanceCurves(List<PerformanceCurveDTO> performanceCurves, Guid libraryId)
        {
            if (!_unitOfWork.Context.PerformanceCurveLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException($"No performance curve library found having id {libraryId}.");
            }

            if (performanceCurves.Any(_ => string.IsNullOrEmpty(_.Attribute)))
            {
                throw new InvalidOperationException("All performance curves must have an attribute.");
            }

            var attributeEntities = _unitOfWork.Context.Attribute.ToList();
            var attributeNames = attributeEntities.Select(_ => _.Name).ToList();
            if (!performanceCurves.All(_ => attributeNames.Contains(_.Attribute)))
            {
                var missingAttributes = performanceCurves.Select(_ => _.Attribute)
                    .Except(attributeNames).ToList();
                if (missingAttributes.Count == 1)
                {
                    throw new RowNotInTableException($"No attribute found having name {missingAttributes[0]}.");
                }

                throw new RowNotInTableException(
                    $"No attributes found having names: {string.Join(", ", missingAttributes)}.");
            }

            var performanceCurveEntities = performanceCurves
                .Select(_ => _.ToLibraryEntity(libraryId, attributeEntities.Single(__ => __.Name == _.Attribute).Id))
                .ToList();

            var entityIds = performanceCurves.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.PerformanceCurve
                .Where(_ => _.PerformanceCurveLibrary.Id == libraryId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            _unitOfWork.Context.DeleteAll<PerformanceCurveEntity>(_ =>
                _.PerformanceCurveLibrary.Id == libraryId && !entityIds.Contains(_.Id));

            _unitOfWork.Context.UpdateAll(
                performanceCurveEntities.Where(_ => existingEntityIds.Contains(_.Id)).ToList());

            _unitOfWork.Context.AddAll(performanceCurveEntities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList());

            _unitOfWork.Context.DeleteAll<EquationEntity>(_ =>
                _.PerformanceCurveEquationJoin.PerformanceCurve.PerformanceCurveLibraryId == libraryId);

            _unitOfWork.Context.DeleteAll<CriterionLibraryPerformanceCurveEntity>(_ =>
                _.PerformanceCurve.PerformanceCurveLibraryId == libraryId);

            if (performanceCurves.Any(_ =>
                _.Equation?.Id != null && _.Equation?.Id != Guid.Empty && !string.IsNullOrEmpty(_.Equation.Expression)))
            {
                var equationEntityPerPerformanceCurveEntityId = performanceCurves
                    .Where(_ => _.Equation?.Id != null && _.Equation?.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.Equation?.Expression))
                    .ToDictionary(_ => _.Id, _ => _.Equation.ToEntity());

                _unitOfWork.EquationRepo.CreateEquations(equationEntityPerPerformanceCurveEntityId,
                    DataPersistenceConstants.EquationJoinEntities.PerformanceCurve);
            }

            if (performanceCurves.Any(_ =>
                _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
            {
                var curveCriterionJoinsToAdd = performanceCurves
                    .Where(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression))
                    .Select(_ => new CriterionLibraryPerformanceCurveEntity
                    {
                        CriterionLibraryId = _.CriterionLibrary.Id, PerformanceCurveId = _.Id
                    })
                    .ToList();

                _unitOfWork.Context.AddAll(curveCriterionJoinsToAdd, _unitOfWork.UserEntity?.Id);
            }
        }

        public void DeletePerformanceCurveLibrary(Guid libraryId)
        {
            if (!_unitOfWork.Context.PerformanceCurveLibrary.Any(_ => _.Id == libraryId))
            {
                return;
            }

            _unitOfWork.Context.DeleteAll<EquationEntity>(_ =>
                _.PerformanceCurveEquationJoin.PerformanceCurve.PerformanceCurveLibraryId == libraryId);

            _unitOfWork.Context.DeleteEntity<PerformanceCurveLibraryEntity>(_ => _.Id == libraryId);
        }

        public List<PerformanceCurveDTO> GetScenarioPerformanceCurves(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found for the given scenario.");
            }

            return _unitOfWork.Context.ScenarioPerformanceCurve.Where(_ => _.SimulationId == simulationId)
                .Include(_ => _.ScenarioPerformanceCurveEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.CriterionLibraryScenarioPerformanceCurveJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Attribute)
                .Select(_ => _.ToDto())
                .AsNoTracking()
                .ToList();
        }

        public void UpsertOrDeleteScenarioPerformanceCurves(List<PerformanceCurveDTO> scenarioPerformanceCurves,
            Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found for the given scenario.");
            }

            if (scenarioPerformanceCurves.Any(_ => string.IsNullOrEmpty(_.Attribute)))
            {
                throw new InvalidOperationException("All performance curves must have an attribute.");
            }

            var attributeEntities = _unitOfWork.Context.Attribute.ToList();
            var attributeNames = attributeEntities.Select(_ => _.Name).ToList();
            if (!scenarioPerformanceCurves.All(_ => attributeNames.Contains(_.Attribute)))
            {
                var missingAttributes = scenarioPerformanceCurves.Select(_ => _.Attribute)
                    .Except(attributeNames).ToList();
                if (missingAttributes.Count == 1)
                {
                    throw new RowNotInTableException($"No attribute found having name {missingAttributes[0]}.");
                }

                throw new RowNotInTableException(
                    $"No attributes found having names: {string.Join(", ", missingAttributes)}.");
            }

            var scenarioPerformanceCurveEntities = scenarioPerformanceCurves
                .Select(_ =>
                    _.ToScenarioEntity(simulationId, attributeEntities.Single(__ => __.Name == _.Attribute).Id))
                .ToList();

            var entityIds = scenarioPerformanceCurves.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.ScenarioPerformanceCurve
                .Where(_ => _.SimulationId == simulationId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            _unitOfWork.Context.DeleteAll<ScenarioPerformanceCurveEntity>(_ =>
                _.SimulationId == simulationId && !entityIds.Contains(_.Id));

            _unitOfWork.Context.UpdateAll(scenarioPerformanceCurveEntities.Where(_ => existingEntityIds.Contains(_.Id))
                .ToList());

            _unitOfWork.Context.AddAll(scenarioPerformanceCurveEntities.Where(_ => !existingEntityIds.Contains(_.Id))
                .ToList());

            _unitOfWork.Context.DeleteAll<EquationEntity>(_ =>
                _.ScenarioPerformanceCurveEquationJoin.ScenarioPerformanceCurve.SimulationId == simulationId);

            _unitOfWork.Context.DeleteAll<CriterionLibraryScenarioPerformanceCurveEntity>(_ =>
                _.ScenarioPerformanceCurve.SimulationId == simulationId);

            if (scenarioPerformanceCurves.Any(_ =>
                _.Equation?.Id != null && _.Equation?.Id != Guid.Empty && !string.IsNullOrEmpty(_.Equation.Expression)))
            {
                var equationEntityPerScenarioPerformanceCurveEntityId = scenarioPerformanceCurves
                    .Where(_ => _.Equation?.Id != null && _.Equation?.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.Equation?.Expression))
                    .ToDictionary(_ => _.Id, _ => _.Equation.ToEntity());

                _unitOfWork.EquationRepo.CreateEquations(equationEntityPerScenarioPerformanceCurveEntityId,
                    DataPersistenceConstants.EquationJoinEntities.ScenarioPerformanceCurve);
            }

            if (scenarioPerformanceCurves.Any(_ =>
                _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
            {
                var curveCriterionJoinsToAdd = scenarioPerformanceCurves
                    .Where(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression))
                    .Select(_ => new CriterionLibraryScenarioPerformanceCurveEntity
                    {
                        CriterionLibraryId = _.CriterionLibrary.Id, ScenarioPerformanceCurveId = _.Id
                    })
                    .ToList();

                _unitOfWork.Context.AddAll(curveCriterionJoinsToAdd, _unitOfWork.UserEntity?.Id);
            }

            // Update last modified date
            var simulationEntity = _unitOfWork.Context.Simulation.Single(_ => _.Id == simulationId);
            _unitOfWork.Context.Upsert(simulationEntity, simulationId, _unitOfWork.UserEntity?.Id);
        }
    }
}
