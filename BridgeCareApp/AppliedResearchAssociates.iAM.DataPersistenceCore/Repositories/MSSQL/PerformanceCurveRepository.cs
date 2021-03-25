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

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfWork;

        public PerformanceCurveRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreatePerformanceCurveLibrary(string name, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            var performanceCurveLibraryEntity = new PerformanceCurveLibraryEntity { Id = Guid.NewGuid(), Name = name };

            _unitOfWork.Context.AddEntity(performanceCurveLibraryEntity);

            _unitOfWork.Context.AddEntity(new PerformanceCurveLibrarySimulationEntity
            {
                PerformanceCurveLibraryId = performanceCurveLibraryEntity.Id,
                SimulationId = simulationId
            });
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
                throw new RowNotInTableException($"No performance curve library found for simulation having id {simulationId}.");
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
                .Select(_ => _.ToEntity(simulationEntity.PerformanceCurveLibrarySimulationJoin.PerformanceCurveLibraryId,
                    attributeEntities.Single(__ => __.Name == _.Attribute.Name).Id))
                .ToList();

            _unitOfWork.Context.AddAll(performanceCurveEntities);

            if (performanceCurves.Any(_ => !_.Equation.ExpressionIsBlank))
            {
                var equationEntityPerPerformanceCurveEntityId = performanceCurves
                    .Where(_ => !_.Equation.ExpressionIsBlank)
                    .ToDictionary(_ => _.Id, _ => _.Equation.ToEntity());

                _unitOfWork.EquationRepo.CreateEquations(equationEntityPerPerformanceCurveEntityId, DataPersistenceConstants.EquationJoinEntities.PerformanceCurve);
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
                .Single(_ => _.Id == simulation.Id);

            simulationEntity.PerformanceCurveLibrarySimulationJoin?.PerformanceCurveLibrary?.PerformanceCurves?.ForEach(
                _ => _.CreatePerformanceCurve(simulation));
        }

        public Task<List<PerformanceCurveLibraryDTO>> PerformanceCurveLibrariesWithPerformanceCurves()
        {
            if (!_unitOfWork.Context.PerformanceCurveLibrary.Any())
            {
                return Task.Factory.StartNew(() => new List<PerformanceCurveLibraryDTO>());
            }

            return Task.Factory.StartNew(() => _unitOfWork.Context.PerformanceCurveLibrary
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

        public void UpsertPermitted(UserInfoDTO userInfo, Guid simulationId, PerformanceCurveLibraryDTO dto)
        {
            if (simulationId != Guid.Empty)
            {
                if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
                {
                    throw new RowNotInTableException($"No simulation found having id {dto.Id}");
                }

                if (!_unitOfWork.Context.Simulation.Any(_ =>
                    _.Id == dto.Id && _.SimulationUserJoins.Any(__ => __.User.Username == userInfo.Sub && __.CanModify)))
                {
                    throw new UnauthorizedAccessException("You are not authorized to modify this simulation.");
                }
            }

            UpsertPerformanceCurveLibrary(dto, simulationId, userInfo);
            UpsertOrDeletePerformanceCurves(dto.PerformanceCurves, dto.Id, userInfo);
        }

        public void UpsertPerformanceCurveLibrary(PerformanceCurveLibraryDTO dto, Guid simulationId, UserInfoDTO userInfo)
        {
            var userEntity = _unitOfWork.Context.User.SingleOrDefault(_ => _.Username == userInfo.Sub);

            var performanceCurveLibraryEntity = dto.ToEntity();

            _unitOfWork.Context.Upsert(performanceCurveLibraryEntity, dto.Id, userEntity?.Id);

            if (simulationId != Guid.Empty)
            {
                if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
                {
                    throw new RowNotInTableException($"No simulation found having id {simulationId}.");
                }

                _unitOfWork.Context.DeleteEntity<PerformanceCurveLibrarySimulationEntity>(_ => _.SimulationId == simulationId);

                _unitOfWork.Context.AddEntity(new PerformanceCurveLibrarySimulationEntity
                {
                    PerformanceCurveLibraryId = performanceCurveLibraryEntity.Id,
                    SimulationId = simulationId
                }, userEntity?.Id);
            }
        }

        public void UpsertOrDeletePerformanceCurves(List<PerformanceCurveDTO> performanceCurves, Guid libraryId, UserInfoDTO userInfo)
        {
            if (!_unitOfWork.Context.PerformanceCurveLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException($"No performance curve library found having id {libraryId}.");
            }

            if (performanceCurves.Any(_ => string.IsNullOrEmpty(_.Attribute)))
            {
                throw new InvalidOperationException("All performance curves must have an attribute.");
            }

            var userEntity = _unitOfWork.Context.User.SingleOrDefault(_ => _.Username == userInfo.Sub);

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
                .Select(_ => _.ToEntity(libraryId, attributeEntities.Single(__ => __.Name == _.Attribute).Id))
                .ToList();

            var entityIds = performanceCurves.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.PerformanceCurve
                .Where(_ => _.PerformanceCurveLibrary.Id == libraryId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            var predicatesPerCrudOperation = new Dictionary<string, Expression<Func<PerformanceCurveEntity, bool>>>
            {
                {"delete", _ => _.PerformanceCurveLibrary.Id == libraryId && !entityIds.Contains(_.Id)},
                {"update", _ => existingEntityIds.Contains(_.Id)},
                {"add", _ => !existingEntityIds.Contains(_.Id)}
            };

            _unitOfWork.Context.DeleteAll<EquationEntity>(_ =>
                _.PerformanceCurveEquationJoin.PerformanceCurve.PerformanceCurveLibraryId == libraryId);

            if (IsRunningFromXUnit)
            {
                _unitOfWork.Context.UpsertOrDelete(performanceCurveEntities, predicatesPerCrudOperation, userEntity?.Id);
            }
            else
            {
                _unitOfWork.Context.BulkUpsertOrDelete(performanceCurveEntities, predicatesPerCrudOperation, userEntity?.Id);
            }

            _unitOfWork.Context.DeleteAll<CriterionLibraryPerformanceCurveEntity>(_ =>
                _.PerformanceCurve.PerformanceCurveLibraryId == libraryId);

            if (performanceCurves.Any(_ => _.Equation?.Id != null && _.Equation?.Id != Guid.Empty && !string.IsNullOrEmpty(_.Equation.Expression)))
            {
                var equationEntityPerPerformanceCurveEntityId = performanceCurves
                    .Where(_ => _.Equation?.Id != null && _.Equation?.Id != Guid.Empty && !string.IsNullOrEmpty(_.Equation?.Expression))
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
                        CriterionLibraryId = _.CriterionLibrary.Id,
                        PerformanceCurveId = _.Id
                    })
                    .ToList();

                _unitOfWork.Context.AddAll(curveCriterionJoinsToAdd, userEntity?.Id);
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
    }
}
