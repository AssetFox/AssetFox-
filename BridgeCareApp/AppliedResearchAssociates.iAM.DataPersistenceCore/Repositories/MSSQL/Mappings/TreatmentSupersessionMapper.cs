using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class TreatmentSupersessionMapper
    {
        public static TreatmentSupersessionEntity ToEntity(this TreatmentSupersession domain, Guid treatmentId)
        {
            return new TreatmentSupersessionEntity
            {
                Id = Guid.NewGuid(),
                TreatmentId = treatmentId
            };
        }

        public static void ToSimulationAnalysisDomain(this TreatmentSupersessionEntity entity,
            SelectableTreatment selectableTreatment)
        {
            var supersession = selectableTreatment.AddSupersession();
            supersession.Treatment = selectableTreatment;
            supersession.Criterion.Expression =
                entity.CriterionLibraryTreatmentSupersessionJoin?.CriterionLibrary.MergedCriteriaExpression ??
                string.Empty;
        }
    }
}
