using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class CashFlowRuleLibraryDTO : BaseLibraryDTO
    {
        public List<CashFlowRuleDTO> CashFlowRules { get; set; } = new List<CashFlowRuleDTO>();
    }
}
