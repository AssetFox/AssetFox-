using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using Attribute = AppliedResearchAssociates.iAM.Analysis.Attribute;
using AppliedResearchAssociates.iAM.DTOs.Static;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class ConditionalTreatmentConsequenceMapper
    {
        public static ScenarioConditionalTreatmentConsequenceEntity ToScenarioEntity(this ConditionalTreatmentConsequence domain, Guid treatmentId, Guid attributeId) =>
            new ScenarioConditionalTreatmentConsequenceEntity
            {
                Id = domain.Id,
                ScenarioSelectableTreatmentId = treatmentId,
                AttributeId = attributeId,
                ChangeValue = domain.Change.Expression
            };

        public static ConditionalTreatmentConsequenceEntity ToLibraryEntity(this TreatmentConsequenceDTO dto, Guid treatmentId,
            Guid attributeId) =>
            new ConditionalTreatmentConsequenceEntity
            {
                Id = dto.Id,
                SelectableTreatmentId = treatmentId,
                AttributeId = attributeId,
                ChangeValue = dto.ChangeValue
            };

        public static ScenarioConditionalTreatmentConsequenceEntity ToScenarioEntity(this TreatmentConsequenceDTO dto, Guid treatmentId,
            Guid attributeId) =>
            new ScenarioConditionalTreatmentConsequenceEntity
            {
                Id = dto.Id,
                ScenarioSelectableTreatmentId = treatmentId,
                AttributeId = attributeId,
                ChangeValue = dto.ChangeValue
            };

        public static ScenarioConditionalTreatmentConsequenceEntity ToScenarioEntityWithCriterionLibraryJoin(this TreatmentConsequenceDTO dto, Guid treatmentId, Guid attributeId, BaseEntityProperties baseEntityProperties)
        {

            var entity = ToScenarioEntity(dto,treatmentId, attributeId);
            var criterionLibraryDto = dto.CriterionLibrary;
            var isvalid = criterionLibraryDto.IsValid();
            if (isvalid)
            {
                var criterionLibrary = criterionLibraryDto.ToSingleUseEntity(baseEntityProperties);
                var join = new CriterionLibraryScenarioConditionalTreatmentConsequenceEntity
                {
                    ScenarioConditionalTreatmentConsequenceId = entity.Id,
                    CriterionLibrary = criterionLibrary,
                };
                BaseEntityPropertySetter.SetBaseEntityProperties(entity, baseEntityProperties);
                BaseEntityPropertySetter.SetBaseEntityProperties(join, baseEntityProperties);
                entity.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin = join;
            }
            return entity;
        }


        public static void CreateConditionalTreatmentConsequence(this ScenarioConditionalTreatmentConsequenceEntity entity, SelectableTreatment treatment, IEnumerable<Attribute> attributes)
        {
            var consequence = treatment.AddConsequence();
            consequence.Id = entity.Id;
            consequence.Attribute = entity.Attribute.GetAttributesFromDomain(attributes);
            consequence.Change.Expression = entity.ChangeValue;
            consequence.Criterion.Expression = entity.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin?.CriterionLibrary
                .MergedCriteriaExpression ?? string.Empty;
            consequence.Equation.Expression = entity.ScenarioConditionalTreatmentConsequenceEquationJoin?.Equation.Expression ?? string.Empty;
        }

        public static TreatmentConsequenceDTO ToDto(this ConditionalTreatmentConsequenceEntity entity) =>
            new TreatmentConsequenceDTO
            {
                Id = entity.Id,
                ChangeValue = entity.ChangeValue,
                Attribute = entity.Attribute != null
                    ? entity.Attribute.Name
                    : "",
                Equation = entity.ConditionalTreatmentConsequenceEquationJoin != null
                    ? entity.ConditionalTreatmentConsequenceEquationJoin.Equation.ToDto()
                    : new EquationDTO(),
                CriterionLibrary = entity.CriterionLibraryConditionalTreatmentConsequenceJoin != null
                    ? entity.CriterionLibraryConditionalTreatmentConsequenceJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO()
            };

        public static TreatmentConsequenceDTO ToDto(this ScenarioConditionalTreatmentConsequenceEntity entity) =>
            new TreatmentConsequenceDTO
            {
                Id = entity.Id,
                ChangeValue = entity.ChangeValue,
                Attribute = entity.Attribute != null
                    ? entity.Attribute.Name
                    : "",
                Equation = entity.ScenarioConditionalTreatmentConsequenceEquationJoin != null
                    ? entity.ScenarioConditionalTreatmentConsequenceEquationJoin.Equation.ToDto()
                    : new EquationDTO(),
                CriterionLibrary = entity.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin != null
                    ? entity.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO()
            };
    }
}
