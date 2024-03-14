using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Common.Logging;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Reporting.Services;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSDistressProgressionReport;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport;
using BridgeCareCore.Services;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class PAMSDistressProgressionReport : IReport
    {
        private readonly IHubService _hubService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ReportHelper _reportHelper;
        private readonly OPICalculations _opiCalculations;

        public Guid ID { get; set; }

        public Guid? SimulationID { get; set; }

        public Guid? NetworkID { get; set; }

        public string Results { get; private set; }

        public string Suffix => throw new NotImplementedException();

        public ReportType Type => ReportType.File;

        public string ReportTypeName { get; private set; }

        public List<string> Errors { get; private set; }

        public bool IsComplete { get; private set; }

        public string Status { get; private set; }

        public string Criteria { get; set; }

        public PAMSDistressProgressionReport(IUnitOfWork unitOfWork, string name, ReportIndexDTO results, IHubService hubService)
        {
            //store passed parameter   
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
            ReportTypeName = name;

            _opiCalculations = new OPICalculations(_unitOfWork);

            //create report objects            
            _reportHelper = new ReportHelper(_unitOfWork);

            //check for existing report id
            var reportId = (results?.Id) ?? Guid.NewGuid();

            //set report return default parameters
            ID = reportId;
            Errors = new List<string>();
            Status = "Report definition created.";
            Results = string.Empty;
            IsComplete = false;
        }

        public async Task Run(string parameters, CancellationToken? cancellationToken = null, IWorkQueueLog workQueueLog = null)
        {
            workQueueLog ??= new DoNothingWorkQueueLog();

            //check for the parameters string
            if (string.IsNullOrEmpty(parameters) || string.IsNullOrWhiteSpace(parameters))
            {
                Errors.Add("Parameters string is empty OR there are no parameters defined");
                IndicateError();
                return;
            }

            // Determine the Guid for the simulation and set simulation id
            string simulationId = ReportHelper.GetSimulationId(parameters);
            if (!Guid.TryParse(simulationId, out Guid _simulationId))
            {
                Errors.Add("Simulation ID could not be parsed to a Guid");
                IndicateError();
                return;
            }
            SimulationID = _simulationId;

            string simulationName;
            try
            {
                var simulationObject = _unitOfWork.SimulationRepo.GetSimulation(_simulationId);
                simulationName = simulationObject?.Name;
                NetworkID = simulationObject.NetworkId;
            }
            catch (Exception e)
            {
                IndicateError();
                Errors.Add("Failed to find simulation");
                Errors.Add(e.Message);
                return;
            }
            // Check for simulation existence      
            if (string.IsNullOrEmpty(simulationName) || string.IsNullOrWhiteSpace(simulationName))
            {
                IndicateError();
                Errors.Add($"Failed to find name using simulation ID {_simulationId}.");
                return;
            }

            if (NetworkID == Guid.Empty)
            {
                IndicateError();
                Errors.Add($"Failed to find NetworkID using simulation ID {_simulationId}.");
                return;
            }

            // Generate report 
            string distressProgressionReportPath;
            try
            {
                Criteria = ReportHelper.GetCriteria(parameters);
                distressProgressionReportPath = GenerateDistressProgressionReport((Guid)NetworkID, _simulationId, workQueueLog, cancellationToken);
                if (!string.IsNullOrEmpty(Criteria) && string.IsNullOrEmpty(distressProgressionReportPath))
                {
                    var errorStatus = "No assets found for given criteria";
                    IndicateError(errorStatus);
                    Errors.Add(errorStatus);
                    return;
                }
            }
            catch (Exception e)
            {
                IndicateError();
                Errors.Add("Failed to get generate PAMS distress progression report");
                Errors.Add(e.Message);
                return;
            }
            if (string.IsNullOrEmpty(distressProgressionReportPath) || string.IsNullOrWhiteSpace(distressProgressionReportPath))
            {
                Errors.Add("PAMS Distress progression report path is missing or not set");
                IndicateError();
                return;
            }

            // Report success with location of file
            Results = distressProgressionReportPath;
            IsComplete = true;
            Status = "File generated.";
            return;
        }

        private string GenerateDistressProgressionReport(Guid networkID, Guid simulationId, IWorkQueueLog workQueueLog, CancellationToken? cancellationToken)
        {
            checkCancelled(cancellationToken, simulationId);
            
            var logger = new CallbackLogger((string message) =>
            {
                var dto = new SimulationReportDetailDTO
                {
                    SimulationId = simulationId,
                    Status = message,
                };
                UpdateSimulationAnalysisDetail(dto);
            });
            var reportOutputData = _unitOfWork.SimulationOutputRepo.GetSimulationOutputViaJson(simulationId);
            var reportDetailDto = new SimulationReportDetailDTO { SimulationId = simulationId };

            var simulationYears = new List<int>();
            foreach (var item in reportOutputData.Years)
            {
                simulationYears.Add(item.Year);
            }

            var simulationYearsCount = simulationYears.Count;
            var explorer = _unitOfWork.AttributeRepo.GetExplorer();
            var network = _unitOfWork.NetworkRepo.GetSimulationAnalysisNetwork(networkID, explorer);
            _unitOfWork.SimulationRepo.GetSimulationInNetwork(simulationId, network);
            var simulation = network.Simulations.First();
            _unitOfWork.InvestmentPlanRepo.GetSimulationInvestmentPlan(simulation);
            _unitOfWork.AnalysisMethodRepo.GetSimulationAnalysisMethod(simulation, null);
            
            using var excelPackage = new ExcelPackage(new FileInfo("DistressProgressionReportTestData.xlsx"));
            checkCancelled(cancellationToken, simulationId);

            // Pavement Work Summary TAB
            reportDetailDto.Status = $"Creating" + PAMSConstants.OPICalculationsTab + "TAB";
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            UpdateSimulationAnalysisDetail(reportDetailDto);
            var worksheet = excelPackage.Workbook.Worksheets.Add(PAMSConstants.OPICalculationsTab);
            _opiCalculations.Fill(worksheet, reportOutputData, simulationYears);

            //check and generate folder            
            var folderPathForSimulation = $"Reports\\{simulationId}";
            if (Directory.Exists(folderPathForSimulation) == false)
            {
                _ = Directory.CreateDirectory(folderPathForSimulation);
            }
            checkCancelled(cancellationToken, simulationId);

            //set and return value
            var filePath = Path.Combine(folderPathForSimulation, "SummaryReport.xlsx");
            var bin = excelPackage.GetAsByteArray();
            File.WriteAllBytes(filePath, bin);            
            var functionReturnValue = filePath ?? string.Empty;
            reportDetailDto.Status = $"Report generation completed";
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            UpdateSimulationAnalysisDetail(reportDetailDto);
            return functionReturnValue;            
        }

        private void IndicateError(string status = null)
        {
            Status = status ?? "PAMS distress progression report completed with errors";
            IsComplete = true;
        }

        private void checkCancelled(CancellationToken? cancellationToken, Guid simulationId)
        {
            if (cancellationToken != null && cancellationToken.Value.IsCancellationRequested)
            {
                throw new Exception("Report was cancelled");
            }
            var reportDetailDto = new SimulationReportDetailDTO
            {
                SimulationId = simulationId,
                Status = $""
            };
            UpdateSimulationAnalysisDetail(reportDetailDto);
        }

        private void UpdateSimulationAnalysisDetail(SimulationReportDetailDTO dto) => _unitOfWork.SimulationReportDetailRepo.UpsertSimulationReportDetail(dto);
    }
}
