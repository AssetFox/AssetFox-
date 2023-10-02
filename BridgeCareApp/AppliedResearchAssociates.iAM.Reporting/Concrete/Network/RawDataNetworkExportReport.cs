using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Common.Logging;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using OfficeOpenXml;
using System.Data;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.Reporting.Services.NetworkExportReport;
using Newtonsoft.Json.Linq;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class RawDataNetworkExportReport : IReport
    {
        protected readonly IHubService _hubService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly NetworkTab _networkTab;

        public RawDataNetworkExportReport(IUnitOfWork unitOfWork, string name, ReportIndexDTO results, IHubService hubService, string suffix)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
            _networkTab = new NetworkTab();

            // Check for existing report id
            var reportId = results?.Id; if (reportId == null) { reportId = Guid.NewGuid(); }

            // Set report return default parameters
            ID = (Guid)reportId;
            Errors = new List<string>();
            Warnings = new List<string>();
            Status = "Report definition created.";
            Results = string.Empty;
            IsComplete = false;
            Suffix = suffix ?? string.Empty;
            Criteria = string.Empty;
        }

        public Guid ID { get; set; }

        public Guid? SimulationID { get; set; }

        public Guid? NetworkID { get; set; }

        public string Results { get; private set; }

        public ReportType Type => ReportType.File;

        public string ReportTypeName { get; private set; }

        public List<string> Errors { get; private set; }

        public List<string> Warnings { get; set; }

        public bool IsComplete { get; private set; }

        public string Status { get; private set; }

        public string Suffix { get; private set; }

        public string Criteria { get; set; }

        public async Task Run(string parameters, CancellationToken? cancellationToken = null, IWorkQueueLog workQueueLog = null)
        {
            var param = JObject.Parse(parameters);
            var scenarioId = new Guid();
            var simulationId = new Guid();
            var networkId = new Guid();
            var maintainableAssets = new List<MaintainableAsset>();
            var aggregatedResults = new List<AggregatedResultDTO>();
            var id = param.SelectToken("scenarioId")?.ToObject<string>()?.ToString();
            if (Guid.TryParse(id, out scenarioId))
            {
                networkId = _unitOfWork.NetworkRepo.GetRawNetwork().Id;
                simulationId = _unitOfWork.SimulationRepo.GetSimulation(scenarioId).Id;
                maintainableAssets = _unitOfWork.MaintainableAssetRepo.GetAllInNetworkWithLocations(networkId);
                aggregatedResults = _unitOfWork.AggregatedResultRepo.GetAllAggregatedResultsForNetwork(networkId);
            }
            else
            {
                scenarioId = Guid.NewGuid();
                simulationId = Guid.NewGuid();
                networkId = Guid.NewGuid();
            }

            var reportPath = string.Empty;
            try
            {
                if (cancellationToken != null && cancellationToken.Value.IsCancellationRequested)
                    throw new Exception("Report was cancelled");
                reportPath = GenerateNetworkExportReport(workQueueLog, maintainableAssets, networkId, aggregatedResults, cancellationToken);
            }
            catch (Exception e)
            {
                IndicateError();
                Errors.Add("Failed to generate Network Export report");
                Errors.Add(e.Message);
                return;
            }

            if (string.IsNullOrEmpty(reportPath) || string.IsNullOrWhiteSpace(reportPath))
            {
                Errors.Add("Network Export report path is missing or not set");
                IndicateError();
                return;
            }

            // Report success with location of file
            Results = reportPath;
            IsComplete = true;
            Status = "File generated.";
            SimulationID = simulationId;
            NetworkID = networkId;
            ReportTypeName = "RawDataNetworkExportReport"; //_unitOfWork.NetworkRepo.GetNetworkName(networkId) + " Network Report";
            return;
        }

        private string GenerateNetworkExportReport(IWorkQueueLog workQueueLog, List<MaintainableAsset> maintainableAssets, Guid networkId, List<AggregatedResultDTO> aggregatedResults, CancellationToken? cancellationToken = null)
        {
            if (cancellationToken != null && cancellationToken.Value.IsCancellationRequested)
                throw new Exception("Report was cancelled");
            var reportPath = string.Empty;
            var reportDetailDto = new SimulationReportDetailDTO
            {
                SimulationId = Guid.Empty,
                Status = $"Generating..."
            };
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            using var excelPackage = new ExcelPackage(new FileInfo("RawDataNetworkExportReportData.xlsx"));
            var worksheet = excelPackage.Workbook.Worksheets.Add("Aggregated Results");
            _networkTab.Fill(worksheet, maintainableAssets, aggregatedResults);

            if (cancellationToken != null && cancellationToken.Value.IsCancellationRequested)
                throw new Exception("Report was cancelled");
            reportDetailDto.Status = $"Creating Report file";
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            var folderPathForSimulation = $"Reports\\{networkId}";
            Directory.CreateDirectory(folderPathForSimulation);
            reportPath = Path.Combine(folderPathForSimulation, "RawDataNetworkExportReport.xlsx");

            if (cancellationToken != null && cancellationToken.Value.IsCancellationRequested)
                throw new Exception("Report was cancelled");
            var bin = excelPackage.GetAsByteArray();
            File.WriteAllBytes(reportPath, bin);

            reportDetailDto.Status = $"Report generation completed";
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);

            return reportPath;
        }

        private void UpdateSimulationAnalysisDetail(SimulationReportDetailDTO dto) => _unitOfWork.SimulationReportDetailRepo.UpsertSimulationReportDetail(dto);

        private void UpdateSimulationAnalysisDetailWithStatus(SimulationReportDetailDTO dto, string message)
        {
            dto.Status = message;
            UpdateSimulationAnalysisDetail(dto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, dto.Status, dto.SimulationId);
        }

        private void IndicateError()
        {
            Status = "Network Export output report completed with errors";
            IsComplete = true;
        }

        private void checkCancelled(CancellationToken? cancellationToken, Guid simulationId)
        {
            if (cancellationToken != null && cancellationToken.Value.IsCancellationRequested)
            {
                throw new Exception("Report was cancelled");
            }
        }
    }
}
