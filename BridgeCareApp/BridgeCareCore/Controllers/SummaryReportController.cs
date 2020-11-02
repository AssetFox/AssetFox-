using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.FileSystem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SummaryReportController : ControllerBase
    {
        private readonly ISimulationOutputRepository _simulationOutputFileRepo;
        private readonly ILogger<SummaryReportController> _logger;

        public SummaryReportController(ISimulationOutputRepository simulationOutputFileRepo,
            ILogger<SummaryReportController> logger)
        {
            _simulationOutputFileRepo = simulationOutputFileRepo ?? throw new ArgumentNullException(nameof(simulationOutputFileRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [Route("GenerateSummaryReport")]
        public IActionResult GenerateSummaryReport(Guid networkId, Guid simulationId)
        {
            var result = _simulationOutputFileRepo.GetSimulationResults(networkId, simulationId);
            //BackgroundJob.Enqueue(() => summaryReportGenerator.GenerateExcelReport(model));
            return Ok("Summary report has been generated");
        }
    }
}
