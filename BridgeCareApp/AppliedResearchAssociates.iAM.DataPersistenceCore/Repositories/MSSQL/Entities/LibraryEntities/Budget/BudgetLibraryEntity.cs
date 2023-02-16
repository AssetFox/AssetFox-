using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget
{
    public class BudgetLibraryEntity : LibraryEntity
    {
        public BudgetLibraryEntity()
        {
            Budgets = new HashSet<BudgetEntity>();
            Users = new HashSet<BudgetLibraryUserEntity>();
        }

        public virtual ICollection<BudgetEntity> Budgets { get; set; }

        public virtual ICollection<BudgetLibraryUserEntity> Users { get; set; }
    }
}
