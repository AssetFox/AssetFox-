using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface INetworkRepository
    {
        void CreateNetwork(DataAssignment.Networking.Network network);

        IEnumerable<DataAssignment.Networking.Network> GetAllNetworks();

        Domains.Network GetSimulationAnalysisNetwork(string networkName, Explorer explorer);
    }
}
