using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface INetworkRepository
    {
        void CreateNetwork(DataAssignment.Networking.Network network);

        IEnumerable<DataAssignment.Networking.Network> GetAllNetworks();
    }
}
