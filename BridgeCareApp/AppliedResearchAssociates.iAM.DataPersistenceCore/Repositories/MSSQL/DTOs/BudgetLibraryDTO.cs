using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class BudgetLibraryDTO : BaseLibraryDTO
    {
        public List<BudgetDTO> Budgets { get; set; }
    }
}
