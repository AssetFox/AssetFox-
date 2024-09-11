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
using AppliedResearchAssociates.iAM.Reporting.Services;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class NetworkExportReport : IReport
    {
        protected readonly IHubService _hubService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly NetworkTab _networkTab;
        private Guid _networkId;

        public NetworkExportReport(IUnitOfWork unitOfWork, string name, ReportIndexDTO results, IHubService hubService)
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
            Suffix = string.Empty;
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
            workQueueLog ??= new DoNothingWorkQueueLog();
            if (string.IsNullOrEmpty(parameters) || string.IsNullOrWhiteSpace(parameters))
            {
                Errors.Add("No simulation ID provided in the parameters of PAMS Simulation Report runner");
                IndicateError();
                return;
            }

            var scenarioId = ReportHelper.GetSimulationId(parameters);
            if (!Guid.TryParse(scenarioId, out Guid _scenarioId))
            {
                Errors.Add("Provided simulation ID is not a GUID");
                IndicateError();
                return;
            }

            var simulationName = string.Empty;
            List<MaintainableAsset> maintainableAssets;
            Dictionary<Guid, List<AssetAttributeValuePair>> aggregatedResults;
            try
            {
                var simulationObject = _unitOfWork.SimulationRepo.GetSimulation(_scenarioId);
                simulationName = simulationObject.Name;
                _networkId = simulationObject.NetworkId;
                maintainableAssets = _unitOfWork.MaintainableAssetRepo.GetAllInNetworkWithLocations(_networkId);
                aggregatedResults = _unitOfWork.AggregatedResultRepo.GetAssetAttributeValuePairDictionary(_networkId);
            }
            catch (Exception e)
            {
                IndicateError();
                Errors.Add("Failed to find simulation");
                Errors.Add(e.Message);
                return;
            }

            if (simulationName == null)
            {
                IndicateError();
                Errors.Add($"Failed to find name using simulation ID {_scenarioId}.");
                return;
            }

            if (maintainableAssets == null || aggregatedResults == null)
            {
                IndicateError();
                Errors.Add($"Failed to find MaintainableAssets or AggregatedResults using simulation ID {_scenarioId}.");
                return;
            }

            var reportPath = string.Empty;
            try
            {
                if (cancellationToken != null && cancellationToken.Value.IsCancellationRequested)
                    throw new Exception("Report was cancelled");
                reportPath = GenerateNetworkExportReport(workQueueLog, maintainableAssets, _networkId, aggregatedResults, cancellationToken);
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
            SimulationID = _scenarioId;
            NetworkID = _networkId;
            ReportTypeName = "NetworkExportReport";
            return;
        }

        private string GenerateNetworkExportReport(IWorkQueueLog workQueueLog, List<MaintainableAsset> maintainableAssets, Guid networkId, Dictionary<Guid, List<AssetAttributeValuePair>> aggregatedResults, CancellationToken? cancellationToken = null)
        {
            if (cancellationToken != null && cancellationToken.Value.IsCancellationRequested)
                throw new Exception("Report was cancelled");
            var reportPath = string.Empty;
            var reportDetailDto = new SimulationReportDetailDTO
            {
                SimulationId = Guid.Empty,
                Status = $"Generating...",
                ReportType = ReportTypeName
            };
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            using var excelPackage = new ExcelPackage(new FileInfo("NetworkExportReportData.xlsx"));
            var worksheet = excelPackage.Workbook.Worksheets.Add("Aggregated Results");
            var attributeDefaultValuePairs = _unitOfWork.AttributeRepo.GetAttributeDefaultValuePairs(networkId);
            _networkTab.Fill(worksheet, maintainableAssets, aggregatedResults, attributeDefaultValuePairs);

            if (cancellationToken != null && cancellationToken.Value.IsCancellationRequested)
                throw new Exception("Report was cancelled");
            reportDetailDto.Status = $"Creating Report file";
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            var folderPathForSimulation = $"Reports\\{networkId}";
            Directory.CreateDirectory(folderPathForSimulation);
            reportPath = Path.Combine(folderPathForSimulation, "NetworkExportReport.xlsx");

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
