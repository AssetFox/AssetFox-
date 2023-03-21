using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class BudgetPriorityDTO : BaseDTO
    {
        public int PriorityLevel { get; set; }

        public int? Year { get; set; }

        public Guid libraryId { get; set; }

        public List<BudgetPercentagePairDTO> BudgetPercentagePairs { get; set; }

        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
