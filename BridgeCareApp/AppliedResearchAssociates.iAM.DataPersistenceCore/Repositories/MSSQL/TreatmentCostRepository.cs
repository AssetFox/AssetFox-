using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class TreatmentCostRepository : ITreatmentCostRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public TreatmentCostRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfDataPersistenceWork) =>
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ??
                                         throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));

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

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.TreatmentCost.AddRange(costEntities);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkInsert(costEntities);
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();

            if (equationEntityPerCostEntityId.Values.Any())
            {
                _unitOfDataPersistenceWork.EquationRepo.CreateEquations(equationEntityPerCostEntityId,
                    DataPersistenceConstants.EquationJoinEntities.TreatmentCost);
            }

            if (costEntityIdsPerExpression.Values.Any())
            {
                _unitOfDataPersistenceWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(costEntityIdsPerExpression,
                    DataPersistenceConstants.EquationJoinEntities.TreatmentCost, simulationName);
            }
        }

        public void UpsertOrDeleteTreatmentCosts(Dictionary<Guid, List<TreatmentCostDTO>> treatmentCostPerTreatmentId, Guid libraryId)
        {
            var entities = treatmentCostPerTreatmentId.SelectMany(_ => _.Value.Select(__ => __.ToEntity(_.Key)))
                .ToList();

            var entityIds = entities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfDataPersistenceWork.Context.TreatmentCost
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
                _unitOfDataPersistenceWork.Context.UpsertOrDelete(entities, predicatesPerCrudOperation);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkUpsertOrDelete(entities, predicatesPerCrudOperation);
            }

            var treatmentCosts = treatmentCostPerTreatmentId.SelectMany(_ => _.Value).ToList();

            if (treatmentCosts.Any(_ =>
                _.Equation?.Id != null && _.Equation?.Id != Guid.Empty && !string.IsNullOrEmpty(_.Equation.Expression)))
            {
                var equationEntityPerTreatmentCostId = treatmentCosts
                    .Where(_ => _.Equation?.Id != null && _.Equation?.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.Equation.Expression))
                    .ToDictionary(_ => _.Id, _ => _.Equation.ToEntity());

                _unitOfDataPersistenceWork.EquationRepo.CreateEquations(equationEntityPerTreatmentCostId,
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

                if (IsRunningFromXUnit)
                {
                    _unitOfDataPersistenceWork.Context.CriterionLibraryTreatmentCost.AddRange(criterionLibraryJoinsToAdd);
                }
                else
                {
                    _unitOfDataPersistenceWork.Context.BulkInsert(criterionLibraryJoinsToAdd);
                }
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }
    }
}
