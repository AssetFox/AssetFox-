using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace BridgeCareCore.Hubs
{
    public class BridgeCareHub : Hub
    {
        public async Task SendMessage(string status)
        {
            await Clients.All.SendAsync("BroadcastMessage", status);
        }
    }
}
