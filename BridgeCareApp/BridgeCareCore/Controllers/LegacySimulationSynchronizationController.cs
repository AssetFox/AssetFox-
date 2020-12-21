using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Hubs;
using BridgeCareCore.Logging;
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
        private readonly IHubContext<BridgeCareHub> _hubContext;
        private readonly LegacySimulationSynchronizer _legacySimulationSynchronizer;
        private readonly ILog _logger;

        public LegacySimulationSynchronizationController(IHubContext<BridgeCareHub> hub,
            LegacySimulationSynchronizer legacySimulationSynchronizer, ILog logger)
        {
            _hubContext = hub ?? throw new ArgumentNullException(nameof(hub));
            _legacySimulationSynchronizer = legacySimulationSynchronizer ?? throw new ArgumentNullException(nameof(legacySimulationSynchronizer));
            _logger = logger;
        }
        [HttpPost]
        [Route("SynchronizeLegacyData/{legacySimulationId}")]
        public async Task<IActionResult> SynchronizeLegacyData(int legacySimulationId)
        {
            try
            {
                await _hubContext
                    .Clients
                    .All
                    .SendAsync("BroadcastDataMigration", "Starting data migration...");

                await _legacySimulationSynchronizer.SynchronizeLegacySimulation(legacySimulationId);

                await _hubContext
                    .Clients
                    .All
                    .SendAsync("BroadcastDataMigration", "Finished data migration");

                return Ok();
            }
            catch(Exception e)
            {
                _logger.Error($"{e.Message}::{e.StackTrace}");
                return StatusCode(500, $"Error => {e.Message}::{e.StackTrace}");
            }
        }
    }
}
