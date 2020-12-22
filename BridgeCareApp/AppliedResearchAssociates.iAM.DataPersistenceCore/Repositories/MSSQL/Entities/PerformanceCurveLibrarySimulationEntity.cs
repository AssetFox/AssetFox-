using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class PerformanceCurveLibrarySimulationEntity : BaseEntity
    {
        public Guid PerformanceCurveLibraryId { get; set; }

        public Guid SimulationId { get; set; }

        public virtual PerformanceCurveLibraryEntity PerformanceCurveLibrary { get; set; }

        public virtual SimulationEntity Simulation { get; set; }
    }
}
