using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ICashFlowRuleRepository
    {
        void CreateCashFlowRules(List<CashFlowRule> cashFlowRules, Guid simulationId);

        List<CashFlowRuleLibraryDTO> GetCashFlowRuleLibraries();

        List<CashFlowRuleLibraryDTO> GetCashFlowRuleLibrariesNoChildren();

        void UpsertCashFlowRuleLibrary(CashFlowRuleLibraryDTO dto);

        void UpsertOrDeleteCashFlowRules(List<CashFlowRuleDTO> cashFlowRules, Guid libraryId);

        void DeleteCashFlowRuleLibrary(Guid libraryId);

        List<CashFlowRuleDTO> GetScenarioCashFlowRules(Guid simulationId);

        List<CashFlowRuleDTO> GetCashFlowRulesByLibraryId(Guid libraryId);

        void UpsertOrDeleteScenarioCashFlowRules(List<CashFlowRuleDTO> cashFlowRules, Guid simulationId);

        List<CashFlowRuleLibraryDTO> GetCashFlowRuleLibrariesNoChildrenAccessibleToUser(Guid userId);
        LibraryUserAccessModel GetLibraryAccess(Guid libraryId, Guid userId);

        void UpsertOrDeleteUsers(Guid cashFlowRuleLibraryId, IList<LibraryUserDTO> libraryUsers);
        List<LibraryUserDTO> GetLibraryUsers(Guid cashFlowRuleLibraryId);

        void AddLibraryIdToScenarioCashFlowRule(List<CashFlowRuleDTO> cashFlowRuleDTOs, Guid? libraryId);
    }
}
