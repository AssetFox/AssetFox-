using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Reporting;
using BridgeCareCore.Hubs;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.IO;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportGenerator _generator;
        private readonly IHubContext<BridgeCareHub> _hubContext;

        public ReportController(IReportGenerator generator, IHubContext<BridgeCareHub> hub)
        {
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            _hubContext = hub ?? throw new ArgumentNullException(nameof(hub));
        }

        [HttpPost]
        [Route("GetHTML/{reportName}")]
        [Authorize]
        public async Task<IActionResult> GetHTML(string reportName)
        {
            // NOTE:  This might be useful:  https://weblog.west-wind.com/posts/2013/dec/13/accepting-raw-request-body-content-with-aspnet-web-api

            SendRealTimeMessage($"Starting to process {reportName}.");

            // Manually bring in the body JSON as doing so in the parameters (i.e., [FromBody] JObject parameters) will fail when the body does not exist
            string parameters = String.Empty;
            if (Request.ContentLength > 0)
            {
                using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    parameters = await reader.ReadToEndAsync();
                }
            }

            var report = await _generator.Generate(reportName);

            // Report back to the user if the request report isn't an HTML report
            if (report.Type != ReportType.HTML)
            {
                var message = new List<string>() { $"{reportName} is not an HTML report" };
                return CreateErrorListing(message);
            }

            // Run the report as long as it does not have any existing errors (i.e., failure on generation)
            if (report.Errors.Count() == 0)
            {
                SendRealTimeMessage($"Running {reportName}.");
                await report.Run(parameters);
                SendRealTimeMessage($"Completed running {reportName}");
            }

            // Handle a completed run with errors
            if (report.Errors.Count() > 0)
            {
                return CreateErrorListing(report.Errors);
            }

            // Handle an incomplete run without errors
            if (!report.IsComplete)
            {
                var message = new List<string>() { $"{reportName} ran but never completed" };
                return CreateErrorListing(message);
            }

            // Report is good, return it
            var validResult = Content(report.Results);
            validResult.ContentType = "text/html";
            validResult.StatusCode = (int?)HttpStatusCode.OK;
            return validResult;
        }

        private void SendRealTimeMessage(string message) =>
            _hubContext
                .Clients
                .All
                .SendAsync("BroadcastReportGenerationStatus", message);

        private IActionResult CreateErrorListing(List<string> errors)
        {
            var errorHTML = new StringBuilder("<h2>Error Listing</h2><list>");
            foreach (var item in errors)
            {
                errorHTML.Append($"<li>{item}</li>");
            }
            errorHTML.Append("</list>");

            var returnValue = Content(errorHTML.ToString());
            returnValue.ContentType = "text/html";
            returnValue.StatusCode = (int?)HttpStatusCode.BadRequest;
            return returnValue;
        }
    }
}
