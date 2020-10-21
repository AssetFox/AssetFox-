using System.Collections.Generic;
using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface INetworkRepository
    {
        List<NetworkModel> GetAllNetworks(BridgeCareContext db);
    }
}
