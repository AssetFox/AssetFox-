using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class PerformanceCurveRepository : IPerformanceCurveRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public PerformanceCurveRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateScenarioPerformanceCurves(List<PerformanceCurve> performanceCurves, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            var simulationEntity = _unitOfWork.Context.Simulation
                .Single(_ => _.Id == simulationId);

            var attributeEntities = _unitOfWork.Context.Attribute.AsNoTracking().ToList();
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
                .Select(_ => _.ToScenarioEntity(simulationId, attributeEntities
                    .Single(attribute => attribute.Name == _.Attribute.Name).Id))
                .ToList();

            _unitOfWork.Context.AddAll(performanceCurveEntities);

            if (performanceCurves.Any(_ => !_.Equation.ExpressionIsBlank))
            {
                var equationJoins = new List<ScenarioPerformanceCurveEquationEntity>();

                var equations = performanceCurves.Where(_ => !_.Equation.ExpressionIsBlank)
                    .Select(curve =>
                    {
                        var equation = new EquationEntity
                        {
                            Id = Guid.NewGuid(),
                            Expression = curve.Equation.Expression,
                        };
                        equationJoins.Add(new ScenarioPerformanceCurveEquationEntity
                        {
                            EquationId = equation.Id, ScenarioPerformanceCurveId = curve.Id
                        });
                        return equation;
                    }).ToList();

                _unitOfWork.Context.AddAll(equations, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(equationJoins, _unitOfWork.UserEntity?.Id);
            }

            if (performanceCurves.Any(_ => !_.Criterion.ExpressionIsBlank))
            {
                var criterionJoins = new List<CriterionLibraryScenarioPerformanceCurveEntity>();

                var criteria = performanceCurves.Where(curve => !curve.Criterion.ExpressionIsBlank)
                    .Select(curve =>
                    {
                        var criterion = new CriterionLibraryEntity
                        {
                            Id = Guid.NewGuid(),
                            MergedCriteriaExpression = curve.Criterion.Expression,
                            Name = $"{curve.Name} {curve.Attribute} Criterion",
                            IsSingleUse = true
                        };
                        criterionJoins.Add(new CriterionLibraryScenarioPerformanceCurveEntity
                        {
                            CriterionLibraryId = criterion.Id, ScenarioPerformanceCurveId = curve.Id
                        });
                        return criterion;
                    }).ToList();

                _unitOfWork.Context.AddAll(criteria, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criterionJoins, _unitOfWork.UserEntity?.Id);
            }

            // Update last modified date
            _unitOfWork.SimulationRepo.UpdateLastModifiedDate(simulationEntity);
        }

        public void GetScenarioPerformanceCurves(Simulation simulation)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulation.Id))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            if (_unitOfWork.Context.ScenarioPerformanceCurve.Any(_ => _.SimulationId == simulation.Id))
            {
                return;
            }

            _unitOfWork.Context.ScenarioPerformanceCurve.AsNoTracking()
                .Include(_ => _.Attribute)
                .Include(_ => _.CriterionLibraryScenarioPerformanceCurveJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.ScenarioPerformanceCurveEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Where(_ => _.SimulationId == simulation.Id)
                .ToList()
                .ForEach(_ => _.CreatePerformanceCurve(simulation));
        }

        public List<PerformanceCurveLibraryDTO> GetPerformanceCurveLibraries()
        {
            if (!_unitOfWork.Context.PerformanceCurveLibrary.Any())
            {
                return new List<PerformanceCurveLibraryDTO>();
            }

            return _unitOfWork.Context.PerformanceCurveLibrary.AsNoTracking()
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

        public void UpsertPerformanceCurveLibrary(PerformanceCurveLibraryDTO dto) =>
            _unitOfWork.Context.Upsert(dto.ToEntity(), dto.Id, _unitOfWork.UserEntity?.Id);

        public void UpsertOrDeletePerformanceCurves(List<PerformanceCurveDTO> performanceCurves, Guid libraryId)
        {
            if (!_unitOfWork.Context.PerformanceCurveLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException("The specified performance curve library was not found.");
            }

            if (performanceCurves.Any(_ => string.IsNullOrEmpty(_.Attribute)))
            {
                throw new InvalidOperationException("All performance curves must have an attribute.");
            }

            var attributeEntities = _unitOfWork.Context.Attribute.AsNoTracking().ToList();
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

            var existingEntityIds = _unitOfWork.Context.PerformanceCurve.AsNoTracking()
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
                var equationJoins = new List<PerformanceCurveEquationEntity>();

                var equations = performanceCurves
                    .Where(_ => _.Equation?.Id != null && _.Equation?.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.Equation.Expression))
                    .Select(curve =>
                    {
                        var equation = new EquationEntity
                        {
                            Id = Guid.NewGuid(),
                            Expression = curve.Equation.Expression,
                        };
                        equationJoins.Add(new PerformanceCurveEquationEntity
                        {
                            EquationId = equation.Id, PerformanceCurveId = curve.Id
                        });
                        return equation;
                    }).ToList();

                _unitOfWork.Context.AddAll(equations, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(equationJoins, _unitOfWork.UserEntity?.Id);
            }

            if (performanceCurves.Any(_ =>
                _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
            {
                var criterionJoins = new List<CriterionLibraryPerformanceCurveEntity>();

                var criteria = performanceCurves
                    .Where(curve =>curve.CriterionLibrary?.Id != null && curve.CriterionLibrary?.Id != Guid.Empty &&
                                   !string.IsNullOrEmpty(curve.CriterionLibrary.MergedCriteriaExpression))
                    .Select(curve =>
                    {
                        var criterion = new CriterionLibraryEntity
                        {
                            Id = Guid.NewGuid(),
                            MergedCriteriaExpression = curve.CriterionLibrary.MergedCriteriaExpression,
                            Name = $"{curve.Name} {curve.Attribute} Criterion",
                            IsSingleUse = true
                        };
                        criterionJoins.Add(new CriterionLibraryPerformanceCurveEntity
                        {
                            CriterionLibraryId = criterion.Id, PerformanceCurveId = curve.Id
                        });
                        return criterion;
                    }).ToList();

                _unitOfWork.Context.AddAll(criteria, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criterionJoins, _unitOfWork.UserEntity?.Id);
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
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            return _unitOfWork.Context.ScenarioPerformanceCurve.AsNoTracking()
                .Where(_ => _.SimulationId == simulationId)
                .Include(_ => _.ScenarioPerformanceCurveEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.CriterionLibraryScenarioPerformanceCurveJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Attribute)
                .Select(_ => _.ToDto())
                .ToList();
        }

        public void UpsertOrDeleteScenarioPerformanceCurves(List<PerformanceCurveDTO> scenarioPerformanceCurves,
            Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            if (scenarioPerformanceCurves.Any(_ => string.IsNullOrEmpty(_.Attribute)))
            {
                throw new InvalidOperationException("All performance curves must have an attribute.");
            }

            var attributeEntities = _unitOfWork.Context.Attribute.AsNoTracking().ToList();
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

            var existingEntityIds = _unitOfWork.Context.ScenarioPerformanceCurve.AsNoTracking()
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
                var criterionJoins = new List<CriterionLibraryScenarioPerformanceCurveEntity>();

                var criteria = scenarioPerformanceCurves
                    .Where(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression))
                    .Select(curve =>
                    {
                        var criterion = new CriterionLibraryEntity
                        {
                            Id = Guid.NewGuid(),
                            MergedCriteriaExpression = curve.CriterionLibrary.MergedCriteriaExpression,
                            Name = $"{curve.Name} {curve.Attribute} Criterion",
                            IsSingleUse = true
                        };
                        criterionJoins.Add(new CriterionLibraryScenarioPerformanceCurveEntity
                        {
                            CriterionLibraryId = criterion.Id, ScenarioPerformanceCurveId = curve.Id
                        });
                        return criterion;
                    }).ToList();

                _unitOfWork.Context.AddAll(criteria, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criterionJoins, _unitOfWork.UserEntity?.Id);
            }

            // Update last modified date
            var simulationEntity = _unitOfWork.Context.Simulation.Single(_ => _.Id == simulationId);
            _unitOfWork.Context.Upsert(simulationEntity, simulationId, _unitOfWork.UserEntity?.Id);
        }
    }
}
