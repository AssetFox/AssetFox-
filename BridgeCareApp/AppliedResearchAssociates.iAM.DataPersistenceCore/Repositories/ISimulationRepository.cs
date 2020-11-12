using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISimulationRepository
    {
        int CreateSimulations(List<Simulation> simulations, Guid networkId);

        IEnumerable<Simulation> GetAllInNetwork(Guid networkId);
    }
}
