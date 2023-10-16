using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.Analysis;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class TreatmentSupersessionMapper
    {
        public static TreatmentSupersessionEntity ToLibraryEntity(this TreatmentSupersedeRule domain, Guid treatmentId) =>
            new TreatmentSupersessionEntity
            {
                Id = domain.Id,
                TreatmentId = treatmentId
            };
        public static ScenarioTreatmentSupersessionEntity ToScenarioEntity(this TreatmentSupersedeRule domain, Guid treatmentId) =>
            new ScenarioTreatmentSupersessionEntity
            {
                Id = domain.Id,
                TreatmentId = treatmentId
            };

        public static void CreateTreatmentSupersession(this ScenarioTreatmentSupersessionEntity entity,
            SelectableTreatment selectableTreatment)
        {
            var supersession = selectableTreatment.AddSupersedeRule();
            supersession.Treatment = selectableTreatment;
            supersession.Criterion.Expression =
                entity.CriterionLibraryScenarioTreatmentSupersessionJoin?.CriterionLibrary.MergedCriteriaExpression ??
                string.Empty;
        }
    }
}
