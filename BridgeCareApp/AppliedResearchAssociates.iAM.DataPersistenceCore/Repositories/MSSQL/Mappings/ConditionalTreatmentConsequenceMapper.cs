using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using Attribute = AppliedResearchAssociates.iAM.Domains.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class ConditionalTreatmentConsequenceMapper
    {
        public static ConditionalTreatmentConsequenceEntity ToEntity(this ConditionalTreatmentConsequence domain, Guid treatmentId, Guid attributeId) =>
            new ConditionalTreatmentConsequenceEntity
            {
                Id = Guid.NewGuid(),
                SelectableTreatmentId = treatmentId,
                AttributeId = attributeId,
                ChangeValue = domain.Change.Expression
            };

        public static void ToSimulationAnalysisDomain(this ConditionalTreatmentConsequenceEntity entity, SelectableTreatment treatment)
        {
            var consequence = treatment.AddConsequence();
            consequence.Attribute = entity.Attribute.ToSimulationAnalysisDomain();
            consequence.Change.Expression = entity.ChangeValue;
            consequence.Criterion.Expression = entity.CriterionLibraryConditionalTreatmentConsequenceJoin?.CriterionLibrary
                .MergedCriteriaExpression ?? string.Empty;
            consequence.Equation.Expression = entity.ConditionalTreatmentConsequenceEquationJoin?.Equation.Expression ?? string.Empty;
        }
    }
}
