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

        void UpsertPermitted(UserInfoDTO userInfo, Guid simulationId, CashFlowRuleLibraryDTO dto);

        void UpsertCashFlowRuleLibrary(CashFlowRuleLibraryDTO dto, Guid simulationId, UserInfoDTO userInfo);

        void UpsertOrDeleteCashFlowRules(List<CashFlowRuleDTO> cashFlowRules, Guid libraryId, UserInfoDTO userInfo);

        void DeleteCashFlowRuleLibrary(Guid libraryId);
    }
}
