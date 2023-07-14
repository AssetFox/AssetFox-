using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// Defines a set of cash flow rules for a simulation.  This tells the simulation when a treatment cost
    /// should be spread over multiple years.
    /// </summary>
    public class CashFlowRuleDTO : BaseDTO
    {
        /// <summary>
        /// Name of the cash flow ruleset
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ID of an associated library
        /// </summary>
        public Guid LibraryId { get; set; }

        /// <summary>
        /// Has this setting been modified from the associated library?
        /// </summary>
        public bool IsModified { get; set; }

        /// <summary>
        /// List the rules included in this set
        /// </summary>
        public List<CashFlowDistributionRuleDTO> CashFlowDistributionRules { get; set; } = new List<CashFlowDistributionRuleDTO>();

        /// <summary>
        /// Defines the assets that can use this rule set
        /// </summary>
        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
