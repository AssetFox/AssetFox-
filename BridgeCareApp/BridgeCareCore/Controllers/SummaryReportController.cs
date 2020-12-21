using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces.SummaryReport;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SummaryReportController : ControllerBase
    {
        private readonly ILogger<SummaryReportController> _logger;
        private readonly ISummaryReportGenerator _summaryReportGenerator;
        private readonly IHubContext<BridgeCareHub> HubContext;

        public SummaryReportController(ISummaryReportGenerator summaryReportGenerator,
            ILogger<SummaryReportController> logger, IHubContext<BridgeCareHub> hub)
        {
            _summaryReportGenerator = summaryReportGenerator ?? throw new ArgumentNullException(nameof(summaryReportGenerator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            HubContext = hub ?? throw new ArgumentNullException(nameof(hub));
        }

        [HttpPost]
        [Route("GenerateSummaryReport/{networkId}/{simulationId}")]
        public async Task<FileResult> GenerateSummaryReport(Guid networkId, Guid simulationId)
        {
            var broadcastingMessage = "Starting report generation";

            try
            {
                await HubContext
                    .Clients
                    .All
                    .SendAsync("BroadcastSummaryReportGenerationStatus", broadcastingMessage, simulationId);

                var response = await Task.Factory.StartNew(() => _summaryReportGenerator.GenerateReport(simulationId, networkId));
                //var response = _summaryReportGenerator.GenerateReport(simulationId, networkId));

                const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Response.ContentType = contentType;
                HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");

                var fileContentResult = new FileContentResult(response, contentType)
                {
                    FileDownloadName = "SummaryReportTestData.xlsx"
                };

                broadcastingMessage = "Finished generating the summary report.";
                sendRealTimeMessage(broadcastingMessage, simulationId);

                return fileContentResult;
            }
            catch (Exception e)
            {
                broadcastingMessage = "An error has occured while generating the summary report";
                sendRealTimeMessage(broadcastingMessage, simulationId);
                return null;
            }
        }

        private void sendRealTimeMessage(string message, Guid simulationId)
        {
            HubContext
                .Clients
                .All
                .SendAsync("BroadcastSummaryReportGenerationStatus", message, simulationId);
        }
    }
}
