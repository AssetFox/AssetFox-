using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IRemainingLifeLimitRepository
    {
        void CreateRemainingLifeLimits(List<RemainingLifeLimit> remainingLifeLimits, Guid simulationId);

        List<RemainingLifeLimitLibraryDTO> RemainingLifeLimitLibrariesWithRemainingLifeLimits();

        void UpsertRemainingLifeLimitLibrary(RemainingLifeLimitLibraryDTO dto);

        void UpsertOrDeleteRemainingLifeLimits(List<RemainingLifeLimitDTO> remainingLifeLimits, Guid libraryId);

        void DeleteRemainingLifeLimitLibrary(Guid libraryId);
        List<RemainingLifeLimitDTO> GetScenarioRemainingLifeLimits(Guid simulationId);
        void UpsertOrDeleteScenarioRemainingLifeLimits(List<RemainingLifeLimitDTO> scenarioRemainingLifeLimit, Guid simulationId);
    }
}
