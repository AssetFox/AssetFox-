using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class TargetConditionGoalLibrarySimulationEntity
    {
        public Guid TargetConditionGoalLibraryId { get; set; }

        public Guid SimulationId { get; set; }

        public virtual SimulationEntity Simulation { get; set; }

        public virtual TargetConditionGoalLibraryEntity TargetConditionGoalLibrary { get; set; }
    }
}
