﻿using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IRemainingLifeLimitRepository
    {
        List<RemainingLifeLimitLibraryDTO> GetAllRemainingLifeLimitLibrariesWithRemainingLifeLimits();

        List<RemainingLifeLimitLibraryDTO> GetAllRemainingLifeLimitLibrariesNoChildren();

        void UpsertRemainingLifeLimitLibrary(RemainingLifeLimitLibraryDTO dto);

        DateTime GetLibraryModifiedDate(Guid remainingLibraryId);

        void UpsertOrDeleteRemainingLifeLimits(List<RemainingLifeLimitDTO> remainingLifeLimits, Guid libraryId);
        void UpsertRemainingLifeLimitLibraryAndLimits(RemainingLifeLimitLibraryDTO library);
        void DeleteRemainingLifeLimitLibrary(Guid libraryId);
        List<RemainingLifeLimitDTO> GetScenarioRemainingLifeLimits(Guid simulationId);
        List<RemainingLifeLimitDTO> GetRemainingLifeLimitsByLibraryId(Guid libraryId);
        void UpsertOrDeleteScenarioRemainingLifeLimits(List<RemainingLifeLimitDTO> scenarioRemainingLifeLimit, Guid simulationId);

        List<RemainingLifeLimitLibraryDTO> GetRemainingLifeLimitLibrariesNoChildrenAccessibleToUser(Guid userId);

        LibraryUserAccessModel GetLibraryAccess(Guid libraryId, Guid userId);

        void UpsertOrDeleteUsers(Guid remainingLifeLimitLibraryId, IList<LibraryUserDTO> libraryUsers);

        List<LibraryUserDTO> GetLibraryUsers(Guid remainingLifeLimitLibraryId);
    }
}
