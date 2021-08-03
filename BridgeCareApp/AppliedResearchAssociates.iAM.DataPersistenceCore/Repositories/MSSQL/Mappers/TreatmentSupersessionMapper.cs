using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class TreatmentSupersessionMapper
    {
        public static TreatmentSupersessionEntity ToEntity(this TreatmentSupersession domain, Guid treatmentId) =>
            new TreatmentSupersessionEntity
            {
                Id = domain.Id,
                TreatmentId = treatmentId
            };

        public static void CreateTreatmentSupersession(this ScenarioTreatmentSupersessionEntity entity,
            SelectableTreatment selectableTreatment)
        {
            var supersession = selectableTreatment.AddSupersession();
            supersession.Treatment = selectableTreatment;
            supersession.Criterion.Expression =
                entity.CriterionLibraryScenarioTreatmentSupersessionJoin?.CriterionLibrary.MergedCriteriaExpression ??
                string.Empty;
        }
    }
}
