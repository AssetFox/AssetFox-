using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract
{
    public class BaseBudgetAmountEntity : BaseEntity
    {
        public Guid Id { get; set; }

        public int Year { get; set; }

        public decimal Value { get; set; }
    }
}
