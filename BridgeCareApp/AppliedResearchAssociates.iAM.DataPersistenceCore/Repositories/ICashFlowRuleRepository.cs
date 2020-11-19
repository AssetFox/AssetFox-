using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ICashFlowRuleRepository
    {
        void CreateCashFlowRuleLibrary(string name, string simulationName);
        void CreateCashFlowRules(List<CashFlowRule> cashFlowRules, string simulationName);
    }
}
