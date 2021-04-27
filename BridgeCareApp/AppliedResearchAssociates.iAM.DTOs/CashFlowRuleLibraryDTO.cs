using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class CashFlowRuleLibraryDTO : BaseLibraryDTO
    {
        public List<CashFlowRuleDTO> CashFlowRules { get; set; } = new List<CashFlowRuleDTO>();
    }
}
