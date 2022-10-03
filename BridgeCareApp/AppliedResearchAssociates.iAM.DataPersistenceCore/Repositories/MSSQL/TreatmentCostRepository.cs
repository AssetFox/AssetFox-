﻿using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using System.Data;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class TreatmentCostRepository : ITreatmentCostRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public TreatmentCostRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ??
                                         throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateScenarioTreatmentCosts(Dictionary<Guid, List<TreatmentCost>> treatmentCostsPerTreatmentId, string simulationName)
        {
            var costEntities = new List<ScenarioTreatmentCostEntity>();
            var equationEntities = new List<EquationEntity>();
            var equationJoinEntities = new List<ScenarioTreatmentCostEquationEntity>();
            var criterionEntities = new List<CriterionLibraryEntity>();
            var criterionJoinEntities = new List<CriterionLibraryScenarioTreatmentCostEntity>();

            treatmentCostsPerTreatmentId.Keys.ForEach(treatmentId =>
            {
                costEntities.AddRange(treatmentCostsPerTreatmentId[treatmentId].Select(_ =>
                {
                    var costEntity = _.ToScenarioEntity(treatmentId);

                    if (!_.Equation.ExpressionIsBlank)
                    {
                        var equationEntity = _.Equation.ToEntity();
                        equationEntities.Add(equationEntity);
                        equationJoinEntities.Add(new ScenarioTreatmentCostEquationEntity
                        {
                            ScenarioTreatmentCostId = _.Id, EquationId = equationEntity.Id
                        });
                    }

                    if (!_.Criterion.ExpressionIsBlank)
                    {
                        var criterionEntity = _.Criterion.ToEntity("");
                        criterionEntity.IsSingleUse = true;
                        criterionEntities.Add(criterionEntity);
                        criterionJoinEntities.Add(new CriterionLibraryScenarioTreatmentCostEntity
                        {
                            CriterionLibraryId = criterionEntity.Id, ScenarioTreatmentCostId = _.Id
                        });
                    }

                    return costEntity;
                }).ToList());
            });

            _unitOfWork.Context.AddAll(costEntities);
            _unitOfWork.Context.AddAll(equationEntities);
            _unitOfWork.Context.AddAll(equationJoinEntities);
            _unitOfWork.Context.AddAll(criterionEntities);
            _unitOfWork.Context.AddAll(criterionJoinEntities);
        }

        public void UpsertOrDeleteTreatmentCosts(Dictionary<Guid, List<TreatmentCostDTO>> treatmentCostPerTreatmentId, Guid libraryId)
        {
            var treatmentCostEntities = treatmentCostPerTreatmentId
                .SelectMany(_ => _.Value.Select(cost => cost.ToLibraryEntity(_.Key)))
                .ToList();

            var entityIds = treatmentCostEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.TreatmentCost.AsNoTracking()
                .Where(_ => _.SelectableTreatment.TreatmentLibraryId == libraryId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            _unitOfWork.Context.DeleteAll<TreatmentCostEntity>(_ =>
                _.SelectableTreatment.TreatmentLibraryId == libraryId && !entityIds.Contains(_.Id));

            _unitOfWork.Context.UpdateAll(treatmentCostEntities.Where(_ => existingEntityIds.Contains(_.Id)).ToList());

            _unitOfWork.Context.AddAll(treatmentCostEntities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList());

            var treatmentCosts = treatmentCostPerTreatmentId.SelectMany(_ => _.Value).ToList();

            if (treatmentCosts.Any(_ =>
                _.Equation?.Id != null && _.Equation?.Id != Guid.Empty && !string.IsNullOrEmpty(_.Equation.Expression)))
            {
                var equationsJoins = new List<TreatmentCostEquationEntity>();

                var equations = treatmentCosts
                    .Where(_ => _.Equation?.Id != null && _.Equation?.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.Equation.Expression))
                    .Select(curve =>
                    {
                        var equation = new EquationEntity
                        {
                            Id = Guid.NewGuid(),
                            Expression = curve.Equation.Expression,
                        };
                        equationsJoins.Add(new TreatmentCostEquationEntity
                        {
                            EquationId = equation.Id,
                            TreatmentCostId = curve.Id
                        });
                        return equation;
                    }).ToList();
                _unitOfWork.Context.AddAll(equations, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(equationsJoins, _unitOfWork.UserEntity?.Id);
            }

            if (treatmentCosts.Any(_ =>
                _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
            {
                var criterionJoins = new List<CriterionLibraryTreatmentCostEntity>();

                var criteria = treatmentCosts
                    .Where(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression))
                    .Select(treatment =>
                    {
                        var criterion = new CriterionLibraryEntity
                        {
                            Id = Guid.NewGuid(),
                            MergedCriteriaExpression = treatment.CriterionLibrary.MergedCriteriaExpression,
                            Name = $"{treatment} Criterion",
                            IsSingleUse = true
                        };
                        criterionJoins.Add(new CriterionLibraryTreatmentCostEntity
                        {
                            CriterionLibraryId = criterion.Id,
                            TreatmentCostId = treatment.Id
                        });
                        return criterion;
                    }).ToList();

                _unitOfWork.Context.AddAll(criteria, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criterionJoins, _unitOfWork.UserEntity?.Id);
            }
        }

        public void UpsertOrDeleteScenarioTreatmentCosts(Dictionary<Guid, List<TreatmentCostDTO>> scenarioTreatmentCostPerTreatmentId,
            Guid SimulationId)
        {
            var scenarioTreatmentCostEntities = scenarioTreatmentCostPerTreatmentId
                .SelectMany(_ => _.Value.Select(cost => cost.ToScenarioEntity(_.Key)))
                .ToList();

            var entityIds = scenarioTreatmentCostEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.ScenarioTreatmentCost.AsNoTracking()
                .Where(_ => _.ScenarioSelectableTreatment.SimulationId == SimulationId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            _unitOfWork.Context.DeleteAll<ScenarioTreatmentCostEntity>(_ =>
                _.ScenarioSelectableTreatment.SimulationId == SimulationId && !entityIds.Contains(_.Id));

            _unitOfWork.Context.UpdateAll(scenarioTreatmentCostEntities.Where(_ => existingEntityIds.Contains(_.Id)).ToList());

            _unitOfWork.Context.AddAll(scenarioTreatmentCostEntities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList());

            var treatmentCosts = scenarioTreatmentCostPerTreatmentId.SelectMany(_ => _.Value).ToList();

            if (treatmentCosts.Any(_ =>
                _.Equation?.Id != null && _.Equation?.Id != Guid.Empty && !string.IsNullOrEmpty(_.Equation.Expression)))
            {
                var equationsJoins = new List<ScenarioTreatmentCostEquationEntity>();

                var equations = treatmentCosts
                    .Where(_ => _.Equation?.Id != null && _.Equation?.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.Equation.Expression))
                    .Select(curve =>
                    {
                        var equationEntity = new EquationEntity
                        {
                            Id = Guid.NewGuid(),
                            Expression = curve.Equation.Expression,
                        };
                        equationsJoins.Add(new ScenarioTreatmentCostEquationEntity
                        {
                            EquationId = equationEntity.Id,
                            ScenarioTreatmentCostId = curve.Id
                        });
                        return equationEntity;
                    }).ToList();
                _unitOfWork.Context.AddAll(equations, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(equationsJoins, _unitOfWork.UserEntity?.Id);
            }

            if (treatmentCosts.Any(_ =>
                _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
            {
                var criterionJoins = new List<CriterionLibraryScenarioTreatmentCostEntity>();

                var criteria = treatmentCosts
                    .Where(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression))
                    .Select(treatment =>
                    {
                        var criterion = new CriterionLibraryEntity
                        {
                            Id = Guid.NewGuid(),
                            MergedCriteriaExpression = treatment.CriterionLibrary.MergedCriteriaExpression,
                            Name = $"{treatment} Criterion",
                            IsSingleUse = true
                        };
                        criterionJoins.Add(new CriterionLibraryScenarioTreatmentCostEntity
                        {
                            CriterionLibraryId = criterion.Id,
                            ScenarioTreatmentCostId = treatment.Id
                        });
                        return criterion;
                    }).ToList();

                _unitOfWork.Context.AddAll(criteria, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criterionJoins, _unitOfWork.UserEntity?.Id);
            }
        }

        public List<TreatmentCostDTO> GetTreatmentCostByScenariotreatmentId(Guid treatmentId)
        {
            if (!_unitOfWork.Context.ScenarioSelectableTreatment.Any(_ => _.Id == treatmentId))
            {
                throw new RowNotInTableException("The specified scenario treatment was not found.");
            }

            return _unitOfWork.Context.ScenarioTreatmentCost.AsNoTracking()
                .Include(_ => _.CriterionLibraryScenarioTreatmentCostJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.ScenarioTreatmentCostEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Select(_ => _.ToDto())
                .ToList();
        }

        public List<TreatmentCostDTO> GetTreatmentCostByTreatmentId(Guid treatmentId)
        {
            if (!_unitOfWork.Context.SelectableTreatment.Any(_ => _.Id == treatmentId))
            {
                throw new RowNotInTableException("The specified treatment was not found.");
            }

            return _unitOfWork.Context.TreatmentCost.AsNoTracking()
                .Include(_ => _.CriterionLibraryTreatmentCostJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.TreatmentCostEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Select(_ => _.ToDto())
                .ToList();
        }
    }
}
