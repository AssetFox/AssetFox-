using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract
{
    public class BaseBudgetPriorityEntity : BaseEntity
    {
        public Guid Id { get; set; }

        public int PriorityLevel { get; set; }

        public int? Year { get; set; }

        public Guid LibraryId { get; set; }
    }
}
