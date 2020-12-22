using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class BudgetLibrarySimulationEntity : BaseEntity
    {
        public Guid BudgetLibraryId { get; set; }

        public Guid SimulationId { get; set; }

        public virtual BudgetLibraryEntity BudgetLibrary { get; set; }

        public virtual SimulationEntity Simulation { get; set; }
    }
}
