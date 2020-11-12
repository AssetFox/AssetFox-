using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class BudgetPriorityLibrarySimulationEntity
    {
        public Guid BudgetPriorityLibraryId { get; set; }
        public Guid SimulationId { get; set; }

        public virtual BudgetPriorityLibraryEntity BudgetPriorityLibrary { get; set; }
        public virtual SimulationEntity Simulation { get; set; }
    }
}
