using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace BridgeCareCore.Services
{
    public class HubService : IHubService
    {
        private readonly IHubContext<BridgeCareHub> _hubContext;

        public HubService(IHubContext<BridgeCareHub> hubContext) => _hubContext = hubContext;

        public void SendRealTimeMessage(string method, object arg) =>
            _hubContext?.Clients?.All?.SendAsync(method, arg);

        public void SendRealTimeMessage(string method, object arg1, object arg2) =>
            _hubContext?.Clients?.All?.SendAsync(method, arg1, arg2);
    }
}
