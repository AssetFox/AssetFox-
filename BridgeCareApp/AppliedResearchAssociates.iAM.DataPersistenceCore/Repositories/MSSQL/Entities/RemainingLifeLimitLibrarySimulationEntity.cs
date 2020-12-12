using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class RemainingLifeLimitLibrarySimulationEntity : BaseEntity
    {
        public Guid RemainingLifeLimitLibraryId { get; set; }

        public Guid SimulationId { get; set; }

        public virtual RemainingLifeLimitLibraryEntity RemainingLifeLimitLibrary { get; set; }

        public virtual SimulationEntity Simulation { get; set; }
    }
}
