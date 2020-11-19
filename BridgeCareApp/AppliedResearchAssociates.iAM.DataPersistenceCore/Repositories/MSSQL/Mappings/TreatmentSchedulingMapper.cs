using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class TreatmentSchedulingMapper
    {
        public static TreatmentSchedulingEntity ToEntity(this TreatmentScheduling domain, Guid treatmentId)
        {
            return new TreatmentSchedulingEntity
            {
                Id = Guid.NewGuid(),
                TreatmentId = treatmentId,
                OffsetToFutureYear = domain.OffsetToFutureYear
            };
        }

        public static void ToSimulationAnalysisDomain(this TreatmentSchedulingEntity entity,
            SelectableTreatment selectableTreatment)
        {
            var scheduling = selectableTreatment.Schedulings.GetAdd(new TreatmentScheduling());
            scheduling.OffsetToFutureYear = entity.OffsetToFutureYear;
            scheduling.Treatment = selectableTreatment;
        }
    }
}
