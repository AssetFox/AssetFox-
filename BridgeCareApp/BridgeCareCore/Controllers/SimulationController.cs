using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Interfaces.Simulation;
using BridgeCareCore.Logging;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimulationController : Controller
    {
        private readonly ISimulationAnalysis _simulationAnalysis;
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;
        private readonly ILog _logger;

        public SimulationController(ISimulationAnalysis simulationAnalysis, UnitOfDataPersistenceWork unitOfDataPersistenceWork, ILog logger)
        {
            _simulationAnalysis = simulationAnalysis ?? throw new ArgumentNullException(nameof(simulationAnalysis));
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
            _logger = logger;
        }

        [HttpGet]
        [Route("GetScenario/{simulationId}")]
        public async Task<IActionResult> GetSimulation(Guid simulationId)
        {
            try
            {
                return Ok(_unitOfDataPersistenceWork.SimulationRepo.GetSimulation(simulationId));
            }
            catch (Exception e)
            {
                _logger.Error($"{e.Message}::{e.StackTrace}");
                return StatusCode(500, e);
            }
        }

        [HttpGet]
        [Route("GetScenarios")]
        public async Task<IActionResult> GetSimulations()
        {
            try
            {
                var simulationDtos = await Task.Factory
                    .StartNew(() => _unitOfDataPersistenceWork.SimulationRepo.GetAllInNetwork(new Guid(BridgeCareCoreConstants.PennDotNetworkId)));
                return Ok(simulationDtos);
            }
            catch (Exception e)
            {
                _logger.Error($"{e.Message}::{e.StackTrace}");
                return StatusCode(500, e);
            }
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
