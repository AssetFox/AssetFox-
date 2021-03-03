using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ICashFlowRuleRepository
    {
        void CreateCashFlowRuleLibrary(string name, Guid simulationId);

        void CreateCashFlowRules(List<CashFlowRule> cashFlowRules, Guid simulationId);

        Task<List<CashFlowRuleLibraryDTO>> CashFlowRuleLibrariesWithCashFlowRules();

        void AddOrUpdateCashFlowRuleLibrary(CashFlowRuleLibraryDTO dto, Guid simulationId);

        void AddOrUpdateOrDeleteCashFlowRules(List<CashFlowRuleDTO> cashFlowRules, Guid libraryId);

        void DeleteCashFlowRuleLibrary(Guid libraryId);
    }
}
