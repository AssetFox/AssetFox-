using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using AppliedResearchAssociates.iAM.Reporting;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Http;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : BridgeCareCoreBaseController
    {
        private readonly IReportGenerator _generator;
        private readonly ILog _log;

        public ReportController(IReportGenerator generator, IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor, ILog logger) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            _log = logger ?? throw new ArgumentNullException(nameof(logger));
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
            var parameters = await GetParameters();
            var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(parameters);
            HubService.SendRealTimeMessage(UnitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, "", parameters);

            try
            {
                return Ok();
            }
            finally
            {
                Response.OnCompleted(async () =>
                {
                    var report = await GenerateReport(reportName, ReportType.File, parameters);

                    if (report == null)
                    {
                        SendRealTimeMessage($"Failed to generate report object for '{reportName}' on simulation '{simulationName}'");
                    }

                    // Handle a completed run with errors
                    if (report.Errors.Any())
                    {
                        SendRealTimeMessage($"Failed to generate '{reportName}' on simulation '{simulationName}'");

                        _log.Information($"Failed to generate '{reportName}'");

                        foreach (string message in report.Errors)
                        {
                            _log.Information($"Message: {message}");
                        }
                    }

                    // Handle an incomplete run without errors
                    if (!report.IsComplete)
                    {
                        SendRealTimeMessage($"{reportName} on simulation '{simulationName}' ran but never completed");
                    }

                    //create report index repository
                    var reportIndexID = createReportIndexRepository(report);

                    if (string.IsNullOrEmpty(reportIndexID) || string.IsNullOrWhiteSpace(reportIndexID))
                    {
                        SendRealTimeMessage($"Failed to create report repository index on {reportName}");
                    }
                });
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
