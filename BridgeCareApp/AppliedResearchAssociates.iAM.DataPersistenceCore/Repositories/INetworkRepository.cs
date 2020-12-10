using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface INetworkRepository
    {
        void CreateNetwork(DataAssignment.Networking.Network network);

        void CreateNetwork(Network network);

        IEnumerable<DataAssignment.Networking.Network> GetAllNetworks();

        Domains.Network GetSimulationAnalysisNetwork(Guid networkId, Explorer explorer);
    }
}
