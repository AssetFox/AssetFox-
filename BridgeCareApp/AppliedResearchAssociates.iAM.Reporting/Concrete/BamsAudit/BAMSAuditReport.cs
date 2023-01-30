using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Services;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class BAMSAuditReport : IReport
    {
        protected readonly IHubService _hubService;
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private Guid _networkId;

        public BAMSAuditReport(UnitOfDataPersistenceWork unitOfWork, string name, ReportIndexDTO results, IHubService hubService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
            ReportTypeName = name;
            Warnings = new List<string>();
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

        public async Task Run(string parameters)
        {
            // Check for the parameters
            if (string.IsNullOrEmpty(parameters) || string.IsNullOrWhiteSpace(parameters))
            {
                Errors.Add("Parameters string is empty OR there are no parameters defined");
                IndicateError();
                return;
            }

            // Set simulation id
            if (!Guid.TryParse(parameters, out Guid _simulationId))
            {
                Errors.Add("Simulation ID could not be parsed to a Guid");
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
                reportPath = GenerateAuditReport(_networkId, _simulationId);
            }
            catch (Exception e)
            {
                IndicateError();
                Errors.Add("Failed to generate Audit report");
                Errors.Add(e.Message);
                return;
            }

            if (string.IsNullOrEmpty(reportPath) || string.IsNullOrWhiteSpace(reportPath))
            {
                Errors.Add("Audit report path is missing or not set");
                IndicateError();
                return;
            }

            // Report success with location of file
            Results = reportPath;
            IsComplete = true;
            Status = "File generated.";
            return;
        }

        private string GenerateAuditReport(Guid networkId, Guid simulationId)
        {
            var reportPath = string.Empty;
            var reportDetailDto = new SimulationReportDetailDTO
            {
                SimulationId = simulationId,
                Status = $"Generating..."
            };
            UpdateSimulationAnalysisDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto, simulationId);
                        
            var logger = new CallbackLogger(str => UpdateSimulationAnalysisDetailWithStatus(reportDetailDto, str));
            var reportOutputData = _unitOfWork.SimulationOutputRepo.GetSimulationOutputViaJson(simulationId);            

            using var excelPackage = new ExcelPackage(new FileInfo("AuditReportData.xlsx"));
            
            // Bridge Data TAB
            reportDetailDto.Status = $"Creating Bridges TAB";
            UpdateSimulationAnalysisDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto, simulationId);            
            // TODO Add worksheet and Fill method for bridges data tab here


            // Fill Decisions TAB
            reportDetailDto.Status = $"Creating Decisions TAB";
            UpdateSimulationAnalysisDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto, simulationId);
            // TODO Add worksheet and Fill method for Decisions tab here
            // TODO take a look at unfunded treatment final list's UnfundedTreatmentCommon.cs line 49, for GCR...yellow columns.
            // simulationOutput.Years to be focused for getting data, Use SortedDictionary to collect data of interest for this tab
            // TODO ask what would be used for "Budget Levels"?? Budgets from SimulationYearDetail? what should be the values -- AvailableFunding??

            // Check and generate folder
            var folderPathForSimulation = $"Reports\\{simulationId}";
            Directory.CreateDirectory(folderPathForSimulation);
            reportPath = Path.Combine(folderPathForSimulation, "AuditReport.xlsx");

            var bin = excelPackage.GetAsByteArray();
            File.WriteAllBytes(reportPath, bin);                       
            
            reportDetailDto.Status = $"Report generation completed";
            UpdateSimulationAnalysisDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto, simulationId);
                        
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
            Status = "Audit output report completed with errors";
            IsComplete = true;
        }
    }
}
