using System;
using System.Threading.Tasks;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
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
        private readonly IEsecSecurity _esecSecurity;
        private readonly LegacySimulationSynchronizerService _legacySimulationSynchronizerService;

        public LegacySimulationSynchronizationController( IEsecSecurity esecSecurity,
            LegacySimulationSynchronizerService legacySimulationSynchronizerService,
            IHubService hubService) : base(hubService)
        {
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
            _legacySimulationSynchronizerService = legacySimulationSynchronizerService ??
                                                   throw new ArgumentNullException(
                                                       nameof(legacySimulationSynchronizerService));
        }

        [HttpPost]
        [Route("SynchronizeLegacySimulation/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> SynchronizeLegacySimulation(int simulationId)
        {
            try
            {
                var userInfo = _esecSecurity.GetUserInformation(Request);

                _hubService.SendRealTimeMessage(HubConstant.BroadcastDataMigration, "Starting data migration...");

                await Task.Factory.StartNew(() =>
                {
                    _legacySimulationSynchronizerService.Synchronize(simulationId, userInfo.Name);

                    _hubService.SendRealTimeMessage(HubConstant.BroadcastDataMigration, "Finished data migration...");
                });

                return Ok();
            }
            catch (Exception e)
            {
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Synchronization error::{e.Message}");
                throw;
            }
        }
    }
}
