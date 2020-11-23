using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IRemainingLifeLimitRepository
    {
        void CreateRemainingLifeLimitLibrary(string name, string simulationName);
        void CreateRemainingLifeLimits(List<RemainingLifeLimit> remainingLifeLimits, string simulationName);
    }
}
