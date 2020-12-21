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
        private readonly UnitOfWork _unitOfWork;
        private readonly ILog _logger;

        public SimulationController(ISimulationAnalysis simulationAnalysis, UnitOfWork unitOfWork, ILog logger)
        {
            _simulationAnalysis = simulationAnalysis ?? throw new ArgumentNullException(nameof(simulationAnalysis));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger;
        }

        [HttpGet]
        [Route("GetScenario/{simulationId}")]
        public async Task<IActionResult> GetSimulation(Guid simulationId)
        {
            try
            {
                return Ok(_unitOfWork.SimulationRepo.GetSimulation(simulationId));
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
                    .StartNew(() => _unitOfWork.SimulationRepo.GetAllInNetwork(new Guid(BridgeCareCoreConstants.PennDotNetworkId)));
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
