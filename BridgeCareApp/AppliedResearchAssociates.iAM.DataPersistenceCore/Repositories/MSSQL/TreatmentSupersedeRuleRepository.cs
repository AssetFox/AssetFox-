using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class TreatmentSupersedeRuleRepository : ITreatmentSupersedeRuleRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public TreatmentSupersedeRuleRepository(UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateTreatmentSupersedeRules(Dictionary<Guid, List<TreatmentSupersedeRule>> treatmentSupersedeRulesPerTreatmentId,
            string simulationName, Guid simulationId)
        {
            var supersedeRuleEntityIdsPerExpression = new Dictionary<string, List<Guid>>();

            var supersedeRuleEntities = treatmentSupersedeRulesPerTreatmentId.SelectMany(_ =>
                _.Value.Select(__ =>
                {
                    var entity = __.ToScenarioTreatmentSupersedeRuleEntity(_.Key, simulationId);

                    if (!__.Criterion.ExpressionIsBlank)
                    {
                        if (supersedeRuleEntityIdsPerExpression.ContainsKey(__.Criterion.Expression))
                        {
                            supersedeRuleEntityIdsPerExpression[__.Criterion.Expression].Add(entity.Id);
                        }
                        else
                        {
                            supersedeRuleEntityIdsPerExpression.Add(__.Criterion.Expression, new List<Guid> { entity.Id });
                        }
                    }

                    return entity;
                }))
                .ToList();

            _unitOfWork.Context.AddAll(supersedeRuleEntities);

            if (supersedeRuleEntityIdsPerExpression.Values.Any())
            {
                _unitOfWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(
                    supersedeRuleEntityIdsPerExpression,
                    DataPersistenceConstants.CriterionLibraryJoinEntities.TreatmentSupersedeRule, simulationName);
            }
        }

        public void UpsertScenarioTreatmentSupersedeRule(Dictionary<Guid, List<TreatmentSupersedeRule>> scenarioTreatmentSupersedeRulePerTreatmentId, Guid simulationId)
        {
            var scenarioTreatmentSupersedeRuleEntities = scenarioTreatmentSupersedeRulePerTreatmentId
              .SelectMany(_ => _.Value.Select(rule => rule
                  .ToScenarioEntity(_.Key)))
              .ToList();

            var entityIds = scenarioTreatmentSupersedeRuleEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.ScenarioTreatmentPerformanceFactor.AsNoTracking()
                .Where(_ => _.ScenarioSelectableTreatment.SimulationId == simulationId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            _unitOfWork.Context.UpdateAll(scenarioTreatmentSupersedeRuleEntities.Where(_ => existingEntityIds.Contains(_.Id)).ToList());
            _unitOfWork.Context.AddAll(scenarioTreatmentSupersedeRuleEntities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList());
        }

    }
}
