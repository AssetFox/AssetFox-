using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.BudgetPriority
{
    public class BudgetPriorityLibraryUserEntity : LibraryUserBaseEntity
    {
        public virtual BudgetPriorityLibraryEntity BudgetPriorityLibrary { get; set; }
    }
}
