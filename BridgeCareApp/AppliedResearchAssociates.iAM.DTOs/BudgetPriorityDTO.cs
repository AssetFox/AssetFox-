using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// Describes asset priority levels that are applied prior to the analysis
    /// optimization.  All assets at the highest priority level are treated
    /// prior to any other assets being addressed until the specified percentage
    /// budget has been spent for a specific budget.  Any asset that does not
    /// meet a crteria in this setting is not analyzed.
    /// </summary>
    public class BudgetPriorityDTO : BaseDTO
    {
        /// <summary>
        /// Defines the priority level.  Lower numbers are higher priority
        /// </summary>
        public int PriorityLevel { get; set; }

        /// <summary>
        /// The year for the priority.  If blank, it applies to any year that
        /// is not specificlly identified.
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// ID of an associated library
        /// </summary>
        public Guid libraryId { get; set; }

        /// <summary>
        /// Has this setting been modified from the associated library?
        /// </summary>
        public bool IsModified { get; set; }

        /// <summary>
        /// The list of budgets and the amount that can be spent by budget at 
        /// this priority level
        /// </summary>
        public List<BudgetPercentagePairDTO> BudgetPercentagePairs { get; set; }

        /// <summary>
        /// Defines the assets that can use this priority
        /// </summary>
        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
