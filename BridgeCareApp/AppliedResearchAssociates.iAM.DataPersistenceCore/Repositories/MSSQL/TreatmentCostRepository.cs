using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class TreatmentCostRepository : ITreatmentCostRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfWork;

        public TreatmentCostRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ??
                                         throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateTreatmentCosts(Dictionary<Guid, List<TreatmentCost>> treatmentCostsPerTreatmentId, string simulationName)
        {
            var costEntities = new List<TreatmentCostEntity>();
            var equationEntityPerCostEntityId = new Dictionary<Guid, EquationEntity>();
            var costEntityIdsPerExpression = new Dictionary<string, List<Guid>>();

            treatmentCostsPerTreatmentId.Keys.ForEach(treatmentId =>
            {
                var entities = treatmentCostsPerTreatmentId[treatmentId].Select(_ =>
                    {
                        var costEntity = _.ToEntity(treatmentId);

                        if (!_.Equation.ExpressionIsBlank)
                        {
                            equationEntityPerCostEntityId.Add(costEntity.Id, _.Equation.ToEntity());
                        }

                        if (!_.Criterion.ExpressionIsBlank)
                        {
                            if (costEntityIdsPerExpression.ContainsKey(_.Criterion.Expression))
                            {
                                costEntityIdsPerExpression[_.Criterion.Expression].Add(costEntity.Id);
                            }
                            else
                            {
                                costEntityIdsPerExpression.Add(_.Criterion.Expression, new List<Guid>
                                {
                                    costEntity.Id
                                });
                            }
                        }

                        return costEntity;
                    })
                    .ToList();

                costEntities.AddRange(entities);
            });

            _unitOfWork.Context.AddAll(costEntities);

            if (equationEntityPerCostEntityId.Values.Any())
            {
                _unitOfWork.EquationRepo.CreateEquations(equationEntityPerCostEntityId,
                    DataPersistenceConstants.EquationJoinEntities.TreatmentCost);
            }

            if (costEntityIdsPerExpression.Values.Any())
            {
                _unitOfWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(costEntityIdsPerExpression,
                    DataPersistenceConstants.EquationJoinEntities.TreatmentCost, simulationName);
            }
        }

        public void UpsertOrDeleteTreatmentCosts(Dictionary<Guid, List<TreatmentCostDTO>> treatmentCostPerTreatmentId, Guid libraryId)
        {
            var treatmentCostEntities = treatmentCostPerTreatmentId.SelectMany(_ => _.Value.Select(__ => __.ToEntity(_.Key)))
                .ToList();

            var entityIds = treatmentCostEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.TreatmentCost
                .Where(_ => _.SelectableTreatment.TreatmentLibraryId == libraryId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            var predicatesPerCrudOperation = new Dictionary<string, Expression<Func<TreatmentCostEntity, bool>>>
            {
                {"delete", _ => _.SelectableTreatment.TreatmentLibraryId == libraryId && !entityIds.Contains(_.Id)},
                {"update", _ => existingEntityIds.Contains(_.Id)},
                {"add", _ => !existingEntityIds.Contains(_.Id)}
            };

            if (IsRunningFromXUnit)
            {
                _unitOfWork.Context.UpsertOrDelete(treatmentCostEntities, predicatesPerCrudOperation, _unitOfWork.UserEntity?.Id);
            }
            else
            {
                _unitOfWork.Context.BulkUpsertOrDelete(treatmentCostEntities, predicatesPerCrudOperation, _unitOfWork.UserEntity?.Id);
            }

            var treatmentCosts = treatmentCostPerTreatmentId.SelectMany(_ => _.Value).ToList();

            if (treatmentCosts.Any(_ =>
                _.Equation?.Id != null && _.Equation?.Id != Guid.Empty && !string.IsNullOrEmpty(_.Equation.Expression)))
            {
                var equationEntityPerTreatmentCostId = treatmentCosts
                    .Where(_ => _.Equation?.Id != null && _.Equation?.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.Equation.Expression))
                    .ToDictionary(_ => _.Id, _ => _.Equation.ToEntity());

                _unitOfWork.EquationRepo.CreateEquations(equationEntityPerTreatmentCostId,
                    DataPersistenceConstants.EquationJoinEntities.TreatmentCost);
            }

            if (treatmentCosts.Any(_ =>
                _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
            {
                var criterionLibraryJoinsToAdd = treatmentCosts
                    .Where(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)).Select(_ =>
                        new CriterionLibraryTreatmentCostEntity
                        {
                            CriterionLibraryId = _.CriterionLibrary.Id,
                            TreatmentCostId = _.Id
                        }).ToList();

                _unitOfWork.Context.AddAll(criterionLibraryJoinsToAdd, _unitOfWork.UserEntity?.Id);
            }
        }
    }
}
