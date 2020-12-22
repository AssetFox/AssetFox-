using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class TreatmentLibrarySimulationEntity : BaseEntity
    {
        public Guid TreatmentLibraryId { get; set; }

        public Guid SimulationId { get; set; }

        public virtual SimulationEntity Simulation { get; set; }

        public virtual TreatmentLibraryEntity TreatmentLibrary { get; set; }
    }
}
