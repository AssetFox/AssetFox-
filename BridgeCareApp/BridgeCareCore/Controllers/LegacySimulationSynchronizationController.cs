using System;
using System.Threading.Tasks;
using BridgeCareCore.Hubs;
using BridgeCareCore.Logging;
using BridgeCareCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LegacySimulationSynchronizationController : ControllerBase
    {
        private readonly IHubContext<BridgeCareHub> _hubContext;
        private readonly LegacySimulationSynchronizerService _legacySimulationSynchronizerService;
        private readonly ILog _logger;

        public LegacySimulationSynchronizationController(IHubContext<BridgeCareHub> hub,
            LegacySimulationSynchronizerService legacySimulationSynchronizerService, ILog logger)
        {
            _hubContext = hub ?? throw new ArgumentNullException(nameof(hub));
            _legacySimulationSynchronizerService = legacySimulationSynchronizerService ?? throw new ArgumentNullException(nameof(legacySimulationSynchronizerService));
            _logger = logger;
        }

        [HttpPost]
        [Route("SynchronizeLegacySimulation/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> SynchronizeLegacySimulation(int simulationId)
        {
            try
            {
                await _hubContext
                    .Clients
                    .All
                    .SendAsync("BroadcastDataMigration", "Starting data migration...");

                await _legacySimulationSynchronizerService.Synchronize(simulationId);

                await _hubContext
                    .Clients
                    .All
                    .SendAsync("BroadcastDataMigration", "Finished data migration...");

                return Ok();
            }
            catch (Exception e)
            {
                _logger.Error($"{e.Message}::{e.StackTrace}");
                return StatusCode(500, $"Error => {e.Message}::{e.StackTrace}");
            }
        }
    }
}
