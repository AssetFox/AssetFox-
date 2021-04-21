using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IRemainingLifeLimitRepository
    {
        void CreateRemainingLifeLimitLibrary(string name, Guid simulationId);

        void CreateRemainingLifeLimits(List<RemainingLifeLimit> remainingLifeLimits, Guid simulationId);

        List<RemainingLifeLimitLibraryDTO> RemainingLifeLimitLibrariesWithRemainingLifeLimits();

        void UpsertRemainingLifeLimitLibrary(RemainingLifeLimitLibraryDTO dto, Guid simulationId);

        void UpsertOrDeleteRemainingLifeLimits(List<RemainingLifeLimitDTO> remainingLifeLimits, Guid libraryId);

        void DeleteRemainingLifeLimitLibrary(Guid libraryId);
    }
}
