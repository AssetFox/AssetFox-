using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// Describes a specific budget
    /// </summary>
    public class BudgetDTO : BaseDTO
    {
        /// <summary>
        /// Name of the budget
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Funding order for the budgets (higher numbers are used first)
        /// </summary>
        public int BudgetOrder { get; set; }

        /// <summary>
        /// ID of an associated library
        /// </summary>
        public Guid LibraryId { get; set; }

        /// <summary>
        /// Has this setting been modified from the associated library?
        /// </summary>
        public bool IsModified { get; set; }

        /// <summary>
        /// List of available funding for this budget by year
        /// </summary>
        public List<BudgetAmountDTO> BudgetAmounts { get; set; }

        /// <summary>
        /// Defines the assets that can use this budget
        /// </summary>
        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
