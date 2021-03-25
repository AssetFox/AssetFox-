using Microsoft.AspNetCore.Mvc;
using BridgeCareCore.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace BridgeCareCore.Controllers
{
    public class HubControllerBase : ControllerBase
    {
        private readonly IHubContext<BridgeCareHub> _hubContext;

        public HubControllerBase(IHubContext<BridgeCareHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public void SendRealTimeMessage(string method, object arg) =>
            _hubContext?.Clients?.All?.SendAsync(method, arg);
    }
}
