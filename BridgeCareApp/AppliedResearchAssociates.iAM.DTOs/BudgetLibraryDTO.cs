using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class BudgetLibraryDTO : BaseLibraryDTO
    {
        public List<BudgetDTO> Budgets { get; set; }
    }
}
