using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget
{
    public class BudgetEntity : BaseBudgetEntity
    {
        public BudgetEntity() => BudgetAmounts = new HashSet<BudgetAmountEntity>();

        public Guid BudgetLibraryId { get; set; }

        public virtual BudgetLibraryEntity BudgetLibrary { get; set; }

        public virtual ICollection<BudgetAmountEntity> BudgetAmounts { get; set; }

        public virtual CriterionLibraryBudgetEntity CriterionLibraryBudgetJoin { get; set; }
    }
}
