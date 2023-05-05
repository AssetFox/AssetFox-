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
using AppliedResearchAssociates.iAM.Reporting.Services;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSPBExport;
using BridgeCareCore.Services;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class PAMSPBExportReport : IReport
    {
        protected readonly IHubService _hubService;
        private readonly IUnitOfWork _unitOfWork;
        private Guid _networkId;
        private readonly ReportHelper _reportHelper;
        private readonly TreatmentTab _treatmentTab;
        private readonly MASTab _masTab;

        public PAMSPBExportReport(IUnitOfWork unitOfWork, string name, ReportIndexDTO results, IHubService hubService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
            ReportTypeName = name;
            _reportHelper = new ReportHelper();
            _treatmentTab = new TreatmentTab();
            _masTab = new MASTab();

            // Check for existing report id
            var reportId = results?.Id; if (reportId == null) { reportId = Guid.NewGuid(); }

            // Set report return default parameters
            ID = (Guid)reportId;
            Errors = new List<string>();
            Warnings = new List<string>();
            Status = "Report definition created.";
            Results = string.Empty;
            IsComplete = false;
        }

        public Guid ID { get; set; }

        public Guid? SimulationID { get; set; }

        public string Results { get; private set; }

        public ReportType Type => ReportType.File;

        public string ReportTypeName { get; private set; }

        public List<string> Errors { get; private set; }

        public List<string> Warnings { get; set; }

        public bool IsComplete { get; private set; }

        public string Status { get; private set; }

        public async Task Run(string parameters, CancellationToken? cancellationToken = null, IWorkQueueLog workQueueLog = null)
        {
            workQueueLog ??= new DoNothingWorkQueueLog();
            // Check for the parameters
            if (string.IsNullOrEmpty(parameters) || string.IsNullOrWhiteSpace(parameters))
            {
                Errors.Add("No simulation ID provided in the parameters of PAMS Simulation Report runner");
                IndicateError();
                return;
            }

            // Set simulation id
            if (!Guid.TryParse(parameters, out Guid _simulationId))
            {
                Errors.Add("Provided simulation ID is not a GUID");
                IndicateError();
                return;
            }
            SimulationID = _simulationId;

            var simulationName = string.Empty;
            try
            {
                var simulationObject = _unitOfWork.SimulationRepo.GetSimulation(_simulationId);
                simulationName = simulationObject.Name;
                _networkId = simulationObject.NetworkId;
            }
            catch (Exception e)
            {
                IndicateError();
                Errors.Add("Failed to find simulation");
                Errors.Add(e.Message);
                return;
            }

            // Check for simulation existence                        
            if (simulationName == null)
            {
                IndicateError();
                Errors.Add($"Failed to find name using simulation ID {_simulationId}.");
                return;
            }

            // Generate report 
            var reportPath = string.Empty;
            try
            {
                reportPath = GeneratePAMSPBExportReport(_networkId, _simulationId, workQueueLog, cancellationToken);
            }
            catch (Exception e)
            {
                IndicateError();
                Errors.Add("Failed to generate PAMS PB Export report");
                Errors.Add(e.Message);
                return;
            }

            if (string.IsNullOrEmpty(reportPath) || string.IsNullOrWhiteSpace(reportPath))
            {
                Errors.Add("PAMS PB Export report path is missing or not set");
                IndicateError();
                return;
            }

            // Report success with location of file
            Results = reportPath;
            IsComplete = true;
            Status = "File generated.";
            return;
        }

        private string GeneratePAMSPBExportReport(Guid networkId, Guid simulationId, IWorkQueueLog workQueueLog, CancellationToken? cancellationToken = null)
        {
            checkCancelled(cancellationToken);
            var reportPath = string.Empty;
            var reportDetailDto = new SimulationReportDetailDTO
            {
                SimulationId = simulationId,
                Status = $"Generating..."
            };
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            UpsertSimulationReportDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto, simulationId);

            var logger = new CallbackLogger(str => UpsertSimulationReportDetailWithStatus(reportDetailDto, str));
            var simulationOutput = _unitOfWork.SimulationOutputRepo.GetSimulationOutputViaJson(simulationId);

            var explorer = _unitOfWork.AttributeRepo.GetExplorer();
            var network = _unitOfWork.NetworkRepo.GetSimulationAnalysisNetwork(networkId, explorer);
            _unitOfWork.SimulationRepo.GetSimulationInNetwork(simulationId, network);
            var simulation = network.Simulations.First();                
            var networkMaintainableAssets = _unitOfWork.MaintainableAssetRepo.GetAllInNetworkWithLocations(_networkId);
            var networkMaintainableAssetIds = networkMaintainableAssets.Select(x => x.Id);
            var attributeDTOs = _unitOfWork.AttributeRepo.GetAttributes();
            var requiredAttributeIds = GetRequiredAttributeIds(attributeDTOs);
            var attributeDatumDTOs = _unitOfWork.AttributeDatumRepo.GetAllInNetwork(networkMaintainableAssetIds, requiredAttributeIds);

            // Report
            using var excelPackage = new ExcelPackage(new FileInfo("PAMSPBExportReportData.xlsx"));

            // MAS Tab
            reportDetailDto.Status = $"Creating PAMS MAS TAB";
            UpsertSimulationReportDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto, simulationId);
            var masWorksheet = excelPackage.Workbook.Worksheets.Add(PAMSPBExportReportConstants.MASTab);
            _masTab.Fill(masWorksheet, simulationOutput, simulation.Network.Id, networkMaintainableAssets, attributeDatumDTOs, attributeDTOs);

            // Teatments Tab
            reportDetailDto.Status = $"Creating PAMS Treatments TAB";
            UpsertSimulationReportDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto, simulationId);
            var treatmentsWorksheet = excelPackage.Workbook.Worksheets.Add(PAMSPBExportReportConstants.TreatmentTab);
            _treatmentTab.Fill(treatmentsWorksheet, simulationOutput, simulationId, simulation.Network.Id, simulation.Treatments, networkMaintainableAssets);

            checkCancelled(cancellationToken);
            // Check and generate folder
            reportDetailDto.Status = $"Creating Report file";
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            UpsertSimulationReportDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto, simulationId);
            var folderPathForSimulation = $"Reports\\{simulationId}";
            Directory.CreateDirectory(folderPathForSimulation);
            reportPath = Path.Combine(folderPathForSimulation, "PAMSPBExportReport.xlsx");
            checkCancelled(cancellationToken);
            var bin = excelPackage.GetAsByteArray();
            File.WriteAllBytes(reportPath, bin);

            reportDetailDto.Status = $"Report generation completed";
            UpsertSimulationReportDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto, simulationId);

            return reportPath;
        }

        private static List<Guid> GetRequiredAttributeIds(List<AttributeDTO> attributeDTOs)
        {
            var requiredAttributes = new List<string>
            {
                "SEGMENT_LENGTH","WIDTH","DISTRICT","CNTY","SR","DIRECTION","INTERSTATE","LANES","SURFACE_NAME","RISKSCORE"
            };
            return attributeDTOs.Where(_ => requiredAttributes.Contains(_.Name)).Select(_ => _.Id).ToList();
        }

        private void UpsertSimulationReportDetail(SimulationReportDetailDTO dto) => _unitOfWork.SimulationReportDetailRepo.UpsertSimulationReportDetail(dto);

        private void UpsertSimulationReportDetailWithStatus(SimulationReportDetailDTO dto, string message)
        {
            dto.Status = message;
            UpsertSimulationReportDetail(dto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, dto.Status, dto.SimulationId);
        }

        private void IndicateError()
        {
            Status = "PAMS PB Export output report completed with errors";
            IsComplete = true;
        }

        private void checkCancelled(CancellationToken? cancellationToken)
        {
            if (cancellationToken != null && cancellationToken.Value.IsCancellationRequested)
            {
                throw new Exception("Report was cancelled");
            }
        }
    }
}
