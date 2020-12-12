using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class DeficientConditionGoalLibrarySimulationEntity : BaseEntity
    {
        public Guid DeficientConditionGoalLibraryId { get; set; }

        public Guid SimulationId { get; set; }

        public virtual DeficientConditionGoalLibraryEntity DeficientConditionGoalLibrary { get; set; }

        public virtual SimulationEntity Simulation { get; set; }
    }
}
