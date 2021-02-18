using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using SQLitePCL;

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

            var performanceCurveLibraryEntity = new PerformanceCurveLibraryEntity {Id = Guid.NewGuid(), Name = name};

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

                _unitOfDataPersistenceWork.EquationRepo.CreateEquations(equationEntityPerPerformanceCurveEntityId, "PerformanceCurveEntity");
            }

            if (performanceCurves.Any(_ => !_.Criterion.ExpressionIsBlank))
            {
                var curveIdsPerExpression = performanceCurves
                    .Where(_ => !_.Criterion.ExpressionIsBlank)
                    .GroupBy(_ => _.Criterion.Expression, _ => _)
                    .ToDictionary(_ => _.Key, _ => _.Select(__ => __.Id).ToList());

                _unitOfDataPersistenceWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(curveIdsPerExpression, "PerformanceCurveEntity", simulationEntity.Name);
            }
        }

        public void GetSimulationPerformanceCurves(Simulation simulation)
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

        public void AddOrUpdatePerformanceCurveLibrary(PerformanceCurveLibraryDTO dto, Guid? simulationId)
        {
            var performanceCurveLibraryEntity = new PerformanceCurveLibraryEntity { Id = dto.Id, Name = dto.Name };

            _unitOfDataPersistenceWork.Context.AddOrUpdate(performanceCurveLibraryEntity, dto.Id);

            if (simulationId.HasValue && simulationId != Guid.Empty)
            {
                if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId.Value))
                {
                    throw new RowNotInTableException($"No simulation found having id {simulationId.Value}.");
                }

                _unitOfDataPersistenceWork.Context.AddOrUpdate(new PerformanceCurveLibrarySimulationEntity
                {
                    PerformanceCurveLibraryId = performanceCurveLibraryEntity.Id,
                    SimulationId = simulationId.Value
                }, _ => _.SimulationId == simulationId.Value);
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        public void AddOrUpdateOrDeletePerformanceCurves(List<PerformanceCurveDTO> performanceCurves, Guid libraryId)
        {
            if (!_unitOfDataPersistenceWork.Context.PerformanceCurveLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException($"No performance curve library found having id {libraryId}.");
            }

            var attributeNames = performanceCurves.Select(_ => _.Attribute).Distinct().ToList();
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
                .Select(_ => _.ToEntity(libraryId, attributeEntities.Single(__ => __.Name == _.Attribute).Id))
                .ToList();

            if (IsRunningFromXUnit)
            {
                foreach (var entity in performanceCurveEntities)
                {
                    _unitOfDataPersistenceWork.Context.AddOrUpdate(entity, entity.Id);
                }
            }
            else
            {
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

                _unitOfDataPersistenceWork.Context.BulkAddOrUpdateOrDelete(performanceCurveEntities, predicatesPerCrudOperation);
            }

            // delete existing performance curve equations and joins
            var curveEquationJoinsToDelete = _unitOfDataPersistenceWork.Context.PerformanceCurveEquation
                .Where(_ => _.PerformanceCurve.PerformanceCurveLibraryId == libraryId)
                .ToList();
            var equationIds = curveEquationJoinsToDelete.Select(_ => _.EquationId).ToList();

            var equationsToDelete = _unitOfDataPersistenceWork.Context.Equation
                .Where(_ => (_.AttributeEquationCriterionLibraryJoin == null &&
                            _.ConditionalTreatmentConsequenceEquationJoin == null &&
                            _.PerformanceCurveEquationJoin == null && _.TreatmentCostEquationJoin == null) ||
                            equationIds.Contains(_.Id))
                .ToList();

            // delete existing performance curve criterion library joins
            var curveCriterionJoinsToDelete = _unitOfDataPersistenceWork.Context.CriterionLibraryPerformanceCurve
                .Where(_ => _.PerformanceCurve.PerformanceCurveLibraryId == libraryId)
                .ToList();

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.PerformanceCurveEquation.RemoveRange(curveEquationJoinsToDelete);
                _unitOfDataPersistenceWork.Context.Equation.RemoveRange(equationsToDelete);
                _unitOfDataPersistenceWork.Context.CriterionLibraryPerformanceCurve.RemoveRange(curveCriterionJoinsToDelete);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkDelete(curveEquationJoinsToDelete);
                _unitOfDataPersistenceWork.Context.BulkDelete(equationsToDelete);
                _unitOfDataPersistenceWork.Context.BulkDelete(curveCriterionJoinsToDelete);
            }

            // create performance curve equations and their joins
            if (performanceCurves.Any(_ => _.Equation?.Id != null && _.Equation?.Id != Guid.Empty && !string.IsNullOrEmpty(_.Equation.Expression)))
            {
                var equationEntityPerPerformanceCurveEntityId = performanceCurves
                    .Where(_ => _.Equation?.Id != null && _.Equation?.Id != Guid.Empty && !string.IsNullOrEmpty(_.Equation?.Expression))
                    .ToDictionary(_ => _.Id, _ => _.Equation.ToEntity());

                _unitOfDataPersistenceWork.EquationRepo.CreateEquations(equationEntityPerPerformanceCurveEntityId, "PerformanceCurveEntity");
            }

            // create performance curve criterion library joins
            if (performanceCurves.Any(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty && !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
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

        public Task<List<PerformanceCurveLibraryDTO>> GetPerformanceCurveLibrariesWithPerformanceCurves()
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
                .Select(_ => _.ToDto())
                .ToList());
        }

        public Task<PerformanceCurveLibraryDTO> GetSimulationPerformanceCurveLibraryWithPerformanceCurves(Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulations found for network having id {simulationId}");
            }

            if (!_unitOfDataPersistenceWork.Context.PerformanceCurveLibrarySimulation.Any(_ => _.SimulationId == simulationId))
            {
                return Task.Factory.StartNew(() => new PerformanceCurveLibraryDTO());
            }

            return Task.Factory.StartNew(() => _unitOfDataPersistenceWork.Context.PerformanceCurveLibrarySimulation
                .Include(_ => _.PerformanceCurveLibrary)
                .ThenInclude(_ => _.PerformanceCurves)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.PerformanceCurveLibrary)
                .ThenInclude(_ => _.PerformanceCurves)
                .ThenInclude(_ => _.CriterionLibraryPerformanceCurveJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.PerformanceCurveLibrary)
                .ThenInclude(_ => _.PerformanceCurves)
                .ThenInclude(_ => _.PerformanceCurveEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Single(_ => _.SimulationId == simulationId)
                .PerformanceCurveLibrary.ToDto());
        }

        public void DeletePerformanceCurveLibrary(Guid libraryId)
        {
            if (!_unitOfDataPersistenceWork.Context.PerformanceCurveLibrary.Any(_ => _.Id == libraryId))
            {
                return;
            }

            var libraryToDelete = _unitOfDataPersistenceWork.Context.PerformanceCurveLibrary.Single(_ => _.Id == libraryId);

            _unitOfDataPersistenceWork.Context.PerformanceCurveLibrary.Remove(libraryToDelete);

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
