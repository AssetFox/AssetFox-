using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.Analysis;
using System.Linq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class TreatmentSchedulingMapper
    {
        public static TreatmentSchedulingEntity ToLibraryEntity(this TreatmentScheduling domain, Guid treatmentId) =>
            new TreatmentSchedulingEntity
            {
                Id = domain.Id,
                TreatmentId = treatmentId,
                OffsetToFutureYear = domain.OffsetToFutureYear
            };
        public static ScenarioTreatmentSchedulingEntity ToScenarioEntity(this TreatmentScheduling domain, Guid treatmentId) =>
            new ScenarioTreatmentSchedulingEntity
            {
                Id = domain.Id,
                TreatmentId = treatmentId,
                OffsetToFutureYear = domain.OffsetToFutureYear
            };

        public static void CreateTreatmentScheduling(this ScenarioTreatmentSchedulingEntity entity,
            SelectableTreatment selectableTreatment, Simulation simulation)
        {
            var scheduling = selectableTreatment.Schedulings.GetAdd(new TreatmentScheduling());
            scheduling.OffsetToFutureYear = entity.OffsetToFutureYear;
            scheduling.TreatmentToSchedule = simulation.Treatments.FirstOrDefault(_ => _.Id == selectableTreatment.Id);
        }
    }
}
