using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Interfaces.Simulation;
using BridgeCareCore.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimulationController : Controller
    {
        private readonly ISimulationAnalysis _simulationAnalysis;
        //private readonly ISimulationRepository _simulationRepo;
        private readonly UnitOfWork _unitOfWork;
        private readonly ILog _logger;

        public SimulationController(ISimulationAnalysis simulationAnalysis, /*ISimulationRepository simulationRepo, */UnitOfWork unitOfWork, ILog logger)
        {
            _simulationAnalysis = simulationAnalysis ?? throw new ArgumentNullException(nameof(simulationAnalysis));
            /*_simulationRepo = simulationRepo ?? throw new ArgumentNullException(nameof(simulationRepo));*/
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger;
        }

        [HttpGet]
        [Route("GetScenario/{simulationId}")]
        public async Task<IActionResult> GetSimulation(Guid simulationId) => Ok(_unitOfWork.SimulationRepo.GetSimulation(simulationId));

        [HttpGet]
        [Route("GetScenarios")]
        public async Task<IActionResult> GetSimulations()
        {
            var simulationDtos = await Task.Factory
                .StartNew(() => _unitOfWork.SimulationRepo.GetAllInNetwork(new Guid(BridgeCareCoreConstants.PennDotNetworkId)));
            return Ok(simulationDtos);
        }


        [HttpPost]
        [Route("RunSimulation/{networkId}/{simulationId}")]
        public async Task<IActionResult> RunSimulation(Guid networkId, Guid simulationId)
        {
            try
            {
                await _simulationAnalysis.CreateAndRun(networkId, simulationId);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.Error($"{e.Message}::{e.StackTrace}");
                return StatusCode(500, e);
            }
        }
    }
}
