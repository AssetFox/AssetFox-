using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class BudgetPriorityDTO : BaseDTO
    {
        public int PriorityLevel { get; set; }
        public int? Year { get; set; }
        public List<BudgetPercentagePairDTO> BudgetPercentagePairs { get; set; }
        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
