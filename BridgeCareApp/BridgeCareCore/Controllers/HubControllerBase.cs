using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BridgeCareCore.Controllers
{
    public class HubControllerBase : ControllerBase
    {
        public readonly IHubService _hubService;

        public HubControllerBase(IHubService hubService) => _hubService = hubService;
    }
}
