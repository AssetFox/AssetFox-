using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusHubController : ControllerBase
    {
        private readonly IHubContext<BridgeCareHub> HubContext;
        public StatusHubController(IHubContext<BridgeCareHub> hub)
        {
            HubContext = hub;
        }
        [HttpGet]
        [Route("GetStatus")]
        public async Task<IActionResult> SendStatus()
        {
            await HubContext
                    .Clients
                    .All
                    .SendAsync("BroadcastMessage", "from controller");
            return Ok(2);
        }
    }
}
