using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Reporting;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSPBExportReport;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security.Interfaces;
using BridgeCareCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.Models;
using Newtonsoft.Json.Linq;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : BridgeCareCoreBaseController
    {
        private readonly IReportGenerator _generator;
        private readonly ILog _log;
        private readonly IGeneralWorkQueueService _generalWorkQueueService;
        public const string ReportError = "Report Error";

        public ReportController(IReportGenerator generator, IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor, ILog logger, IGeneralWorkQueueService generalWorkQueService) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            _log = logger ?? throw new ArgumentNullException(nameof(logger));
            _generalWorkQueueService = generalWorkQueService ?? throw new ArgumentNullException(nameof(generalWorkQueService));
        }

        public class ReportDetails
        {
            public Guid simulationId { get; set; }
            public string reportName { get; set; }
            public bool isGenerated { get; set; }
        }

        #region "API functions"

        [HttpPost]
        [Route("GetHTML/{reportName}")]
        [Authorize]
        public async Task<IActionResult> GetHtml(string reportName)
        {
            // NOTE:  This might be useful:  https://weblog.west-wind.com/posts/2013/dec/13/accepting-raw-request-body-content-with-aspnet-web-api
                        
            var parameters = await GetParameters();
            var report = await GenerateReport(reportName, ReportType.HTML, parameters);

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

        [HttpGet]
        [Route("GetAllReportNamesInSystem")]
        [Authorize]
        public async Task<IActionResult> GetReportNames()
        {
            try
            {
                var reportList = await Task.Run(() => UnitOfWork.ReportIndexRepository.GetAllReportsInSystem());
                var reportNames = reportList.Select(report => new { report.ReportId, report.ReportName });
                return Ok(reportNames);
            }
            catch (Exception ex)
            {

                // Return a meaningful error message to the client
                var errorMessage = $"An error occurred while retrieving the report names. Error: {ex.Message}";
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, errorMessage);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("GetFile/{reportName}")]
        [Authorize]
        public async Task<IActionResult> GetFile(string reportName)
        {
            try
            {
                var parameters = await GetParameters();
                var scenarioName = "";
                var scenarioId = new Guid();
                var id = parameters.SelectToken("scenarioId")?.ToObject<string>()?.ToString();
                if (Guid.TryParse(id, out scenarioId))
                {
                    await Task.Factory.StartNew(() =>
                    {
                        scenarioName = UnitOfWork.SimulationRepo.GetSimulationName(scenarioId);
                    });
                }
                else
                    scenarioId = Guid.NewGuid();

                ReportGenerationWorkitem workItem = new ReportGenerationWorkitem(parameters, UserInfo.Name, scenarioName, reportName);
                var analysisHandle = _generalWorkQueueService.CreateAndRunInFastQueue(workItem);

                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastFastWorkQueueUpdate, scenarioId.ToString());

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{ReportError}::GetFile - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{ReportError}::GetFile - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("DownloadReport/{simulationId}/{reportName}")]
        [Authorize]
        public async Task<IActionResult> DownloadReport(Guid simulationId, string reportName)
        {
            var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
            if (simulationId == Guid.Empty || reportName == String.Empty)
            {
                var message = new List<string>() { $"No simulation or report name provided." };
                return CreateErrorListing(message);
            }

            if (UnitOfWork.SimulationRepo.GetSimulation(simulationId) == null)
            {
                var message = new List<string>() { $"A simulation with the ID of {simulationId} is not available in the database." };
                return CreateErrorListing(message);
            }

            var report = UnitOfWork.ReportIndexRepository.GetAllForScenario(simulationId)
                .Where(_ => _.Type == reportName)
                .OrderByDescending(_ => _.CreationDate)
                .FirstOrDefault();
            if (report == null)
            {
                var message = new List<string>() { $"No simulations of the specified type ({reportName}) exist for simulation {simulationName}.  Did you run the report?" };
                return CreateErrorListing(message);
            }

            var reportPath = Path.Combine(Environment.CurrentDirectory, report.Result);
            if (string.IsNullOrEmpty(reportPath) || string.IsNullOrWhiteSpace(reportPath))
            {
                var message = new List<string>() { $"The report for {simulationName} did not include any results" };
                return CreateErrorListing(message);
            }

            FileInfoDTO result;
            try
            {
                result = await GetReport(report);
            }
            catch (Exception e)
            {
                return CreateErrorListing(new List<string>() { e.Message });
            }
            return Ok(result);
        }

        [HttpPost]
        [Route("GetReportGenerationStatus")]
        [Authorize]
        public async Task<IActionResult> GetReportGenerationStatus([FromBody] List<ReportDetails> reportDetails)
        {
            foreach (var report in reportDetails)
            {
                if (report.simulationId == Guid.Empty || report.reportName == String.Empty)
                {
                    var message = new List<string>() { $"No simulation or report name provided." };
                    return CreateErrorListing(message);
                }

                var availableReport = UnitOfWork.ReportIndexRepository.GetAllForScenario(report.simulationId)
                    .Where(_ => _.Type == report.reportName)
                    .OrderByDescending(_ => _.CreationDate)
                    .FirstOrDefault();

                if (availableReport == null)
                {
                    report.isGenerated = false;
                }
                else
                {
                    var reportPath = Path.Combine(Environment.CurrentDirectory, "Reports", reportDetails[0].simulationId.ToString());

                    // Check if the directory exists and if the file exists
                    if (Directory.Exists(reportPath) && System.IO.File.Exists(availableReport.Result))
                    {
                        report.isGenerated = true;
                    }
                    else
                    {
                        report.isGenerated = false;
                    }
                }
            }
            return Ok(reportDetails);
        }

        [HttpGet]
        [Route("DeleteReport/{simulationId}/{reportName}")]
        [Authorize]
        public async Task<IActionResult> DeleteReport(Guid simulationId, string reportName)
        {
            var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
            if (simulationId == Guid.Empty || reportName == String.Empty)
            {
                var message = new List<string>() { $"No simulation or report name provided." };
                return CreateErrorListing(message);
            }

            if (UnitOfWork.SimulationRepo.GetSimulation(simulationId) == null)
            {
                var message = new List<string>() { $"A simulation with the ID of {simulationId} is not available in the database." };
                return CreateErrorListing(message);
            }

            var report = UnitOfWork.ReportIndexRepository.GetAllForScenario(simulationId)
                .Where(_ => _.Type == reportName)
                .OrderByDescending(_ => _.CreationDate)
                .FirstOrDefault();
            if (report == null)
            {
                var message = new List<string>() { $"No simulations of the specified type ({reportName}) exist for simulation {simulationName}.  Did you run the report?" };
                return CreateErrorListing(message);
            }

            // Get path
            var reportPath = Path.Combine(Environment.CurrentDirectory, report.Result);
            if (string.IsNullOrEmpty(reportPath) || string.IsNullOrWhiteSpace(reportPath))
            {
                var message = new List<string>() { $"The report for {simulationName} did not include any results" };
                return CreateErrorListing(message);
            }

            // Check if the file path is valid and if the file or directory exists
            if (!System.IO.File.Exists(reportPath))
            {
                var message = new List<string> { $"The report or directory for {simulationName} does not exist." };
                return CreateErrorListing(message);
            }

            try
            {
                System.IO.File.Delete(reportPath);
                return Ok($"The report {reportName} for simulation {simulationName} has been successfully deleted.");
            }
            catch (Exception e)
            {
                var message = new List<string>() { $"The report path {reportPath} does not exist" };
                return CreateErrorListing(new List<string>() { e.Message });
            }

        }

        [HttpGet]
        [Route("DeleteAllGeneratedReports/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> DeleteAllGeneratedReports(Guid simulationId)
        {
            var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
            if (simulationId == Guid.Empty)
            {
                var message = new List<string>() { $"No simulation or report name provided." };
                return CreateErrorListing(message);
            }

            if (UnitOfWork.SimulationRepo.GetSimulation(simulationId) == null)
            {
                var message = new List<string>() { $"A simulation with the ID of {simulationId} is not available in the database." };
                return CreateErrorListing(message);
            }

            // Get path
            var reportPath = Path.Combine(Environment.CurrentDirectory, "Reports", simulationId.ToString());
            if (string.IsNullOrEmpty(reportPath) || string.IsNullOrWhiteSpace(reportPath))
            {
                var message = new List<string>() { $"The report for {simulationName} did not include any results" };
                return CreateErrorListing(message);
            }

            // Throw an error if the path does not exist
            if (!Directory.Exists(reportPath))
            {
                var message = new List<string>() { $"The report path for {simulationName} does not exist." };
                return CreateErrorListing(message);  // Or throw an exception if you prefer
            }

            try
            {
                if (Directory.Exists(reportPath))
                {
                    // Delete the directory and all its contents
                    Directory.Delete(reportPath, true);
                }
                else
                {
                    var message = new List<string>() { $"The report path {reportPath} does not exist" };
                    return CreateErrorListing(message);
                }
            }
            catch (Exception e)
            {
                return CreateErrorListing(new List<string>() { e.Message });
            }

            return Ok($"All reports for {simulationName} have been successfully deleted.");
        }

        #endregion

        #region "Internal functions"
        private async Task<JObject> GetParameters()
        {
            // Manually bring in the body JSON as doing so in the parameters (i.e., [FromBody] JObject parameters) will fail when the body does not exist
            var data = string.Empty;
            var parameters = new List<string>();
            if (Request.ContentLength > 0)
            {
                using var reader = new StreamReader(Request.Body, Encoding.UTF8);
                data = await reader.ReadToEndAsync();
            }            
            var parameterObj = JObject.Parse(data);
            
            return parameterObj;
        }

        private async Task<IReport> GenerateReport(string reportName, ReportType expectedReportType, JObject parameters)
        {            
            var simulationId = parameters.SelectToken("scenarioId")?.ToObject<IEnumerable<object>>().FirstOrDefault()?.ToString();
            var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
            IReport reportObject;
            if (ReportType.HTML == expectedReportType)
            {
                var last3Characters = reportName.Substring(reportName.Length - 3);
                reportName = reportName.Substring(0, reportName.Length - 3);
                reportObject = await _generator.Generate(reportName, last3Characters);
            }
            else
            {
                reportObject = await _generator.Generate(reportName);
            }

           // var criteria = parameters.SelectToken("expression")?.ToObject<IEnumerable<object>>().FirstOrDefault()?.ToString();

            //generate report
            if (reportObject == null)
            {
                // Set the error string before creating the FailureReport output object as the report type will be overwritten
                var errorMessage = $"Failed to generate specified report '{reportName}' for simulation '{simulationName}'";
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
                await reportObject.Run(parameters.ToString());
            }

            //return object
            return reportObject;
        }

        private async Task<FileInfoDTO> GetReport(ReportIndexDTO reportIndex)
        {
            var reportPath = Path.Combine(Environment.CurrentDirectory, reportIndex.Result);
            if (!System.IO.File.Exists(reportPath))
            {
                throw new InvalidOperationException($"Cannot get report for report {reportIndex.Type}");
            }
            var fileData = await Task.Factory.StartNew(() => FetchFromFileLocation(reportPath));
            var simulationName = reportIndex.SimulationId != null ? UnitOfWork.SimulationRepo.GetSimulationName((Guid)reportIndex.SimulationId) : String.Empty;
            var downloadFileName = $"{simulationName} {reportIndex.Type}.xlsx";
            return new FileInfoDTO
            {
                FileData = Convert.ToBase64String(fileData),
                FileName = downloadFileName,
                MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            };
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
            var errorHtml = new StringBuilder("<h2>Report Errors</h2><list>");
            foreach (var item in errors)
            {
                errorHtml.Append($"<li>{item}</li>");
                SendRealTimeMessage(item);
            }
            errorHtml.Append("</list>");

            var returnValue = Content(errorHtml.ToString());
            returnValue.ContentType = "text/html";
            returnValue.StatusCode = (int?)HttpStatusCode.OK;
            return returnValue;
        }

        #endregion
    }
}
