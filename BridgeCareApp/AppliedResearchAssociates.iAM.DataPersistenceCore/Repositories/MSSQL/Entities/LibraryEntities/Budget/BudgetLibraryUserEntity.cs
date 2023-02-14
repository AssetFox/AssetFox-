using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget
{
    public class BudgetLibraryUserEntity : BaseEntity
    {
        public Guid BudgetLibraryId { get; set; }
        public Guid UserId { get; set; }
        public int AccessLevel { get; set; }

        public virtual BudgetLibraryEntity BudgetLibrary { get; set; }
        public virtual UserEntity User { get; set; }
    }
}
