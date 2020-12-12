using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Interfaces.Simulation;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimulationController : Controller
    {
        private readonly ISimulationAnalysis _simulationAnalysis;

        public SimulationController(ISimulationAnalysis simulationAnalysis)
        {
            _simulationAnalysis = simulationAnalysis;
        }

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
