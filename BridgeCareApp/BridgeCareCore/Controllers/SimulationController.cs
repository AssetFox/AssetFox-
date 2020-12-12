using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using BridgeCareCore.Interfaces.Simulation;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimulationController : Controller
    {
        private readonly ISimulationAnalysis _simulationAnalysis;
        private readonly ISimulationRepository _simulationRepo;

        public SimulationController(ISimulationAnalysis simulationAnalysis, ISimulationRepository simulationRepo)
        {
            _simulationAnalysis = simulationAnalysis ?? throw new ArgumentNullException(nameof(simulationAnalysis));
            _simulationRepo = simulationRepo ?? throw new ArgumentNullException(nameof(simulationRepo));
        }

        [HttpGet]
        [Route("GetScenario/{simulationId}")]
        public IActionResult GetSimulation(Guid simulationId) => Ok(_simulationRepo.GetSimulation(simulationId));

        [HttpGet]
        [Route("GetScenarios/{networkId}")]
        public IActionResult GetSimulations(Guid networkId)
        {
            _simulationAnalysis.GetAllSimulations(networkId);
            return Ok();
        }


        [HttpPost]
        [Route("RunSimulation/{networkId}/{simulationId}")]
        public async Task<IActionResult> RunSimulation(Guid networkId, Guid simulationId)
        {
            _simulationAnalysis.CreateAndRun(networkId, simulationId);
            return Ok();
        }

        [HttpPost]
        [Route("CreateScenario/{simulationName}")]
        public async Task<IActionResult> CreateSimulation(string simulationName)
        {
            _simulationAnalysis.CreateSimulation(simulationName);
            return Ok();
        }
    }
}
