using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using Attribute = AppliedResearchAssociates.iAM.Domains.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class TreatmentConsequenceMapper
    {
        public static TreatmentConsequenceEntity ToEntity(this ConditionalTreatmentConsequence domain, Guid treatmentId, Guid attributeId) =>
            new TreatmentConsequenceEntity
            {
                Id = Guid.NewGuid(),
                TreatmentId = treatmentId,
                AttributeId = attributeId,
                ChangeValue = domain.Change.Expression
            };

        public static void ToSimulationAnalysisDomain(this TreatmentConsequenceEntity entity, SelectableTreatment treatment)
        {
            var consequence = treatment.AddConsequence();
            consequence.Attribute = entity.Attribute.ToDomain().ToSimulationAnalysisAttribute();
            consequence.Change.Expression = entity.ChangeValue;
            consequence.Criterion.Expression = entity.CriterionLibraryTreatmentConsequenceJoin?.CriterionLibrary
                .MergedCriteriaExpression ?? string.Empty;
            consequence.Equation.Expression = entity.TreatmentConsequenceEquationJoin?.Equation.Expression ?? string.Empty;
        }
    }
}
