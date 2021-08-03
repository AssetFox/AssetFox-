using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities;
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
        }

        public void CreatePerformanceCurves(List<PerformanceCurve> performanceCurves, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            var simulationEntity = _unitOfWork.Context.Simulation
                .Single(_ => _.Id == simulationId);

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
                    _.ToScenarioEntity(simulationId, attributeEntities.Single(__ => __.Name == _.Attribute.Name).Id))
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

            if (performanceCurves.Any(_ => !_.Equation.ExpressionIsBlank))
            {
                var equationEntities = new List<EquationEntity>();
                var equationJoinEntities = new List<ScenarioPerformanceCurveEquationEntity>();

                performanceCurves.Where(_ => !_.Equation.ExpressionIsBlank)
                    .ForEach(curve =>
                    {
                        var equationEntity = new EquationEntity
                        {
                            Id = Guid.NewGuid(),
                            Expression = curve.Equation.Expression,
                        };
                        equationEntities.Add(equationEntity);
                        equationJoinEntities.Add(new ScenarioPerformanceCurveEquationEntity
                        {
                            EquationId = equationEntity.Id, ScenarioPerformanceCurveId = curve.Id
                        });
                    });

                _unitOfWork.Context.AddAll(equationEntities, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(equationJoinEntities, _unitOfWork.UserEntity?.Id);
            }

            if (performanceCurves.Any(_ => !_.Criterion.ExpressionIsBlank))
            {
                var criterionLibraryEntities = new List<CriterionLibraryEntity>();
                var criterionLibraryJoinEntities = new List<CriterionLibraryPerformanceCurveEntity>();

                performanceCurves.Where(curve => !curve.Criterion.ExpressionIsBlank)
                    .ForEach(curve =>
                    {
                        var criterionLibraryEntity = new CriterionLibraryEntity
                        {
                            Id = Guid.NewGuid(),
                            MergedCriteriaExpression = curve.Criterion.Expression,
                            Name = $"{curve.Name} {curve.Attribute} Criterion",
                            IsSingleUse = true
                        };
                        criterionLibraryEntities.Add(criterionLibraryEntity);
                        criterionLibraryJoinEntities.Add(new CriterionLibraryPerformanceCurveEntity
                        {
                            CriterionLibraryId = criterionLibraryEntity.Id, PerformanceCurveId = curve.Id
                        });
                    });

                _unitOfWork.Context.AddAll(criterionLibraryEntities, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criterionLibraryJoinEntities, _unitOfWork.UserEntity?.Id);
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
                .Include(_ => _.PerformanceCurves)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.PerformanceCurves)
                .ThenInclude(_ => _.CriterionLibraryScenarioPerformanceCurveJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.PerformanceCurves)
                .ThenInclude(_ => _.ScenarioPerformanceCurveEquationJoin)
                .ThenInclude(_ => _.Equation)
                .AsNoTracking()
                .Single(_ => _.Id == simulation.Id);

            simulationEntity.PerformanceCurves?.ForEach(_ => _.CreatePerformanceCurve(simulation));
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
                var criterionLibraryEntities = new List<CriterionLibraryEntity>();
                var criterionLibraryJoinEntities = new List<CriterionLibraryPerformanceCurveEntity>();

                performanceCurves.Where(curve =>
                        curve.CriterionLibrary?.Id != null && curve.CriterionLibrary?.Id != Guid.Empty &&
                        !string.IsNullOrEmpty(curve.CriterionLibrary.MergedCriteriaExpression))
                    .ForEach(curve =>
                    {
                        var criterionLibraryEntity = new CriterionLibraryEntity
                        {
                            Id = Guid.NewGuid(),
                            MergedCriteriaExpression = curve.CriterionLibrary.MergedCriteriaExpression,
                            Name = $"{curve.Name} {curve.Attribute} Criterion",
                            IsSingleUse = true
                        };
                        criterionLibraryEntities.Add(criterionLibraryEntity);
                        criterionLibraryJoinEntities.Add(new CriterionLibraryPerformanceCurveEntity
                        {
                            CriterionLibraryId = criterionLibraryEntity.Id, PerformanceCurveId = curve.Id
                        });
                    });

                _unitOfWork.Context.AddAll(criterionLibraryEntities, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criterionLibraryJoinEntities, _unitOfWork.UserEntity?.Id);
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
                var equationEntities = new List<EquationEntity>();
                var equationJoinEntities = new List<ScenarioPerformanceCurveEquationEntity>();

                scenarioPerformanceCurves.Where(_ =>
                        _.Equation?.Id != null && _.Equation?.Id != Guid.Empty && !string.IsNullOrEmpty(_.Equation.Expression))
                    .ForEach(curve =>
                    {
                        var equationEntity = new EquationEntity
                        {
                            Id = Guid.NewGuid(),
                            Expression = curve.Equation.Expression,
                        };
                        equationEntities.Add(equationEntity);
                        equationJoinEntities.Add(new ScenarioPerformanceCurveEquationEntity
                        {
                            EquationId = equationEntity.Id, ScenarioPerformanceCurveId = curve.Id
                        });
                    });

                _unitOfWork.Context.AddAll(equationEntities, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(equationJoinEntities, _unitOfWork.UserEntity?.Id);
            }

            if (scenarioPerformanceCurves.Any(_ =>
                _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
            {
                var criterionLibraryEntities = new List<CriterionLibraryEntity>();
                var criterionLibraryJoinEntities = new List<CriterionLibraryScenarioPerformanceCurveEntity>();

                scenarioPerformanceCurves.Where(_ =>
                        _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                        !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression))
                    .ForEach(curve =>
                    {
                        var criterionLibraryEntity = new CriterionLibraryEntity
                        {
                            Id = Guid.NewGuid(),
                            MergedCriteriaExpression = curve.CriterionLibrary.MergedCriteriaExpression,
                            Name = $"{curve.Name} {curve.Attribute} Criterion",
                            IsSingleUse = true
                        };
                        criterionLibraryEntities.Add(criterionLibraryEntity);
                        criterionLibraryJoinEntities.Add(new CriterionLibraryScenarioPerformanceCurveEntity
                        {
                            CriterionLibraryId = criterionLibraryEntity.Id, ScenarioPerformanceCurveId = curve.Id
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
