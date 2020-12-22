using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class BudgetPriorityEntity : BaseEntity
    {
        public BudgetPriorityEntity()
        {
            BudgetPercentagePairs = new HashSet<BudgetPercentagePairEntity>();
        }

        public Guid Id { get; set; }

        public Guid BudgetPriorityLibraryId { get; set; }

        public int PriorityLevel { get; set; }

        public int? Year { get; set; }

        public virtual BudgetPriorityLibraryEntity BudgetPriorityLibrary { get; set; }

        public virtual CriterionLibraryBudgetPriorityEntity CriterionLibraryBudgetPriorityJoin { get; set; }

        public virtual ICollection<BudgetPercentagePairEntity> BudgetPercentagePairs { get; set; }
    }
}
