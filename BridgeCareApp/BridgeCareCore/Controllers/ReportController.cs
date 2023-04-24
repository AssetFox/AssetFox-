using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Reporting;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using BridgeCareCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        #region "API functions"

        [HttpPost]
        [Route("GetHTML/{reportName}")]
        [Authorize]
        public async Task<IActionResult> GetHtml(string reportName)
        {
            // NOTE:  This might be useful:  https://weblog.west-wind.com/posts/2013/dec/13/accepting-raw-request-body-content-with-aspnet-web-api

            //SendRealTimeMessage($"Starting to process {reportName}.");
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
                if (Guid.TryParse(parameters, out scenarioId))
                {
                    await Task.Factory.StartNew(() =>
                    {
                        scenarioName = UnitOfWork.SimulationRepo.GetSimulationName(scenarioId);
                    });
                }
                else
                    scenarioId = Guid.NewGuid();

                ReportGenerationWorkitem workItem = new ReportGenerationWorkitem(scenarioId, UserInfo.Name, scenarioName, reportName);
                var analysisHandle = _generalWorkQueueService.CreateAndRun(workItem);
                // Before sending a "queued" message that may overwrite early messages from the run,
                // allow a brief moment for an empty queue to start running the submission.
                await Task.Delay(500);
                if (!analysisHandle.WorkHasStarted)
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastSimulationAnalysisDetail, analysisHandle.MostRecentStatusMessage);
                }

                //await analysisHandle.WorkCompletion;
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

        [HttpDelete]
        [Route("Cancel/{networkId}")]
        [Authorize]
        public async Task<IActionResult> CancelNetworkDeletion(Guid networkId)
        {
            try
            {
                var hasBeenRemovedFromQueue = _generalWorkQueueService.Cancel(networkId);
                await Task.Delay(125);

                if (hasBeenRemovedFromQueue)
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWorkQueueStatusUpdate, new QueuedWorkStatusUpdateModel() { Id = networkId, Status = "Canceled" });
                }
                else
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWorkQueueStatusUpdate, new QueuedWorkStatusUpdateModel() { Id = networkId, Status = "Canceling network deletion..." });

                }
                return Ok();
            }
            catch (Exception e)
            {
                var networkName = UnitOfWork.NetworkRepo.GetNetworkName(networkId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Error canceling network deltion for {networkName}::{e.Message}");
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

        #endregion

        #region "Internal functions"
        private async Task<string> GetParameters()
        {
            // Manually bring in the body JSON as doing so in the parameters (i.e., [FromBody] JObject parameters) will fail when the body does not exist
            var parameters = string.Empty;
            if (Request.ContentLength > 0)
            {
                using var reader = new StreamReader(Request.Body, Encoding.UTF8);
                parameters = await reader.ReadToEndAsync();
            }

            return parameters;
        }

        private async Task<IReport> GenerateReport(string reportName, ReportType expectedReportType, string parameters)
        {
            var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(parameters);
            //generate report
            var reportObject = await _generator.Generate(reportName);

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
                //SendRealTimeMessage($"Running {reportName}.");
                await reportObject.Run(parameters);
                //SendRealTimeMessage($"Completed running {reportName}");
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

        private string createReportIndexRepository(IReport reportObject)
        {
            var functionRetrunValue = "";

            //configure report index dto
            var reportIndexDto = new ReportIndexDTO()
            {
                Id = reportObject.ID,
                SimulationId = reportObject.SimulationID,
                Type = reportObject.ReportTypeName,
                Result = reportObject.Results,
                ExpirationDate = DateTime.Now.AddDays(30),
            };

            ////create report index repository
            var isSuccess = this.UnitOfWork.ReportIndexRepository.Add(reportIndexDto);
            if (isSuccess == true) { functionRetrunValue = reportIndexDto.Id.ToString(); }

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
