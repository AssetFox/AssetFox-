using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Hubs;
using BridgeCareCore.Services.LegacySimulationSynchronization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LegacySimulationSynchronizationController : ControllerBase
    {
        private readonly IHubContext<BridgeCareHub> HubContext;
        private readonly LegacySimulationSynchronizer _legacySimulationSynchronizer;

        public LegacySimulationSynchronizationController(IHubContext<BridgeCareHub> hub,
            LegacySimulationSynchronizer legacySimulationSynchronizer)
        {
            HubContext = hub ?? throw new ArgumentNullException(nameof(hub));
            _legacySimulationSynchronizer = legacySimulationSynchronizer ?? throw new ArgumentNullException(nameof(legacySimulationSynchronizer));
        }
        [HttpPost]
        [Route("SynchronizLegacyData/{legacySimulationId}")]
        public async Task<IActionResult> SynchronizLegacyData(int legacySimulationId)
        {
            // maybe hardcode the legacySimulationId for now?
            var broadcastingMessage = "Started data migration";
            try
            {
                await HubContext
                    .Clients
                    .All
                    .SendAsync("BroadcastDataMigration", broadcastingMessage);
                // for this message, check out connectionHub.ts , over there we need to listen on "BroadcastDataMigration"

                _legacySimulationSynchronizer.SynchronizeLegacySimulation(legacySimulationId);

                broadcastingMessage = "Finished data migration";
                await HubContext
                            .Clients
                            .All
                            .SendAsync("BroadcastDataMigration", broadcastingMessage);

                return Ok("Data migrated successfully");
            }
            catch(Exception e)
            {
                broadcastingMessage = "An error has occured during data migration";
                await HubContext
                            .Clients
                            .All
                            .SendAsync("BroadcastDataMigration", broadcastingMessage);
                return StatusCode(500, e);
            }
        }
    }
}
