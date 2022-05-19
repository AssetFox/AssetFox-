using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using AppliedResearchAssociates.iAM.Reporting;
using BridgeCareCore.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.IO;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Http;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : BridgeCareCoreBaseController
    {
        private readonly IReportGenerator _generator;

        public ReportController(IReportGenerator generator, IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) =>
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));            

        #region "API functions"

        [HttpPost]
        [Route("GetHTML/{reportName}")]
        [Authorize]
        public async Task<IActionResult> GetHtml(string reportName)
        {
            // NOTE:  This might be useful:  https://weblog.west-wind.com/posts/2013/dec/13/accepting-raw-request-body-content-with-aspnet-web-api

            //SendRealTimeMessage($"Starting to process {reportName}.");

            var report = await GenerateReport(reportName, ReportType.HTML);

            if (report == null)
            {
                var message = new List<string>() { $"Failed to generate report object for '{reportName}'" };
                return CreateErrorListing(message);
            }

            // Handle a completed run with errors
            if (report.Errors.Any())
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

        [HttpPost]
        [Route("GetFile/{reportName}")]
        [Authorize]
        public async Task<IActionResult> GetFile(string reportName)
        {
            var report = await GenerateReport(reportName, ReportType.File);

            if (report == null)
            {
                var message = new List<string>() { $"Failed to generate report object for '{reportName}'" };
                return CreateErrorListing(message);
            }

            // Handle a completed run with errors
            if (report.Errors.Any())
            {
                return CreateErrorListing(report.Errors);
            }

            // Handle an incomplete run without errors
            if (!report.IsComplete)
            {
                var message = new List<string>() { $"{reportName} ran but never completed" };
                return CreateErrorListing(message);
            }

            //create report index repository
            var reportIndexID = createReportIndexRepository(report);

            if (string.IsNullOrEmpty(reportIndexID) || string.IsNullOrWhiteSpace(reportIndexID)) {
                var message = new List<string>() { $"Failed to create report repository index" };
                return CreateErrorListing(message);
            }

            // Report is good, return the report repository index id
            var validResult = Content(reportIndexID);
            validResult.StatusCode = (int?)HttpStatusCode.OK;
            return validResult;
        }


        [HttpGet]
        [Route("DownloadReport/{reportIndexID}")]
        [Authorize]
        public async Task<IActionResult> DownloadReport(string reportIndexID)
        {
            if (string.IsNullOrEmpty(reportIndexID) || string.IsNullOrWhiteSpace(reportIndexID))
            {
                var message = new List<string>() { $"Repository index identifier is missing or not set" };
                return CreateErrorListing(message);
            }

            //get report path
            var reportIndexEntity = this.UnitOfWork.ReportIndexRepository.Get(Guid.Parse(reportIndexID));
            var reportPath = reportIndexEntity?.Result ?? "";

            if (string.IsNullOrEmpty(reportPath) || string.IsNullOrWhiteSpace(reportPath))
            {
                var message = new List<string>() { $"Failed to get report path using the specified repository index" };
                return CreateErrorListing(message);
            }

            //read file from the specified location and return download response
            var response = await Task.Factory.StartNew(() => FetchFromFileLocation(reportPath));

            //set file download response
            var downloadFileName = $"SummaryReport-\\{reportIndexEntity.SimulationID}\\.xlsx";
            const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            HttpContext.Response.ContentType = contentType;
            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");            
            var fileContentResult = new FileContentResult(response, contentType) { FileDownloadName = downloadFileName };

            // return the download response
            return fileContentResult;
        }

        #endregion


        #region "Internal functions"

        private async Task<IReport> GenerateReport(string reportName, ReportType expectedReportType)
        {
            // Manually bring in the body JSON as doing so in the parameters (i.e., [FromBody] JObject parameters) will fail when the body does not exist
            var parameters = string.Empty;
            if (Request.ContentLength > 0)
            {
                using var reader = new StreamReader(Request.Body, Encoding.UTF8);
                parameters = await reader.ReadToEndAsync();
            }

            //generate report
            var reportObject = await _generator.Generate(reportName);

            if (reportObject == null)
            {
                // Set the error string before creating the FailureReport output object as the report type will be overwritten
                var errorMessage = $"Failed to generate specified report '{reportName}'";
                reportObject = new FailureReport();
                await reportObject.Run(errorMessage);
            }

            // Return an error if the report type does not match the expected type
            if (reportObject.Type != expectedReportType)
            {
                // Set the error string before creating the FailureReport output object as the report type will be overwritten
                var errorMessage = $"A {expectedReportType} type was expected, but {reportName} is a {reportObject.Type} type report.";
                reportObject = new FailureReport();
                await reportObject.Run(errorMessage);
            }

            // Run the report as long as it does not have any existing errors (i.e., failure on generation)
            // Note:  If report was switched to a FailureReport previously, this will not run again
            if (!reportObject.Errors.Any())
            {
                //SendRealTimeMessage($"Running {reportName}.");
                await reportObject.Run(parameters);
                //SendRealTimeMessage($"Completed running {reportName}");
            }

            //return object
            return reportObject;
        }

        private string createReportIndexRepository(IReport reportObject)
        {
            var functionRetrunValue = "";

            //configure report index entity
            var reportIndexEntity = new ReportIndexEntity()
            {
                Id = reportObject.ID,
                SimulationID = reportObject.SimulationID,
                ReportTypeName = reportObject.ReportTypeName,
                Result = reportObject.Results,
                ExpirationDate = DateTime.Now.AddDays(30),
            };

            ////create report index repository
            var isSuccess = this.UnitOfWork.ReportIndexRepository.Add(reportIndexEntity);
            if (isSuccess == true) { functionRetrunValue = reportIndexEntity.Id.ToString(); }

            //return value
            return functionRetrunValue;
        }

        private byte[] FetchFromFileLocation(string filePath)
        {
            if (System.IO.File.Exists(filePath) == false)
            {
                throw new FileNotFoundException($"Summary report is not available in the path {filePath}");
            }

            //read file and return byte array
            byte[] summaryReportData = System.IO.File.ReadAllBytes(filePath);
            return summaryReportData;
        }


        private void SendRealTimeMessage(string message) =>
            HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, message);

        private IActionResult CreateErrorListing(List<string> errors)
        {
            var errorHtml = new StringBuilder("<h2>Error Listing</h2><list>");
            foreach (var item in errors)
            {
                errorHtml.Append($"<li>{item}</li>");
                SendRealTimeMessage(item);
            }
            errorHtml.Append("</list>");

            var returnValue = Content(errorHtml.ToString());
            returnValue.ContentType = "text/html";
            returnValue.StatusCode = (int?)HttpStatusCode.BadRequest;
            return returnValue;
        }

        #endregion
    }
}
