using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using System.Data;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class TreatmentSupersedeRuleRepository : ITreatmentSupersedeRuleRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        public const string TreatmentNotFoundErrorMessage = "The provided treatment was not found";
        public const string ScenarioTreatmentNotFoundErrorMessage = "The provided scenario treatment was not found";

        public TreatmentSupersedeRuleRepository(UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));                

        public void UpsertOrDeleteScenarioTreatmentSupersedeRules(Dictionary<Guid, List<TreatmentSupersedeRuleDTO>> scenarioTreatmentSupersedeRulesPerTreatmentId, Guid simulationId)
        {
            var scenarioTreatmentSupersedeRuleEntities = scenarioTreatmentSupersedeRulesPerTreatmentId
                .SelectMany(_ => _.Value.Select(supersedeRule => supersedeRule.ToScenarioTreatmentSupersedeRuleEntity(_.Key)))
                .ToList();

            var entityIds = scenarioTreatmentSupersedeRuleEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.ScenarioTreatmentSupersedeRule.AsNoTracking().Include(_ => _.ScenarioSelectableTreatment)
                .Where(_ => _.ScenarioSelectableTreatment.SimulationId == simulationId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            _unitOfWork.Context.DeleteAll<ScenarioTreatmentSupersedeRuleEntity>(_ =>
               _.ScenarioSelectableTreatment.SimulationId == simulationId && !entityIds.Contains(_.Id));

            _unitOfWork.Context.UpdateAll(scenarioTreatmentSupersedeRuleEntities.Where(_ => existingEntityIds.Contains(_.Id)).ToList());

            _unitOfWork.Context.AddAll(scenarioTreatmentSupersedeRuleEntities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList());

            _unitOfWork.Context.DeleteAll<CriterionLibraryScenarioTreatmentSupersedeRuleEntity>(_ =>
                _.ScenarioTreatmentSupersedeRule.ScenarioSelectableTreatment.SimulationId == simulationId && existingEntityIds.Contains(_.ScenarioTreatmentSupersedeRuleId));

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
                            Name = treatmentSupersedeRule.CriterionLibrary.Name + " ScenarioTreatmentSupersedeRuleCriterion",
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

        public List<TreatmentSupersedeRuleDTO> GetScenarioTreatmentSupersedeRules(Guid treatmentId, Guid simulationId)
        {
            if (!_unitOfWork.Context.ScenarioSelectableTreatment.Any(_ => _.Id == treatmentId))
            {
                throw new RowNotInTableException(ScenarioTreatmentNotFoundErrorMessage);
            }

            var treatments = _unitOfWork.Context.ScenarioSelectableTreatment.Where(_ => _.SimulationId == simulationId).Select(_ => _.ToDto(null));
            return _unitOfWork.Context.ScenarioTreatmentSupersedeRule.AsNoTracking()
                .Include(_ => _.CriterionLibraryScenarioTreatmentSupersedeRuleJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Where(_ => _.ScenarioSelectableTreatment.Id == treatmentId)
                .Select(_ => _.ToDto(treatments.ToList()))
                .ToList();
        }

        public void UpsertOrDeleteTreatmentSupersedeRules(Dictionary<Guid, List<TreatmentSupersedeRuleDTO>> supersedeRulesPerTreatmentId, Guid libraryId)
        {
            var treatmentSupersedeRuleEntities = supersedeRulesPerTreatmentId
                .SelectMany(_ => _.Value.Select(supersedeRule => supersedeRule.ToTreatmentSupersedeRuleEntity(_.Key)))
                .ToList();

            var entityIds = treatmentSupersedeRuleEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.TreatmentSupersedeRule.AsNoTracking()
                .Where(_ => _.SelectableTreatment.TreatmentLibraryId == libraryId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            _unitOfWork.Context.DeleteAll<TreatmentSupersedeRuleEntity>(_ =>
               _.SelectableTreatment.TreatmentLibraryId == libraryId && !entityIds.Contains(_.Id));

            _unitOfWork.Context.UpdateAll(treatmentSupersedeRuleEntities.Where(_ => existingEntityIds.Contains(_.Id)).ToList());

            _unitOfWork.Context.AddAll(treatmentSupersedeRuleEntities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList());

            _unitOfWork.Context.DeleteAll<CriterionLibraryTreatmentSupersedeRuleEntity>(_ =>
                _.TreatmentSupersedeRule.SelectableTreatment.TreatmentLibraryId == libraryId && existingEntityIds.Contains(_.TreatmentSupersedeRuleId));

            var treatmentSupersedeRules = supersedeRulesPerTreatmentId.SelectMany(_ => _.Value).ToList();
            if (treatmentSupersedeRules.Any(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty))
            {
                var criterionJoins = new List<CriterionLibraryTreatmentSupersedeRuleEntity>();
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
                        criterionJoins.Add(new CriterionLibraryTreatmentSupersedeRuleEntity
                        {
                            CriterionLibraryId = criterion.Id,
                            TreatmentSupersedeRuleId = treatmentSupersedeRule.Id
                        });
                        return criterion;
                    }).ToList();

                _unitOfWork.Context.AddAll(criteria, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criterionJoins, _unitOfWork.UserEntity?.Id);
            }
        }

        public List<TreatmentSupersedeRuleDTO> GetTreatmentSupersedeRules(Guid treatmentId, Guid libraryId)
        {
            if (!_unitOfWork.Context.SelectableTreatment.Any(_ => _.Id == treatmentId))
            {
                throw new RowNotInTableException(TreatmentNotFoundErrorMessage);
            }

            var treatments = _unitOfWork.Context.SelectableTreatment.Where(_ => _.TreatmentLibraryId == libraryId).Select(_ => _.ToDto(null));
            return _unitOfWork.Context.TreatmentSupersedeRule.AsNoTracking()
                .Include(_ => _.CriterionLibraryTreatmentSupersedeRuleJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Where(_ => _.SelectableTreatment.Id == treatmentId)
                .Select(_ => _.ToDto(treatments.ToList()))
                .ToList();
        }
    }
}
