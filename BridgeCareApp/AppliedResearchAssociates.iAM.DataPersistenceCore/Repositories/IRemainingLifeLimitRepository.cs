using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IRemainingLifeLimitRepository
    {
        void CreateRemainingLifeLimitLibrary(string name, Guid simulationId);
        void CreateRemainingLifeLimits(List<RemainingLifeLimit> remainingLifeLimits, Guid simulationId);
    }
}
