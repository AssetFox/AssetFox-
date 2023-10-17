using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.Analysis;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class TreatmentSupersedeRuleMapper
    {
        public static TreatmentSupersedeRuleEntity ToLibraryEntity(this TreatmentSupersedeRule domain, Guid treatmentId) =>
            new TreatmentSupersedeRuleEntity
            {
                Id = domain.Id,
                TreatmentId = treatmentId
            };

        public static ScenarioTreatmentSupersedeRuleEntity ToScenarioEntity(this TreatmentSupersedeRule domain, Guid treatmentId) =>
            new ScenarioTreatmentSupersedeRuleEntity
            {
                Id = domain.Id,
                TreatmentId = treatmentId
            };

        public static void CreateTreatmentSupersedeRule(this ScenarioTreatmentSupersedeRuleEntity entity,
            SelectableTreatment selectableTreatment)
        {
            var supersedeRule = selectableTreatment.AddSupersedeRule();
            supersedeRule.Treatment = selectableTreatment; // TODO Is this correct?
            supersedeRule.Criterion.Expression =
                entity.CriterionLibraryScenarioTreatmentSupersedeRuleJoin?.CriterionLibrary.MergedCriteriaExpression ??
                string.Empty;
        }
    }
}
