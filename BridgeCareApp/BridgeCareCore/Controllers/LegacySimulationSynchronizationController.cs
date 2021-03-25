using System;
using System.Threading.Tasks;
using BridgeCareCore.Hubs;
using BridgeCareCore.Logging;
using BridgeCareCore.Security.Interfaces;
using BridgeCareCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LegacySimulationSynchronizationController : HubControllerBase
    {
        private readonly LegacySimulationSynchronizerService _legacySimulationSynchronizerService;
        private readonly IEsecSecurity _esecSecurity;
        private readonly ILog _logger;


        public LegacySimulationSynchronizationController(
            LegacySimulationSynchronizerService legacySimulationSynchronizerService, IEsecSecurity esecSecurity,
            ILog logger, IHubContext<BridgeCareHub> hubContext) : base(hubContext)
        {
            _legacySimulationSynchronizerService = legacySimulationSynchronizerService ??
                                                   throw new ArgumentNullException(
                                                       nameof(legacySimulationSynchronizerService));
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
            _logger = logger;
        }

        [HttpPost]
        [Route("SynchronizeLegacySimulation/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> SynchronizeLegacySimulation(int simulationId)
        {
            try
            {
                var userInfo = _esecSecurity.GetUserInformation(Request);

                SendRealTimeMessage("BroadcastDataMigration", "Starting data migration...");

                await Task.Factory.StartNew(() =>
                {
                    _legacySimulationSynchronizerService.Synchronize(simulationId, userInfo.Name);

                    SendRealTimeMessage("BroadcastDataMigration", "Finished data migration...");
                });
                
                return Ok();
            }
            catch (Exception e)
            {
                _logger?.Error($"{e.Message}::{e.StackTrace}");
                return StatusCode(500, $"Error => {e.Message}::{e.StackTrace}");
            }
        }
    }
}
