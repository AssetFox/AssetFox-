﻿using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IBudgetPriorityRepository
    {
        List<BudgetPriorityLibraryDTO> GetBudgetPriorityLibraries();

        DateTime GetLibraryModifiedDate(Guid budgetPriorityLibraryId);

        List<BudgetPriorityLibraryDTO> GetBudgetPriortyLibrariesNoChildren();

        void UpsertBudgetPriorityLibrary(BudgetPriorityLibraryDTO dto);
        void UpsertOrDeleteBudgetPriorityLibraryAndPriorities(BudgetPriorityLibraryDTO dto, bool isNewLibrary, Guid ownerIdForNewLibrary);

        void UpsertOrDeleteBudgetPriorities(List<BudgetPriorityDTO> budgetPriorities, Guid libraryId);

        void DeleteBudgetPriorityLibrary(Guid libraryId);

        List<BudgetPriorityDTO> GetScenarioBudgetPriorities(Guid simulationId);
        List<BudgetPriorityDTO> GetBudgetPrioritiesByLibraryId(Guid libraryId);

        void UpsertOrDeleteScenarioBudgetPriorities(List<BudgetPriorityDTO> budgetPriorities, Guid simulationId);

        List<BudgetPriorityLibraryDTO> GetBudgetPriorityLibrariesNoChildrenAccessibleToUser(Guid userId);

        LibraryUserAccessModel GetLibraryAccess(Guid libraryId, Guid userId);

        void UpsertOrDeleteUsers(Guid budgetPriorityLibraryId, IList<LibraryUserDTO> libraryUsers);

        List<LibraryUserDTO> GetLibraryUsers(Guid budgetPriorityLibraryId);
    }
}
