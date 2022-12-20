using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public static class INetworkRepositoryExtensions
    {
        /// <summary>Returns the name if possible; otherwise stringifies and returns the id.
        /// All exceptions are caught. From the point of view of external code, this will never throw.</summary> 
        public static string GetNetworkNameOrId(this INetworkRepository repository, Guid networkId)
        {
            try
            {
                var name = repository.GetNetworkName(networkId);
                return name;
            }
            catch
            {
                return $"Network {networkId}; failed to retreive name";
            }
        }
    }
}
