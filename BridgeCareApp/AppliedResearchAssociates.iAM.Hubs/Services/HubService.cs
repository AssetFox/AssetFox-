using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace AppliedResearchAssociates.iAM.Hubs.Services
{
    public class HubService : IHubService
    {
        private readonly IHubContext<BridgeCareHub> _hubContext;

        public HubService(IHubContext<BridgeCareHub> hubContext) => _hubContext = hubContext;

        public void SendRealTimeMessage(string username, string method, object arg)
        {
            if (string.IsNullOrEmpty(username))
            {
                _hubContext?.Clients?.All?.SendAsync(method, arg);
            }
            else
            {
                _hubContext?.Clients?.Group(username)?.SendAsync(method, arg);
            }
        }

        public void SendRealTimeMessage(string username, string method, object arg1, object arg2)
        {
            if (string.IsNullOrEmpty(username))
            {
                _hubContext?.Clients?.All?.SendAsync(method, arg1, arg2);
            }
            else
            {
                _hubContext?.Clients?.Group(username)?.SendAsync(method, arg1, arg2);
            }
        }
    }
}
