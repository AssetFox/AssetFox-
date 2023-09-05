using System.Collections.Generic;
using System.Threading.Tasks;

namespace BridgeCareCore.Security.Interfaces
{
    public interface IGraphApiClientService
    {
        Task<List<string>> GetGraphApiUserMemberGroup(string userId);
    }
}
