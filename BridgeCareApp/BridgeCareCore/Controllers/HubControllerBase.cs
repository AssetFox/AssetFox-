using BridgeCareCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    public class HubControllerBase : ControllerBase
    {
        public readonly IHubService _hubService;

        public HubControllerBase(IHubService hubService) => _hubService = hubService;
    }
}
