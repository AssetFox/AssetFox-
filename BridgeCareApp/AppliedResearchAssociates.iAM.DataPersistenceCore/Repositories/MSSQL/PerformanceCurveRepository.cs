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
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics;
using Humanizer;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    internal class PerformanceCurveFamilyIds
    {
        public Guid PerformanceCurveId { get; set; }
        public Guid? EquationId { get; set; }
        public Guid? CriterionLibraryId { get; set; }
    }
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

        public void GetScenarioPerformanceCurves(Simulation simulation, Dictionary<Guid, string> attributeNameLookupDictionary)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulation.Id))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            _unitOfWork.Context.ScenarioPerformanceCurve.AsNoTracking()
                .Include(_ => _.CriterionLibraryScenarioPerformanceCurveJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.ScenarioPerformanceCurveEquationJoin)
                .ThenInclude(_ => _.Equation)
                .AsSplitQuery()
                .Where(_ => _.SimulationId == simulation.Id)
                .ToList()
                .ForEach(_ => _.CreatePerformanceCurve(simulation, attributeNameLookupDictionary));
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

        public List<PerformanceCurveLibraryDTO> GetPerformanceCurveLibrariesNoPerformanceCurves()
        {
            if (!_unitOfWork.Context.PerformanceCurveLibrary.Any())
            {
                return new List<PerformanceCurveLibraryDTO>();
            }

            return _unitOfWork.Context.PerformanceCurveLibrary.AsNoTracking()
                .Select(_ => _.ToDto())
                .ToList();
        }

        public void UpsertPerformanceCurveLibrary(PerformanceCurveLibraryDTO dto) =>
            _unitOfWork.Context.Upsert(dto.ToEntity(), dto.Id, _unitOfWork.UserEntity?.Id);

        public void UpsertOrDeletePerformanceCurves(
            List<PerformanceCurveDTO> performanceCurves,
            Guid libraryId)
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


            var performanceCurveFamilies =
                _unitOfWork.Context.PerformanceCurve.AsNoTracking()
                .Where(_ => _.PerformanceCurveLibrary.Id == libraryId && entityIds.Contains(_.Id))
                .Include(_ => _.PerformanceCurveEquationJoin)
                .Include(_ => _.CriterionLibraryPerformanceCurveJoin)
                .Select(_ => new PerformanceCurveFamilyIds
                {
                    PerformanceCurveId = _.Id,
                    EquationId = _.PerformanceCurveEquationJoin == null ? null : _.PerformanceCurveEquationJoin.EquationId,
                    CriterionLibraryId = _.CriterionLibraryPerformanceCurveJoin == null ? null:_.CriterionLibraryPerformanceCurveJoin.CriterionLibraryId,
                })
                .ToList();

            var performanceCurvesThatShouldHaveEquations = performanceCurves.Where(_ =>
                _.Equation?.Id != null && _.Equation?.Id != Guid.Empty && !string.IsNullOrEmpty(_.Equation.Expression))
                .ToList();
            var performanceCurvesThatShouldHaveCriterionLibraries =
                performanceCurves.Where(_ =>
                _.CriterionLibrary?.Id != null
                && _.CriterionLibrary?.Id != Guid.Empty
                && !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression))
                .ToList();
            var familiesThatDidHaveEquations = performanceCurveFamilies.Where(f => f.EquationId != null)
                .ToList();
            var familiesToDeleteEquations = familiesThatDidHaveEquations.Where(f => !performanceCurvesThatShouldHaveEquations.Any(pc => pc.Id == f.PerformanceCurveId)).ToList();
            var familiesToUpdateEquations = familiesThatDidHaveEquations.Where(f => performanceCurvesThatShouldHaveEquations.Any(pc => pc.Id == f.PerformanceCurveId)).ToList();
            var familiesThatDidHaveCriterionLibraries = performanceCurveFamilies.Where(f => f.CriterionLibraryId != null).ToList();
            var familiesToDeleteCriterionLibraries = familiesThatDidHaveCriterionLibraries.Where(f => !performanceCurvesThatShouldHaveCriterionLibraries.Any(pc => pc.Id == f.PerformanceCurveId)).ToList();
            var familiesToUpdateCriterieonLibraries = familiesThatDidHaveCriterionLibraries.Where(f => performanceCurvesThatShouldHaveCriterionLibraries.Any(pc => pc.Id == f.PerformanceCurveId)).ToList();

            var performanceCurvesToAddEquations = performanceCurvesThatShouldHaveEquations
                .Where(pc => !familiesThatDidHaveEquations.Any(f => f.PerformanceCurveId == pc.Id))
                .ToList();

            var performanceCurvesToAddLibraries = performanceCurvesThatShouldHaveCriterionLibraries
                .Where(pc => !familiesThatDidHaveCriterionLibraries.Any(f => f.PerformanceCurveId == pc.Id))
                .ToList();
            var equationIdsToDelete = familiesToDeleteEquations.Select(f => f.EquationId).ToList();
            var equationIdsToMaybeUpdate = familiesToUpdateEquations.Select(f => f.EquationId).ToList();
            var criterionLibraryIdsToDelete = familiesToDeleteCriterionLibraries
                .Select(f => f.CriterionLibraryId)
                .ToList();
            var criterionLibraryIdsToUpdate = familiesToUpdateCriterieonLibraries
                .Select(f => f.CriterionLibraryId)
                .ToList();

            var equationEntitiesToMaybeUpdate = _unitOfWork.Context.Equation
                .Where(e => equationIdsToMaybeUpdate.Contains(e.Id)).ToList();
            var criterionLibraryJoinsToMaybeUpdate =
                _unitOfWork.Context.CriterionLibraryPerformanceCurve
                .Include(clpc => clpc.CriterionLibrary)
                .Where(cl => criterionLibraryIdsToUpdate.Contains(cl.CriterionLibraryId))
                .ToList();
            var equationEntitiesToUpdate = new List<EquationEntity>();
            foreach (var equation in equationEntitiesToMaybeUpdate)
            {
                var family = familiesToUpdateEquations.Single(f => f.EquationId == equation.Id);
                var performanceCurve = performanceCurves.Single(pc => pc.Id == family.PerformanceCurveId);
                if (equation.Expression != performanceCurve.Equation.Expression)
                {
                    equation.Expression = performanceCurve.Equation.Expression;
                    equationEntitiesToUpdate.Add(equation);
                }
            }
            var criterionLibrariesToUpdate = new List<CriterionLibraryEntity>();
            foreach (var criterionLibrary in criterionLibraryJoinsToMaybeUpdate)
            {
                var family = familiesToUpdateCriterieonLibraries
                    .Single(f => f.CriterionLibraryId == criterionLibrary.CriterionLibraryId);
                var performanceCurve = performanceCurves.Single(pc => pc.Id == family.PerformanceCurveId);
                var name = $"{performanceCurve.Name} {performanceCurve.Attribute} Criterion";
                var mergedCrieteriaExpression = performanceCurve.CriterionLibrary.MergedCriteriaExpression;
                if (name!=criterionLibrary.CriterionLibrary.Name || mergedCrieteriaExpression!=criterionLibrary.CriterionLibrary.MergedCriteriaExpression)
                {
                    criterionLibrary.CriterionLibrary.Name = name;
                    criterionLibrary.CriterionLibrary.MergedCriteriaExpression = mergedCrieteriaExpression;
                    criterionLibrariesToUpdate.Add(criterionLibrary.CriterionLibrary);
                }
            }
            _unitOfWork.Context.UpdateRange(equationEntitiesToUpdate);
            _unitOfWork.Context.UpdateRange(criterionLibrariesToUpdate);

            _unitOfWork.Context.DeleteAll<PerformanceCurveEntity>(_ =>
                _.PerformanceCurveLibrary.Id == libraryId && !entityIds.Contains(_.Id));

            _unitOfWork.Context.UpdateAll(
                performanceCurveEntities.Where(_ => existingEntityIds.Contains(_.Id)).ToList());

            _unitOfWork.Context.AddAll(performanceCurveEntities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList());

            _unitOfWork.Context.DeleteAll<EquationEntity>(_ =>
                equationIdsToDelete.Contains(_.Id));

            _unitOfWork.Context.DeleteAll<CriterionLibraryPerformanceCurveEntity>(_ =>
                criterionLibraryIdsToDelete.Contains(_.CriterionLibraryId));

            if (performanceCurvesToAddEquations.Any())
            {
                var equationJoins = new List<PerformanceCurveEquationEntity>();

                var equations = performanceCurvesToAddEquations
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

            if (performanceCurvesToAddLibraries.Any())
            {
                var criterionJoins = new List<CriterionLibraryPerformanceCurveEntity>();

                var criteria = performanceCurvesToAddLibraries
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

        public List<PerformanceCurveDTO> GetScenarioPerformanceCurvesOrderedById(Guid simulationId)
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
                .OrderBy(_ => _.Id)
                .Select(_ => _.ToDto())
                .ToList();
        }

        public void UpsertOrDeleteScenarioPerformanceCurves(
            List<PerformanceCurveDTO> scenarioPerformanceCurves,
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
            var entityIds = scenarioPerformanceCurves.Select(_ => _.Id).ToList();

            var performanceCurveFamilies =
                _unitOfWork.Context.ScenarioPerformanceCurve.AsNoTracking()
                .Where(_ => _.SimulationId == simulationId && entityIds.Contains(_.Id))
                .Include(_ => _.ScenarioPerformanceCurveEquationJoin)
                .Include(_ => _.CriterionLibraryScenarioPerformanceCurveJoin)
                .Select(_ => new PerformanceCurveFamilyIds
                {
                    PerformanceCurveId = _.Id,
                    EquationId = _.ScenarioPerformanceCurveEquationJoin == null ? null : _.ScenarioPerformanceCurveEquationJoin.EquationId,
                    CriterionLibraryId = _.CriterionLibraryScenarioPerformanceCurveJoin == null ? null : _.CriterionLibraryScenarioPerformanceCurveJoin.CriterionLibraryId,
                })
                .ToList();
            var scenarioPerformanceCurveEntities = scenarioPerformanceCurves
                .Select(_ =>
                    _.ToScenarioEntity(simulationId, attributeEntities.Single(__ => __.Name == _.Attribute).Id))
                .ToList();

            var existingEntityIds = _unitOfWork.Context.ScenarioPerformanceCurve.AsNoTracking()
                .Where(_ => _.SimulationId == simulationId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            var performanceCurvesThatShouldHaveEquations = scenarioPerformanceCurves.Where(_ =>
               _.Equation?.Id != null && _.Equation?.Id != Guid.Empty && !string.IsNullOrEmpty(_.Equation.Expression))
                .ToList();
            var performanceCurvesThatShouldHaveCriterionLibraries =
                scenarioPerformanceCurves.Where(_ =>
                _.CriterionLibrary?.Id != null
                && _.CriterionLibrary?.Id != Guid.Empty
                && !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression))
                .ToList();
            var familiesThatDidHaveEquations = performanceCurveFamilies.Where(f => f.EquationId != null)
                .ToList();
            var familiesToDeleteEquations = familiesThatDidHaveEquations.Where(f => !performanceCurvesThatShouldHaveEquations.Any(pc => pc.Id == f.PerformanceCurveId)).ToList();
            var familiesToUpdateEquations = familiesThatDidHaveEquations.Where(f => performanceCurvesThatShouldHaveEquations.Any(pc => pc.Id == f.PerformanceCurveId)).ToList();
            var familiesThatDidHaveCriterionLibraries = performanceCurveFamilies.Where(f => f.CriterionLibraryId != null).ToList();
            var familiesToDeleteCriterionLibraries = familiesThatDidHaveCriterionLibraries.Where(f => !performanceCurvesThatShouldHaveCriterionLibraries.Any(pc => pc.Id == f.PerformanceCurveId)).ToList();
            var familiesToUpdateCriterieonLibraries = familiesThatDidHaveCriterionLibraries.Where(f => performanceCurvesThatShouldHaveCriterionLibraries.Any(pc => pc.Id == f.PerformanceCurveId)).ToList();

            var performanceCurvesToAddEquations = performanceCurvesThatShouldHaveEquations
                .Where(pc => !familiesThatDidHaveEquations.Any(f => f.PerformanceCurveId == pc.Id))
                .ToList();

            var performanceCurvesToAddLibraries = performanceCurvesThatShouldHaveCriterionLibraries
                .Where(pc => !familiesThatDidHaveCriterionLibraries.Any(f => f.PerformanceCurveId == pc.Id))
                .ToList();
            var equationIdsToDelete = familiesToDeleteEquations.Select(f => f.EquationId).ToList();
            var equationIdsToMaybeUpdate = familiesToUpdateEquations.Select(f => f.EquationId).ToList();
            var criterionLibraryIdsToDelete = familiesToDeleteCriterionLibraries
                .Select(f => f.CriterionLibraryId)
                .ToList();
            var criterionLibraryIdsToUpdate = familiesToUpdateCriterieonLibraries
                .Select(f => f.CriterionLibraryId)
                .ToList();

            var equationEntitiesToMaybeUpdate = _unitOfWork.Context.Equation
                .Where(e => equationIdsToMaybeUpdate.Contains(e.Id)).ToList();
            var criterionLibraryJoinsToMaybeUpdate =
                _unitOfWork.Context.CriterionLibraryScenarioPerformanceCurve
                .Include(clpc => clpc.CriterionLibrary)
                .Where(cl => criterionLibraryIdsToUpdate.Contains(cl.CriterionLibraryId))
                .ToList();
            var equationEntitiesToUpdate = new List<EquationEntity>();
            foreach (var equation in equationEntitiesToMaybeUpdate)
            {
                var family = familiesToUpdateEquations.Single(f => f.EquationId == equation.Id);
                var performanceCurve = scenarioPerformanceCurves.Single(pc => pc.Id == family.PerformanceCurveId);
                if (equation.Expression != performanceCurve.Equation.Expression)
                {
                    equation.Expression = performanceCurve.Equation.Expression;
                    equationEntitiesToUpdate.Add(equation);
                }
            }
            var criterionLibrariesToUpdate = new List<CriterionLibraryEntity>();
            foreach (var criterionLibrary in criterionLibraryJoinsToMaybeUpdate)
            {
                var family = familiesToUpdateCriterieonLibraries
                    .Single(f => f.CriterionLibraryId == criterionLibrary.CriterionLibraryId);
                var performanceCurve = scenarioPerformanceCurves.Single(pc => pc.Id == family.PerformanceCurveId);
                var name = $"{performanceCurve.Name} {performanceCurve.Attribute} Criterion";
                var mergedCrieteriaExpression = performanceCurve.CriterionLibrary.MergedCriteriaExpression;
                if (name != criterionLibrary.CriterionLibrary.Name || mergedCrieteriaExpression != criterionLibrary.CriterionLibrary.MergedCriteriaExpression)
                {
                    criterionLibrary.CriterionLibrary.Name = name;
                    criterionLibrary.CriterionLibrary.MergedCriteriaExpression = mergedCrieteriaExpression;
                    criterionLibrariesToUpdate.Add(criterionLibrary.CriterionLibrary);
                }
            }
            _unitOfWork.Context.UpdateRange(equationEntitiesToUpdate);
            _unitOfWork.Context.UpdateRange(criterionLibrariesToUpdate);

            _unitOfWork.Context.DeleteAll<ScenarioPerformanceCurveEntity>(_ =>
                _.SimulationId == simulationId && !entityIds.Contains(_.Id));

            _unitOfWork.Context.UpdateAll(
                scenarioPerformanceCurveEntities.Where(_ => existingEntityIds.Contains(_.Id)).ToList());

            _unitOfWork.Context.AddAll(scenarioPerformanceCurveEntities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList());

            // wjwjwj probably should not be deleting and re-adding? Instead, keep equations around if poossible, and
            // the same for criteria? But when making the change, see if we run into trouble.
            _unitOfWork.Context.DeleteAll<EquationEntity>(_ =>
                equationIdsToDelete.Contains(_.Id));

            _unitOfWork.Context.DeleteAll<CriterionLibraryScenarioPerformanceCurveEntity>(_ =>
                criterionLibraryIdsToDelete.Contains(_.CriterionLibraryId));

            if (performanceCurvesToAddEquations.Any())
            {
                var equationJoins = new List<ScenarioPerformanceCurveEquationEntity>();

                var equations = performanceCurvesToAddEquations
                    .Select(curve =>
                    {
                        var equation = new EquationEntity
                        {
                            Id = Guid.NewGuid(),
                            Expression = curve.Equation.Expression,
                        };
                        equationJoins.Add(new ScenarioPerformanceCurveEquationEntity
                        {
                            EquationId = equation.Id,
                            ScenarioPerformanceCurveId = curve.Id
                        });
                        return equation;
                    }).ToList();

                _unitOfWork.Context.AddAll(equations, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(equationJoins, _unitOfWork.UserEntity?.Id);
            }

            if (performanceCurvesToAddLibraries.Any())
            {
                var criterionJoins = new List<CriterionLibraryScenarioPerformanceCurveEntity>();

                var criteria = performanceCurvesToAddLibraries
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
                            CriterionLibraryId = criterion.Id,
                            ScenarioPerformanceCurveId = curve.Id
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

        public void UpsertOrDeleteScenarioPerformanceCurvesAtomically(
            List<PerformanceCurveDTO> scenarioPerformanceCurves,
            Guid simulationId)
        {
            _unitOfWork.AsTransaction(u =>
            u.PerformanceCurveRepo.UpsertOrDeleteScenarioPerformanceCurves(scenarioPerformanceCurves, simulationId));
        }

        public List<PerformanceCurveDTO> GetPerformanceCurvesForLibrary(Guid performanceCurveLibraryId)
        {
            if (!_unitOfWork.Context.PerformanceCurveLibrary.Any(_ => _.Id == performanceCurveLibraryId))
            {
                return new List<PerformanceCurveDTO>();
            }

            return _unitOfWork.Context.PerformanceCurve.AsNoTracking()
                .Where(_ => _.PerformanceCurveLibraryId == performanceCurveLibraryId)
                .Include(_ => _.Attribute)
                .Include(_ => _.CriterionLibraryPerformanceCurveJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.PerformanceCurveEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Select(_ => _.ToDto())
                .ToList();
        }

        public PerformanceCurveLibraryDTO GetPerformanceCurveLibrary(Guid performanceCurveLibraryId)
        {
            if (!_unitOfWork.Context.PerformanceCurveLibrary.Any())
            {
                return new PerformanceCurveLibraryDTO();
            }

            return _unitOfWork.Context.PerformanceCurveLibrary.AsNoTracking()
                .Where(_ => _.Id == performanceCurveLibraryId)
                .Include(_ => _.PerformanceCurves)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.PerformanceCurves)
                .ThenInclude(_ => _.CriterionLibraryPerformanceCurveJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Users)
                .Include(_ => _.PerformanceCurves)
                .ThenInclude(_ => _.PerformanceCurveEquationJoin)
                .ThenInclude(_ => _.Equation)
                .FirstOrDefault()?.ToDto();
        }

        public List<PerformanceCurveDTO> GetPerformanceCurvesForLibraryOrderedById(Guid performanceCurveLibraryId)
        {
            if (!_unitOfWork.Context.PerformanceCurveLibrary.Any(_ => _.Id == performanceCurveLibraryId))
            {
                return new List<PerformanceCurveDTO>();
            }

            return _unitOfWork.Context.PerformanceCurve.AsNoTracking()
                .Where(_ => _.PerformanceCurveLibraryId == performanceCurveLibraryId)
                .Include(_ => _.Attribute)
                .Include(_ => _.CriterionLibraryPerformanceCurveJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.PerformanceCurveEquationJoin)
                .ThenInclude(_ => _.Equation)
                .OrderBy(_ => _.Id)
                .Select(_ => _.ToDto())
                .ToList();
        }
        public List<PerformanceCurveLibraryDTO> GetPerformanceCurveLibrariesNoChildrenAccessibleToUser(Guid userId)
        {
            return _unitOfWork.Context.PerformanceCurveLibraryUser
               .AsNoTracking()
               .Include(u => u.PerformanceCurveLibrary)
               .Where(u => u.UserId == userId)
               .Select(u => u.PerformanceCurveLibrary.ToDto())
               .ToList();
        }
        public LibraryUserAccessModel GetLibraryAccess(Guid libraryId, Guid userId)
        {
            var exists = _unitOfWork.Context.PerformanceCurveLibrary.Any(pc => pc.Id == libraryId);
            if (!exists)
            {
                return LibraryAccessModels.LibraryDoesNotExist();
            }
            var users = GetAccessForUser(libraryId, userId);
            var user = users.FirstOrDefault();
            return LibraryAccessModels.LibraryExistsWithUsers(userId, user);
        }
        public void UpsertOrDeleteUsers(Guid performanceCurveLibraryId, IList<LibraryUserDTO> libraryUsers)
        {
            var existingEntities = _unitOfWork.Context.PerformanceCurveLibraryUser.Where(u => u.LibraryId == performanceCurveLibraryId).ToList();
            var existingUserIds = existingEntities.Select(u => u.UserId).ToList();
            var desiredUserIDs = libraryUsers.Select(lu => lu.UserId).ToList();
            var userIdsToDelete = existingUserIds.Except(desiredUserIDs).ToList();
            var userIdsToUpdate = existingUserIds.Intersect(desiredUserIDs).ToList();
            var userIdsToAdd = desiredUserIDs.Except(existingUserIds).ToList();
            var entitiesToAdd = libraryUsers.Where(u => userIdsToAdd.Contains(u.UserId)).Select(u => LibraryUserMapper.ToPerformanceCurveLibraryUserEntity(u, performanceCurveLibraryId)).ToList();
            var dtosToUpdate = libraryUsers.Where(u => userIdsToUpdate.Contains(u.UserId)).ToList();
            var entitiesToMaybeUpdate = existingEntities.Where(u => userIdsToUpdate.Contains(u.UserId)).ToList();
            var entitiesToUpdate = new List<PerformanceCurveLibraryUserEntity>();
            foreach (var dto in dtosToUpdate)
            {
                var entityToUpdate = entitiesToMaybeUpdate.FirstOrDefault(e => e.UserId == dto.UserId);
                if (entityToUpdate != null && entityToUpdate.AccessLevel != (int)dto.AccessLevel)
                {
                    entityToUpdate.AccessLevel = (int)dto.AccessLevel;
                    entitiesToUpdate.Add(entityToUpdate);
                }
            }
            _unitOfWork.Context.AddRange(entitiesToAdd);
            _unitOfWork.Context.UpdateRange(entitiesToUpdate);
            var entitiesToDelete = existingEntities.Where(u => userIdsToDelete.Contains(u.UserId)).ToList();
            _unitOfWork.Context.RemoveRange(entitiesToDelete);
            _unitOfWork.Context.SaveChanges();
        }
        public List<LibraryUserDTO> GetAccessForUser(Guid performanceCurveLibraryId, Guid userId)
        {
            var dtos = _unitOfWork.Context.PerformanceCurveLibraryUser
                .Where(u => u.LibraryId == performanceCurveLibraryId && u.UserId == userId)
                .Select(LibraryUserMapper.ToDto)
                .ToList();
            return dtos;
        }
        public List<LibraryUserDTO> GetLibraryUsers(Guid performanceCurveLibraryId)
        {
            var dtos = _unitOfWork.Context.PerformanceCurveLibraryUser
                .Include(u => u.User)
                .Where(u => u.LibraryId == performanceCurveLibraryId)
                .Select(LibraryUserMapper.ToDto)
                .ToList();
            return dtos;
        }

        public void UpsertOrDeletePerformanceCurveLibraryAndCurves(PerformanceCurveLibraryDTO library, bool isNewLibrary, Guid ownerIdForNewLibrary)
        {
            _unitOfWork.AsTransaction(u =>
            {
                u.PerformanceCurveRepo.UpsertPerformanceCurveLibrary(library);
                u.PerformanceCurveRepo.UpsertOrDeletePerformanceCurves(library.PerformanceCurves, library.Id);
                if (isNewLibrary)
                {
                    var users = LibraryUserDtolists.OwnerAccess(ownerIdForNewLibrary);
                    u.PerformanceCurveRepo.UpsertOrDeleteUsers(library.Id, users);
                };
            });
        }
    }
}
