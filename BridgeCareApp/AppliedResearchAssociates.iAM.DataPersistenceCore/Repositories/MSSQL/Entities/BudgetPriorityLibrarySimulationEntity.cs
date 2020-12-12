using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class BudgetPriorityLibrarySimulationEntity : BaseEntity
    {
        public Guid BudgetPriorityLibraryId { get; set; }

        public Guid SimulationId { get; set; }

        public virtual BudgetPriorityLibraryEntity BudgetPriorityLibrary { get; set; }

        public virtual SimulationEntity Simulation { get; set; }
    }
}
