using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// .
    /// </summary>
    public class BudgetPriorityDTO : BaseDTO
    {
        /// <summary>
        /// .
        /// </summary>
        public int PriorityLevel { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public Guid libraryId { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public bool IsModified { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public List<BudgetPercentagePairDTO> BudgetPercentagePairs { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
