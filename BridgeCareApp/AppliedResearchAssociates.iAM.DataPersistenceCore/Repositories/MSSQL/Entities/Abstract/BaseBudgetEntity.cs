using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract
{
    public class BaseBudgetEntity : BaseEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int BudgetOrder { get; set; }
    }
}
