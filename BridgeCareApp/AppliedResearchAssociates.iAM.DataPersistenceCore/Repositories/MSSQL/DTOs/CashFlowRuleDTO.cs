using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class CashFlowRuleDTO : BaseDTO
    {
        public string Name { get; set; }

        public List<CashFlowDistributionRuleDTO> CashFlowDistributionRules { get; set; } = new List<CashFlowDistributionRuleDTO>();

        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
