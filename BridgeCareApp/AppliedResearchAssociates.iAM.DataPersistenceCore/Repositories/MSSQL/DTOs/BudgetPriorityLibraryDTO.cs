using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class BudgetPriorityLibraryDTO : BaseLibraryDTO
    {
        public List<BudgetPriorityDTO> BudgetPriorities { get; set; }
    }
}
