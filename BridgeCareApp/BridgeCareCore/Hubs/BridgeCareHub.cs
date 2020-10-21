using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace BridgeCareCore.Hubs
{
    public class BridgeCareHub : Hub
    {
        public async Task SendMessage(string status = "test")
        {
            await Clients.All.SendAsync("BroadcastMessage", status);
        }
    }
}
