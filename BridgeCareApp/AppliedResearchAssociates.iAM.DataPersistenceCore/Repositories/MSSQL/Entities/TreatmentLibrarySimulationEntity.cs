using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class TreatmentLibrarySimulationEntity
    {
        public Guid TreatmentLibraryId { get; set; }

        public Guid SimulationId { get; set; }

        public virtual SimulationEntity Simulation { get; set; }

        public virtual TreatmentLibraryEntity TreatmentLibrary { get; set; }
    }
}
