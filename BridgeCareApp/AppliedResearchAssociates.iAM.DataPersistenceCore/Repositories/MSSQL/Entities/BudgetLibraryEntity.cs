using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class BudgetLibraryEntity : LibraryEntity
    {
        public BudgetLibraryEntity()
        {
            Budgets = new HashSet<BudgetEntity>();
            BudgetLibrarySimulationJoins = new HashSet<BudgetLibrarySimulationEntity>();
        }

        public virtual ICollection<BudgetEntity> Budgets { get; set; }

        public virtual ICollection<BudgetLibrarySimulationEntity> BudgetLibrarySimulationJoins { get; set; }
    }
}
