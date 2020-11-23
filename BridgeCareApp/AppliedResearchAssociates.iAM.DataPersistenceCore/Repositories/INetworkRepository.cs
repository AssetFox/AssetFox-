using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface INetworkRepository
    {
        void CreateNetwork(DataAssignment.Networking.Network network);

        IEnumerable<DataAssignment.Networking.Network> GetAllNetworks();

        Domains.Network GetSimulationAnalysisNetwork(string networkName);
    }
}
