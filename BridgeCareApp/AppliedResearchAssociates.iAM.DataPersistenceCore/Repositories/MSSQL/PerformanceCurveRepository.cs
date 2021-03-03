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
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class PerformanceCurveRepository : IPerformanceCurveRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public PerformanceCurveRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfDataPersistenceWork) =>
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));

        public void CreatePerformanceCurveLibrary(string name, Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            var performanceCurveLibraryEntity = new PerformanceCurveLibraryEntity { Id = Guid.NewGuid(), Name = name };

            _unitOfDataPersistenceWork.Context.PerformanceCurveLibrary.Add(performanceCurveLibraryEntity);

            _unitOfDataPersistenceWork.Context.PerformanceCurveLibrarySimulation.Add(new PerformanceCurveLibrarySimulationEntity
            {
                PerformanceCurveLibraryId = performanceCurveLibraryEntity.Id,
                SimulationId = simulationId
            });
        }

        public void CreatePerformanceCurves(List<PerformanceCurve> performanceCurves, Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            var simulationEntity = _unitOfDataPersistenceWork.Context.Simulation
                .Include(_ => _.PerformanceCurveLibrarySimulationJoin)
                .Single(_ => _.Id == simulationId);

            if (simulationEntity.PerformanceCurveLibrarySimulationJoin == null)
            {
                throw new RowNotInTableException($"No performance curve library found for simulation having id {simulationId}.");
            }

            var attributeNames = performanceCurves.Select(_ => _.Attribute.Name).Distinct().ToList();
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

            var performanceCurveEntities = performanceCurves
                .Select(_ => _.ToEntity(simulationEntity.PerformanceCurveLibrarySimulationJoin.PerformanceCurveLibraryId,
                    attributeEntities.Single(__ => __.Name == _.Attribute.Name).Id))
                .ToList();

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.PerformanceCurve.AddRange(performanceCurveEntities);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkInsert(performanceCurveEntities);
            }

            if (performanceCurves.Any(_ => !_.Equation.ExpressionIsBlank))
            {
                var equationEntityPerPerformanceCurveEntityId = performanceCurves
                    .Where(_ => !_.Equation.ExpressionIsBlank)
                    .ToDictionary(_ => _.Id, _ => _.Equation.ToEntity());

                _unitOfDataPersistenceWork.EquationRepo.CreateEquations(equationEntityPerPerformanceCurveEntityId, DataPersistenceConstants.EquationJoinEntities.PerformanceCurve);
            }

            if (performanceCurves.Any(_ => !_.Criterion.ExpressionIsBlank))
            {
                var curveIdsPerExpression = performanceCurves
                    .Where(_ => !_.Criterion.ExpressionIsBlank)
                    .GroupBy(_ => _.Criterion.Expression, _ => _)
                    .ToDictionary(_ => _.Key, _ => _.Select(__ => __.Id).ToList());

                _unitOfDataPersistenceWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(curveIdsPerExpression,
                    DataPersistenceConstants.CriterionLibraryJoinEntities.PerformanceCurve, simulationEntity.Name);
            }
        }

        public void SimulationPerformanceCurves(Simulation simulation)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Name == simulation.Name))
            {
                throw new RowNotInTableException($"No simulation found having name {simulation.Name}");
            }

            _unitOfDataPersistenceWork.Context.PerformanceCurve
                .Include(_ => _.Attribute)
                .Include(_ => _.CriterionLibraryPerformanceCurveJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.PerformanceCurveEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.PerformanceCurveLibrary)
                .ThenInclude(_ => _.PerformanceCurveLibrarySimulationJoins)
                .Where(_ => _.PerformanceCurveLibrary.PerformanceCurveLibrarySimulationJoins
                    .SingleOrDefault(__ => __.Simulation.Name == simulation.Name) != null)
                .ForEach(_ => _.CreatePerformanceCurve(simulation));
        }

        public void AddOrUpdatePerformanceCurveLibrary(PerformanceCurveLibraryDTO dto, Guid simulationId)
        {
            var performanceCurveLibraryEntity = dto.ToEntity();

            _unitOfDataPersistenceWork.Context.AddOrUpdate(performanceCurveLibraryEntity, dto.Id);

            if (simulationId != Guid.Empty)
            {
                if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
                {
                    throw new RowNotInTableException($"No simulation found having id {simulationId}.");
                }

                _unitOfDataPersistenceWork.Context.Delete<PerformanceCurveLibrarySimulationEntity>(_ => _.SimulationId == simulationId);

                _unitOfDataPersistenceWork.Context.PerformanceCurveLibrarySimulation.Add(new PerformanceCurveLibrarySimulationEntity
                {
                    PerformanceCurveLibraryId = performanceCurveLibraryEntity.Id,
                    SimulationId = simulationId
                });
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        public void AddOrUpdateOrDeletePerformanceCurves(List<PerformanceCurveDTO> performanceCurves, Guid libraryId)
        {
            if (!_unitOfDataPersistenceWork.Context.PerformanceCurveLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException($"No performance curve library found having id {libraryId}.");
            }

            if (performanceCurves.Any(_ => string.IsNullOrEmpty(_.Attribute)))
            {
                throw new InvalidOperationException("All performance curves must have an attribute.");
            }

            var attributeEntities = _unitOfDataPersistenceWork.Context.Attribute.ToList();
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
                .Select(_ => _.ToEntity(libraryId, attributeEntities.Single(__ => __.Name == _.Attribute).Id))
                .ToList();

            var entityIds = performanceCurves.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfDataPersistenceWork.Context.PerformanceCurve
                .Where(_ => _.PerformanceCurveLibrary.Id == libraryId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            var predicatesPerCrudOperation = new Dictionary<string, Expression<Func<PerformanceCurveEntity, bool>>>
            {
                {"delete", _ => _.PerformanceCurveLibrary.Id == libraryId && !entityIds.Contains(_.Id)},
                {"update", _ => existingEntityIds.Contains(_.Id)},
                {"add", _ => !existingEntityIds.Contains(_.Id)}
            };

            var equationIds = _unitOfDataPersistenceWork.Context.PerformanceCurveEquation
                .Where(_ => _.PerformanceCurve.PerformanceCurveLibraryId == libraryId)
                .Select(_ => _.EquationId).ToList();

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.AddOrUpdateOrDelete(performanceCurveEntities, predicatesPerCrudOperation);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkAddOrUpdateOrDelete(performanceCurveEntities, predicatesPerCrudOperation);
            }

            _unitOfDataPersistenceWork.Context.DeleteAll<EquationEntity>(_ => equationIds.Contains(_.Id));

            _unitOfDataPersistenceWork.Context.DeleteAll<CriterionLibraryPerformanceCurveEntity>(_ =>
                _.PerformanceCurve.PerformanceCurveLibraryId == libraryId);

            if (performanceCurves.Any(_ => _.Equation?.Id != null && _.Equation?.Id != Guid.Empty && !string.IsNullOrEmpty(_.Equation.Expression)))
            {
                var equationEntityPerPerformanceCurveEntityId = performanceCurves
                    .Where(_ => _.Equation?.Id != null && _.Equation?.Id != Guid.Empty && !string.IsNullOrEmpty(_.Equation?.Expression))
                    .ToDictionary(_ => _.Id, _ => _.Equation.ToEntity());

                _unitOfDataPersistenceWork.EquationRepo.CreateEquations(equationEntityPerPerformanceCurveEntityId,
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
                        CriterionLibraryId = _.CriterionLibrary.Id,
                        PerformanceCurveId = _.Id
                    })
                    .ToList();

                if (IsRunningFromXUnit)
                {
                    _unitOfDataPersistenceWork.Context.CriterionLibraryPerformanceCurve.AddRange(curveCriterionJoinsToAdd);
                }
                else
                {
                    _unitOfDataPersistenceWork.Context.BulkInsert(curveCriterionJoinsToAdd);
                }
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        public Task<List<PerformanceCurveLibraryDTO>> PerformanceCurveLibrariesWithPerformanceCurves()
        {
            if (!_unitOfDataPersistenceWork.Context.PerformanceCurveLibrary.Any())
            {
                return Task.Factory.StartNew(() => new List<PerformanceCurveLibraryDTO>());
            }

            return Task.Factory.StartNew(() => _unitOfDataPersistenceWork.Context.PerformanceCurveLibrary
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
                .ToList());
        }

        public void DeletePerformanceCurveLibrary(Guid libraryId)
        {
            if (!_unitOfDataPersistenceWork.Context.PerformanceCurveLibrary.Any(_ => _.Id == libraryId))
            {
                return;
            }

            var libraryToDelete = _unitOfDataPersistenceWork.Context.PerformanceCurveLibrary.Single(_ => _.Id == libraryId);

            _unitOfDataPersistenceWork.Context.PerformanceCurveLibrary.Remove(libraryToDelete);

            _unitOfDataPersistenceWork.Context.SaveChanges();

            var equationsToDelete = _unitOfDataPersistenceWork.Context.Equation
                .Where(_ => _.AttributeEquationCriterionLibraryJoin == null && _.ConditionalTreatmentConsequenceEquationJoin == null &&
                            _.PerformanceCurveEquationJoin == null && _.TreatmentCostEquationJoin == null)
                .ToList();

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.Equation.RemoveRange(equationsToDelete);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkDelete(equationsToDelete);
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }
    }
}
