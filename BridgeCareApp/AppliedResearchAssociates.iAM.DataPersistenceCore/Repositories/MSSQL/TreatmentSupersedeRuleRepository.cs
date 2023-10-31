using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class TreatmentSupersedeRuleRepository : ITreatmentSupersedeRuleRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public TreatmentSupersedeRuleRepository(UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        // TODO add unit test later
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

        public void UpsertOrDeleteScenarioTreatmentSupersedeRules(Dictionary<Guid, List<TreatmentSupersedeRuleDTO>> scenarioTreatmentSupersedeRulesPerTreatmentId, Guid simulationId)
        {
            var scenarioTreatmentSupersedeRuleEntities = scenarioTreatmentSupersedeRulesPerTreatmentId
                .SelectMany(_ => _.Value.Select(supersedeRule => supersedeRule.ToScenarioTreatmentSupersedeRuleEntity(_.Key)))
                .ToList();

            var entityIds = scenarioTreatmentSupersedeRuleEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.ScenarioTreatmentSupersedeRule.AsNoTracking()
                .Where(_ => _.ScenarioSelectableTreatment.SimulationId == simulationId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            _unitOfWork.Context.DeleteAll<ScenarioTreatmentSupersedeRuleEntity>(_ =>
               _.ScenarioSelectableTreatment.SimulationId == simulationId && !entityIds.Contains(_.Id));

            _unitOfWork.Context.UpdateAll(scenarioTreatmentSupersedeRuleEntities.Where(_ => existingEntityIds.Contains(_.Id)).ToList());

            _unitOfWork.Context.AddAll(scenarioTreatmentSupersedeRuleEntities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList());

            var treatmentSupersedeRules = scenarioTreatmentSupersedeRulesPerTreatmentId.SelectMany(_ => _.Value).ToList();
            if (treatmentSupersedeRules.Any(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty))
            {
                var criterionJoins = new List<CriterionLibraryScenarioTreatmentSupersedeRuleEntity>();
                var criteria = treatmentSupersedeRules
                    .Where(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary.Id != Guid.Empty)
                    .Select(treatmentSupersedeRule =>
                    {
                        var criterion = new CriterionLibraryEntity
                        {
                            Id = Guid.NewGuid(),
                            MergedCriteriaExpression = treatmentSupersedeRule.CriterionLibrary.MergedCriteriaExpression,
                            Name = $"{treatmentSupersedeRule} Criterion",
                            IsSingleUse = true
                        };
                        criterionJoins.Add(new CriterionLibraryScenarioTreatmentSupersedeRuleEntity
                        {
                            CriterionLibraryId = criterion.Id,
                            ScenarioTreatmentSupersedeRuleId = treatmentSupersedeRule.Id
                        });
                        return criterion;
                    }).ToList();

                _unitOfWork.Context.AddAll(criteria, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criterionJoins, _unitOfWork.UserEntity?.Id);
            }
        }
    }
}
