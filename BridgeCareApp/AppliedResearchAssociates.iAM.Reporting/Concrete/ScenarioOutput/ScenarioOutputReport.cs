using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Common.Logging;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Hubs.Services;
using AppliedResearchAssociates.iAM.Reporting.Services;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class ScenarioOutputReport : IReport
    {
        private IUnitOfWork _unitOfWork;
        private readonly IHubService _hubService;
        private readonly ReportHelper _reportHelper;

        public Guid ID { get; set; }
        public Guid? SimulationID { get; set; }
        public Guid? NetworkID { get; set; }

        public string Results { get; private set; }

        public ReportType Type => ReportType.File;

        public string ReportTypeName { get; private set; }

        public List<string> Errors { get; private set; }

        public bool IsComplete { get; private set; }

        public string Status { get; private set; }

        public string Suffix => throw new NotImplementedException();

        public string Criteria { get; set; }

        public ScenarioOutputReport(IUnitOfWork unitOfWork, string name, ReportIndexDTO results, IHubService hubService)
        {
            _unitOfWork = unitOfWork;
            _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
            ReportTypeName = name;
            ID = Guid.NewGuid();
            Errors = new List<string>();
            Status = "Report definition created.";
            Results = String.Empty;
            IsComplete = false;
            _reportHelper = new ReportHelper(_unitOfWork);
        }

        public async Task Run(string parameters, CancellationToken? cancellationToken = null, IWorkQueueLog workQueueLog = null)
        {
            workQueueLog ??= new DoNothingWorkQueueLog();
            // TODO:  Don't regenerate the report if it has already been generated AND the date on the file was after the LastRun date of the
            // scenario.
            string simulationId = ReportHelper.GetSimulationId(parameters);
            // Determine the Guid for the simulation
            if (!Guid.TryParse(simulationId, out Guid simulationGuid))
            {
                Errors.Add("Simulation ID could not be parsed to a Guid");
                IndicateError();
                return;
            }
            SimulationID = simulationGuid;
            Status = "Generating report";
            workQueueLog.UpdateWorkQueueStatus(Status);

            // Check for simulation existence
            string reportFileName;
            var simulationName = _unitOfWork.SimulationRepo.GetSimulationName(simulationGuid);
            if (simulationName == null)
            {
                IndicateError();
                Errors.Add($"Failed to find simulation ID {SimulationID}.");
                return;
            }

            if (!string.IsNullOrEmpty(simulationName))
            {
                reportFileName = $"Reports\\{simulationName}-{SimulationID}.json";
            }
            else
            {
                reportFileName = $"Reports\\{SimulationID}.json";
            }

           
            // Pull the simulation object
            Status = "Getting simulation output";
            workQueueLog.UpdateWorkQueueStatus(Status);
            Analysis.Engine.SimulationOutput simulationOutput;
            try
            {
                checkCancelled(cancellationToken, simulationGuid);
                simulationOutput = _unitOfWork.SimulationOutputRepo.GetSimulationOutputViaJson(simulationGuid);
            }
            catch (Exception e)
            {
                IndicateError();
                Errors.Add("Failed to pull simulation output.  Has the simulation been run?");
                Errors.Add(e.Message);
                return;
            }

            // Save the output to a file
            Status = "Saving output";
            workQueueLog.UpdateWorkQueueStatus(Status);
            try
            {
                checkCancelled(cancellationToken, simulationGuid);
                using var reportFileWriter = File.CreateText(reportFileName);
                JsonSerializer serializer = new();
                serializer.Serialize(reportFileWriter, simulationOutput);
            }
            catch (Exception e)
            {
                IndicateError();
                Errors.Add("Failed to write file to server");
                Errors.Add(e.Message);
                return;
            }

            // Report success with location of file
            Results = reportFileName;  // This is not set until here to ensure the file was created correctly
            IsComplete = true;
            Status = "File generated.";
            workQueueLog.UpdateWorkQueueStatus(Status);
            return;
        }

        private void IndicateError()
        {
            Status = "Simulation output report completed with errors";
            IsComplete = true;
        }

        private void UpsertSimulationReportDetail(SimulationReportDetailDTO dto) => _unitOfWork.SimulationReportDetailRepo.UpsertSimulationReportDetail(dto);


        private void checkCancelled(CancellationToken? cancellationToken, Guid simulationId)
        {
            if (cancellationToken != null && cancellationToken.Value.IsCancellationRequested)
            {
                throw new Exception("Report was cancelled");
            }
            var reportDetailDto = new SimulationReportDetailDTO
            {
                SimulationId = simulationId,
                Status = $"",
                ReportType = ReportTypeName
            };
            UpsertSimulationReportDetail(reportDetailDto);
        }
    }
}
