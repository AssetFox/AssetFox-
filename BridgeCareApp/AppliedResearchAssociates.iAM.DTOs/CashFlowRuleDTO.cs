using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// .
    /// </summary>
    public class CashFlowRuleDTO : BaseDTO
    {
        /// <summary>
        /// .
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public Guid LibraryId { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public bool IsModified { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public List<CashFlowDistributionRuleDTO> CashFlowDistributionRules { get; set; } = new List<CashFlowDistributionRuleDTO>();

        /// <summary>
        /// .
        /// </summary>
        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
