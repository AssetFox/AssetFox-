using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class BudgetDTO : BaseDTO
    {
        public string Name { get; set; }

        public List<BudgetAmountDTO> BudgetAmounts { get; set; }

        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
