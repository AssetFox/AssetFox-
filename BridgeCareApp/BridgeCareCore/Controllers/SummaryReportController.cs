using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BridgeCareCore.Interfaces.SummaryReport;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SummaryReportController : ControllerBase
    {
        private readonly ILogger<SummaryReportController> _logger;
        private readonly ISummaryReportGenerator _summaryReportGenerator;

        public SummaryReportController(ISummaryReportGenerator summaryReportGenerator,
            ILogger<SummaryReportController> logger)
        {
            _summaryReportGenerator = summaryReportGenerator ?? throw new ArgumentNullException(nameof(summaryReportGenerator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [Route("GenerateSummaryReport/{networkId}/{simulationId}")]
        public FileResult GenerateSummaryReport(Guid networkId, Guid simulationId)
        {
            var response = _summaryReportGenerator.GenerateReport(networkId, simulationId);

            const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            HttpContext.Response.ContentType = contentType;
            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");

            var fileContentResult = new FileContentResult(response, contentType)
            {
                FileDownloadName = "SummaryReportTestData.xlsx"
            };
            return fileContentResult;
        }
    }
}
