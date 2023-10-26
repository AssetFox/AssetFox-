using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using MathNet.Numerics.Statistics.Mcmc;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.RemainingLifeLimit;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class TreatmentSupersedeRuleMapper
    {
        public static TreatmentSupersedeRuleEntity ToTreatmentSupersedeRuleEntity(this TreatmentSupersedeRule domain, Guid treatmentId) =>
            new TreatmentSupersedeRuleEntity
            {
                Id = domain.Id,
                TreatmentId = treatmentId
            };

        public static ScenarioTreatmentSupersedeRuleEntity ToScenarioTreatmentSupersedeRuleEntity(this TreatmentSupersedeRule domain, Guid treatmentId, Guid simulationId)
        {
            var entity = new ScenarioTreatmentSupersedeRuleEntity
            {
                Id = domain.Id,
                TreatmentId = treatmentId,
                ScenarioSelectableTreatment = domain.Treatment.ToScenarioEntity(simulationId),

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

        public static ScenarioTreatmentSupersedeRuleEntity ToScenarioTreatmentSupersedeRuleEntity(this TreatmentSupersedeRuleDTO treatmentSupersedeRuleDto, BaseEntityProperties baseEntityProperties, Guid treatmentId, Guid simulationId)
        {
            var entity = new ScenarioTreatmentSupersedeRuleEntity();
            entity.Id = treatmentSupersedeRuleDto.Id;
            entity.TreatmentId = treatmentId;
            entity.ScenarioSelectableTreatment = treatmentSupersedeRuleDto.treatment.ToScenarioEntity(simulationId);

            // CriterionLibraryDTO.MergedCriteriaExpression can be empty.
            var criterionLibraryDto = treatmentSupersedeRuleDto.CriterionLibrary;
            if (criterionLibraryDto != null) // TODO test this later via UI
            {
                var criterionLibrary = criterionLibraryDto.ToEntity(baseEntityProperties); // TODO Should ToSingleUseEntity be used?
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

        public static void CreateTreatmentSupersedeRule(this ScenarioTreatmentSupersedeRuleEntity entity,
            SelectableTreatment selectableTreatment, Simulation simulation)
        {
            var supersedeRule = selectableTreatment.AddSupersedeRule();
            supersedeRule.Treatment = entity.ScenarioSelectableTreatment.CreateSelectableTreatment(simulation);
            supersedeRule.Criterion.Expression =
                entity.CriterionLibraryScenarioTreatmentSupersedeRuleJoin?.CriterionLibrary.MergedCriteriaExpression ??
                string.Empty;
        }

        public static TreatmentSupersedeRuleDTO ToDto(this ScenarioTreatmentSupersedeRuleEntity entity) =>
            new TreatmentSupersedeRuleDTO
            {
                Id = entity.Id,
                treatment = entity.ScenarioSelectableTreatment.ToDto(),
                CriterionLibrary = entity.CriterionLibraryScenarioTreatmentSupersedeRuleJoin != null
                    ? entity.CriterionLibraryScenarioTreatmentSupersedeRuleJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO(),                
            };

        public static TreatmentSupersedeRuleDTO ToDto(this TreatmentSupersedeRuleEntity entity) =>
            new TreatmentSupersedeRuleDTO
            {
                Id = entity.Id,
                treatment = entity.SelectableTreatment.ToDto(),
                CriterionLibrary = entity.CriterionLibraryTreatmentSupersedeRuleJoin != null
                    ? entity.CriterionLibraryTreatmentSupersedeRuleJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO(),
            };

        public static TreatmentSupersedeRuleDTO ToEntity(this TreatmentSupersedeRuleDTO dto) =>
         new TreatmentSupersedeRuleEntity
         {
             Id = entity.Id,
             treatment = entity.SelectableTreatment.ToDto(),
             CriterionLibrary = entity.CriterionLibraryTreatmentSupersedeRuleJoin != null
                    ? entity.CriterionLibraryTreatmentSupersedeRuleJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO(),
         };
    }
}
