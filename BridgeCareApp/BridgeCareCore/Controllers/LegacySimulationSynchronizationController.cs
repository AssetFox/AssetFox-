using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security.Interfaces;
using BridgeCareCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LegacySimulationSynchronizationController : BridgeCareCoreBaseController
    {
        private readonly LegacySimulationSynchronizerService _legacySimulationSynchronizerService;

        public LegacySimulationSynchronizationController(LegacySimulationSynchronizerService legacySimulationSynchronizerService,
            IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) =>
            _legacySimulationSynchronizerService = legacySimulationSynchronizerService ??
                                                   throw new ArgumentNullException(nameof(legacySimulationSynchronizerService));

        [HttpPost]
        [Route("SynchronizeLegacySimulation/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> SynchronizeLegacySimulation(int simulationId)
        {
            try
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastDataMigration, "Starting data migration...");

                await Task.Factory.StartNew(() =>
                {
                    _legacySimulationSynchronizerService.Synchronize(simulationId, UserInfo.Name);

                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastDataMigration, "Finished data migration...");
                });

                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Synchronization error::{e.Message}");
                throw;
            }
        }
    }
}
