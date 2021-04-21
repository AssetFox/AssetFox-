using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class BudgetPriorityLibraryDTO : BaseLibraryDTO
    {
        public List<BudgetPriorityDTO> BudgetPriorities { get; set; }
    }
}
