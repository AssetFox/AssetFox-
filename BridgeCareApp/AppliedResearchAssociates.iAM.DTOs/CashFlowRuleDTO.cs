using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class CashFlowRuleDTO : BaseDTO
    {
        public string Name { get; set; }
        public Guid LibraryId { get; set; }
        public List<CashFlowDistributionRuleDTO> CashFlowDistributionRules { get; set; } = new List<CashFlowDistributionRuleDTO>();

        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
