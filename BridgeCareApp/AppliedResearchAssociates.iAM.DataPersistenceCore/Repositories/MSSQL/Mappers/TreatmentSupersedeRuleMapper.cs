using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using MathNet.Numerics.Statistics.Mcmc;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.RemainingLifeLimit;
using System.Linq;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class TreatmentSupersedeRuleMapper
    {
        public static ScenarioTreatmentSupersedeRuleEntity ToScenarioTreatmentSupersedeRuleEntity(this TreatmentSupersedeRule domain, Guid treatmentId, Guid simulationId)
        {
            var entity = new ScenarioTreatmentSupersedeRuleEntity
            {
                Id = domain.Id,
                TreatmentId = treatmentId,
                PreventTreatmentId = domain.Treatment.Id
            };

            var criterion = domain.Criterion;
            if (criterion != null) // TODO test and update as required
            {
                var criterionLibrary = criterion.ToEntity("");
                var join = new CriterionLibraryScenarioTreatmentSupersedeRuleEntity
                {
                    ScenarioTreatmentSupersedeRuleId = entity.Id,
                    CriterionLibrary = criterionLibrary,
                };
                entity.CriterionLibraryScenarioTreatmentSupersedeRuleJoin = join;
            }

            return entity;
        }        

        public static ScenarioTreatmentSupersedeRuleEntity ToScenarioTreatmentSupersedeRuleEntity(this TreatmentSupersedeRuleDTO treatmentSupersedeRuleDto, Guid treatmentId, BaseEntityProperties baseEntityProperties = null)
        {
            var entity = new ScenarioTreatmentSupersedeRuleEntity
            {
                Id = treatmentSupersedeRuleDto.Id,
                TreatmentId = treatmentId,
                PreventTreatmentId = treatmentSupersedeRuleDto.treatment.Id
            };

            // CriterionLibraryDTO.MergedCriteriaExpression can be empty.
            var criterionLibraryDto = treatmentSupersedeRuleDto.CriterionLibrary;
            if (criterionLibraryDto != null) // TODO test this later via UI
            {
                var criterionLibrary = criterionLibraryDto.ToSingleUseEntity(baseEntityProperties);
                var join = new CriterionLibraryScenarioTreatmentSupersedeRuleEntity
                {
                    ScenarioTreatmentSupersedeRuleId = entity.Id,
                    CriterionLibrary = criterionLibrary,
                };
                BaseEntityPropertySetter.SetBaseEntityProperties(entity, baseEntityProperties);
                BaseEntityPropertySetter.SetBaseEntityProperties(join, baseEntityProperties);
                entity.CriterionLibraryScenarioTreatmentSupersedeRuleJoin = join;
            }
            BaseEntityPropertySetter.SetBaseEntityProperties(entity, baseEntityProperties);

            return entity;
        }

        public static TreatmentSupersedeRuleEntity ToTreatmentSupersedeRuleEntity(this TreatmentSupersedeRuleDTO treatmentSupersedeRuleDto, Guid treatmentId, BaseEntityProperties baseEntityProperties = null)
        {
            var entity = new TreatmentSupersedeRuleEntity
            {
                Id = treatmentSupersedeRuleDto.Id,
                TreatmentId = treatmentId,
                PreventTreatmentId = treatmentSupersedeRuleDto.treatment.Id
            };

            // CriterionLibraryDTO.MergedCriteriaExpression can be empty.
            var criterionLibraryDto = treatmentSupersedeRuleDto.CriterionLibrary;
            if (criterionLibraryDto != null)
            {
                var criterionLibrary = criterionLibraryDto.ToSingleUseEntity(baseEntityProperties);
                var join = new CriterionLibraryTreatmentSupersedeRuleEntity
                {
                    TreatmentSupersedeRuleId = entity.Id,
                    CriterionLibrary = criterionLibrary,
                };
                BaseEntityPropertySetter.SetBaseEntityProperties(entity, baseEntityProperties);
                BaseEntityPropertySetter.SetBaseEntityProperties(join, baseEntityProperties);
                entity.CriterionLibraryTreatmentSupersedeRuleJoin = join;
            }
            BaseEntityPropertySetter.SetBaseEntityProperties(entity, baseEntityProperties);

            return entity;
        }

        public static void CreateTreatmentSupersedeRule(this ScenarioTreatmentSupersedeRuleEntity entity,
            SelectableTreatment selectableTreatment, Simulation simulation, List<ScenarioSelectableTreatmentEntity> simpleTreatments)
        {
            var supersedeRule = selectableTreatment.AddSupersedeRule();
            var preventTreatmentEntity = simpleTreatments?.FirstOrDefault(_ => _.Id == entity.PreventTreatmentId);
            supersedeRule.Treatment = preventTreatmentEntity?.ToDomain(simulation, simpleTreatments);
            // Criterion
            var criterionLibrary = entity.CriterionLibraryScenarioTreatmentSupersedeRuleJoin?.CriterionLibrary;
            if (criterionLibrary != null)
            {
                supersedeRule.Criterion.Expression = criterionLibrary.MergedCriteriaExpression ?? string.Empty;
                supersedeRule.Criterion.Id = criterionLibrary.Id;
            }
        }

        public static TreatmentSupersedeRuleDTO ToDto(this ScenarioTreatmentSupersedeRuleEntity entity, List<TreatmentDTO> treatmentList) =>
            new TreatmentSupersedeRuleDTO
            {
                Id = entity.Id,
                treatment = entity.PreventTreatmentId != Guid.Empty && treatmentList != null ? treatmentList.FirstOrDefault(_ => _.Id == entity.PreventTreatmentId) : new TreatmentDTO(),
                CriterionLibrary = entity.CriterionLibraryScenarioTreatmentSupersedeRuleJoin != null
                    ? entity.CriterionLibraryScenarioTreatmentSupersedeRuleJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO(),
            };

        public static TreatmentSupersedeRuleDTO ToDto(this TreatmentSupersedeRuleEntity entity, List<TreatmentDTO> treatmentList) =>
            new TreatmentSupersedeRuleDTO
            {
                Id = entity.Id,
                treatment = entity.PreventTreatmentId != Guid.Empty && treatmentList != null ? treatmentList.FirstOrDefault(_ => _.Id == entity.PreventTreatmentId) : new TreatmentDTO(),
                CriterionLibrary = entity.CriterionLibraryTreatmentSupersedeRuleJoin != null
                    ? entity.CriterionLibraryTreatmentSupersedeRuleJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO(),
            };

        public static TreatmentSupersedeRuleExportDTO ToExportDto(this ScenarioTreatmentSupersedeRuleEntity entity, List<TreatmentDTO> treatmentList) =>
            new TreatmentSupersedeRuleExportDTO
            {
                Id = entity.Id,
                TreatmentName = treatmentList.FirstOrDefault(_ => _.Id == entity.TreatmentId).Name,
                treatment = entity.PreventTreatmentId != Guid.Empty && treatmentList != null ? treatmentList.FirstOrDefault(_ => _.Id == entity.PreventTreatmentId) : new TreatmentDTO(),
                CriterionLibrary = entity.CriterionLibraryScenarioTreatmentSupersedeRuleJoin != null
                    ? entity.CriterionLibraryScenarioTreatmentSupersedeRuleJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO(),
            };

        public static TreatmentSupersedeRuleExportDTO ToExportDto(this TreatmentSupersedeRuleEntity entity, List<TreatmentDTO> treatmentList) =>
            new TreatmentSupersedeRuleExportDTO
            {
                Id = entity.Id,
                TreatmentName = treatmentList.FirstOrDefault(_ => _.Id == entity.TreatmentId).Name,
                treatment = entity.PreventTreatmentId != Guid.Empty && treatmentList != null ? treatmentList.FirstOrDefault(_ => _.Id == entity.PreventTreatmentId) : new TreatmentDTO(),
                CriterionLibrary = entity.CriterionLibraryTreatmentSupersedeRuleJoin != null
                    ? entity.CriterionLibraryTreatmentSupersedeRuleJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO(),
            };
    }
}
