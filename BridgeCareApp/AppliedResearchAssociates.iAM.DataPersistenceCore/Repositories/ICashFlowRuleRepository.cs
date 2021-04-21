using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ICashFlowRuleRepository
    {
        void CreateCashFlowRuleLibrary(string name, Guid simulationId);

        void CreateCashFlowRules(List<CashFlowRule> cashFlowRules, Guid simulationId);

        List<CashFlowRuleLibraryDTO> CashFlowRuleLibrariesWithCashFlowRules();

        void UpsertPermitted(Guid simulationId, CashFlowRuleLibraryDTO dto);

        void UpsertCashFlowRuleLibrary(CashFlowRuleLibraryDTO dto, Guid simulationId);

        void UpsertOrDeleteCashFlowRules(List<CashFlowRuleDTO> cashFlowRules, Guid libraryId);

        void DeleteCashFlowRuleLibrary(Guid libraryId);
    }
}
