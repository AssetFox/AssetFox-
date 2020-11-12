using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class PerformanceCurveLibrarySimulationEntity
    {
        public Guid PerformanceCurveLibraryId { get; set; }
        public Guid SimulationId { get; set; }

        public virtual PerformanceCurveLibraryEntity PerformanceCurveLibrary { get; set; }
        public virtual SimulationEntity Simulation { get; set; }
    }
}
