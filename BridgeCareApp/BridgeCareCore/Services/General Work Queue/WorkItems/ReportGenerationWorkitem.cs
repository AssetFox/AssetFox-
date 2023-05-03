using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.WorkQueue;
using System.Threading;
using System;
using Microsoft.Extensions.DependencyInjection;
using AppliedResearchAssociates.iAM.Reporting;
using System.Reflection.Emit;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.Hubs.Services;
using AppliedResearchAssociates.iAM.Hubs;
using BridgeCareCore.Models;
using System.Linq;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.Reporting.Logging;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using BridgeCareCore.Controllers;

namespace BridgeCareCore.Services
{
    public record ReportGenerationWorkitem(Guid scenarioId, string UserId, string scenarioName,  string reportName) : IWorkSpecification<WorkQueueMetadata>

    {
        public string WorkId => scenarioId.ToString();

        public DateTime StartTime { get; set; }

        public string WorkDescription => $"Generate {reportName} report";

        public string WorkName => scenarioName;

        public WorkQueueMetadata Metadata => new WorkQueueMetadata() {DomainType = DomainType.Simulation, WorkType = WorkType.ReportGeneration};

        public void DoWork(IServiceProvider serviceProvider, Action<string> updateStatusOnHandle, CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();

            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
            var _log = scope.ServiceProvider.GetRequiredService<ILog>();
            var _generator = scope.ServiceProvider.GetRequiredService<IReportGenerator>();
            var _queueLogger = new GeneralWorkQueueLogger(_hubService, UserId, updateStatusOnHandle, scenarioId);
            updateStatusOnHandle.Invoke("Generating...");
            var report = GenerateReport(reportName, ReportType.File, scenarioId.ToString());

            if (report == null)
            {
                SendRealTimeMessage($"Failed to generate report object for '{reportName}' on simulation '{scenarioName}'");
            }

            // Handle a completed run with errors
            if (report.Errors.Any())
            {
                SendRealTimeMessage($"Failed to generate '{reportName}' on simulation '{scenarioName}'");

                _log.Information($"Failed to generate '{reportName}'");

                foreach (string message in report.Errors)
                {
                    _log.Information($"Message: {message}");
                }
            }

            // Handle an incomplete run without errors
            if (!report.IsComplete)
            {
                SendRealTimeMessage($"{reportName} on simulation '{scenarioName}' ran but never completed");
            }

            //create report index repository
            var reportIndexID = createReportIndexRepository(report);

            if (string.IsNullOrEmpty(reportIndexID) || string.IsNullOrWhiteSpace(reportIndexID))
            {
                SendRealTimeMessage($"Failed to create report repository index on {reportName}");
            }

            IReport GenerateReport(string reportName, ReportType expectedReportType, string parameters)
            {
                //generate report
                var reportObject = _generator.Generate(reportName).Result;

                if (reportObject == null)
                {
                    // Set the error string before creating the FailureReport output object as the report type will be overwritten
                    var errorMessage = $"Failed to generate specified report '{reportName}' for simulation '{scenarioName}'";
                    reportObject = new FailureReport();
                    reportObject.Run(errorMessage).Wait();
                }

                // Return an error if the report type does not match the expected type
                if (reportObject.Type != expectedReportType)
                {
                    // Set the error string before creating the FailureReport output object as the report type will be overwritten
                    var errorMessage = $"A {expectedReportType} type was expected, but {reportName} is a {reportObject.Type} type report.";
                    reportObject = new FailureReport();
                    reportObject.Run(errorMessage).Wait();
                }

                // Run the report as long as it does not have any existing errors (i.e., failure on generation)
                // Note:  If report was switched to a FailureReport previously, this will not run again
                if (!reportObject.Errors.Any())
                {
                    //SendRealTimeMessage($"Running {reportName}.");
                    reportObject.Run(parameters, cancellationToken, _queueLogger).Wait();
                    //SendRealTimeMessage($"Completed running {reportName}");
                }

                //return object
                return reportObject;
            }

            string createReportIndexRepository(IReport reportObject)
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
                var isSuccess = _unitOfWork.ReportIndexRepository.Add(reportIndexDto);
                if (isSuccess == true) { functionRetrunValue = reportIndexDto.Id.ToString(); }

                //return value
                return functionRetrunValue;
            }

            void SendRealTimeMessage(string message) =>
                _hubService.SendRealTimeMessage(UserId, HubConstant.BroadcastError, message);
        }

        public void OnFault(IServiceProvider serviceProvider, string errorMessage)
        {
            using var scope = serviceProvider.CreateScope();
            var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();

            _hubService.SendRealTimeMessage(UserId, HubConstant.BroadcastError, $"{ReportController.ReportError}::GetFile - {errorMessage}");
        }
    }
}
