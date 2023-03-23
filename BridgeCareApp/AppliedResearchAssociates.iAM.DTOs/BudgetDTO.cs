using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class BudgetDTO : BaseDTO
    {
        public string Name { get; set; }
        public int BudgetOrder { get; set; }

        public Guid LibraryId { get; set; }

        public List<BudgetAmountDTO> BudgetAmounts { get; set; }

        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
