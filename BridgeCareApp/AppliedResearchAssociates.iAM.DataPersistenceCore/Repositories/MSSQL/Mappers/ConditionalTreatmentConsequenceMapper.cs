using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class ConditionalTreatmentConsequenceMapper
    {
        public static ConditionalTreatmentConsequenceEntity ToEntity(this ConditionalTreatmentConsequence domain, Guid treatmentId, Guid attributeId) =>
            new ConditionalTreatmentConsequenceEntity
            {
                Id = domain.Id,
                SelectableTreatmentId = treatmentId,
                AttributeId = attributeId,
                ChangeValue = domain.Change.Expression
            };

        public static ConditionalTreatmentConsequenceEntity ToEntity(this TreatmentConsequenceDTO dto, Guid treatmentId,
            Guid attributeId) =>
            new ConditionalTreatmentConsequenceEntity
            {
                Id = dto.Id,
                SelectableTreatmentId = treatmentId,
                AttributeId = attributeId,
                ChangeValue = dto.ChangeValue
            };

        public static void CreateConditionalTreatmentConsequence(this ConditionalTreatmentConsequenceEntity entity, SelectableTreatment treatment, IEnumerable<Domains.Attribute> attributes)
        {
            var consequence = treatment.AddConsequence();
            consequence.Id = entity.Id;
            consequence.Attribute = entity.Attribute.GetAttributesFromDomain(attributes);
            consequence.Change.Expression = entity.ChangeValue;
            consequence.Criterion.Expression = entity.CriterionLibraryConditionalTreatmentConsequenceJoin?.CriterionLibrary
                .MergedCriteriaExpression ?? string.Empty;
            consequence.Equation.Expression = entity.ConditionalTreatmentConsequenceEquationJoin?.Equation.Expression ?? string.Empty;
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
    }
}
