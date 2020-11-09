using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class RemainingLifeLimitLibrarySimulationEntity
    {
        public Guid RemainingLifeLimitLibraryId { get; set; }
        public Guid SimulationId { get; set; }

        public virtual RemainingLifeLimitLibraryEntity RemainingLifeLimitLibrary { get; set; }
        public virtual SimulationEntity Simulation { get; set; }
    }
}
