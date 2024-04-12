using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using AppliedResearchAssociates.iAM.Common.Logging;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Reporting.Services;
using AppliedResearchAssociates.iAM.Reporting.Services.GeneralSummaryReport.GeneralBudgetSummary;
using OfficeOpenXml;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.WorkQueue.Logging;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using BridgeCareCore.Services;


namespace AppliedResearchAssociates.iAM.Reporting.Concrete.GeneralSummary
{
    public class GeneralSummaryReport : IReport
    {
        protected readonly IHubService _hubService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ReportHelper _reportHelper;
        private readonly GeneralBudgetSummary _generalBudgetSummary;
        private readonly GeneralDeficientConditionGoals _generalDeficientConditionGoals;
        private readonly GeneralTargetConditionGoals _generalTargetConditionGoals;
        private readonly GeneralWorkDoneTab _generalWorkDoneTab;

        private Guid _networkId;

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

        public string Criteria { get; set; }
        public string Suffix => throw new NotImplementedException();

        public GeneralSummaryReport(IUnitOfWork unitOfWork, string name, ReportIndexDTO results, IHubService hubService)
        {
            //store passed parameter   
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
            ReportTypeName = name;
            Warnings = new List<string>();

            _generalBudgetSummary = new GeneralBudgetSummary(Warnings, _unitOfWork);
            //if (_generalBudgetSummary == null) { throw new ArgumentNullException(nameof(_generalBudgetSummary))};

            _generalWorkDoneTab = new GeneralWorkDoneTab(_unitOfWork);

            _reportHelper = new ReportHelper(_unitOfWork);

            //check for existing report id
            var reportId = (results?.Id) ?? Guid.NewGuid();

            //set report return default parameters
            ID = (Guid)reportId;
            Errors = new List<string>();
            Warnings ??= new List<string>();
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
            if (string.IsNullOrEmpty(simulationName) || string.IsNullOrWhiteSpace(simulationName))
            {
                IndicateError();
                Errors.Add($"Failed to find name using simulation ID {_simulationId}.");
                return;
            }

            if (_networkId == Guid.Empty)
            {
                IndicateError();
                Errors.Add($"Failed to find networkid using simulation ID {_simulationId}.");
                return;
            }

            // Generate Summary report 
            string summaryReportPath;
            try
            {
                Criteria = ReportHelper.GetCriteria(parameters);
                summaryReportPath = GenerateSummaryReport(_networkId, _simulationId, workQueueLog, cancellationToken);
                if (!string.IsNullOrEmpty(Criteria) && string.IsNullOrEmpty(summaryReportPath))
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
                Errors.Add("Failed to get generate summary report");
                Errors.Add(e.Message);
                return;
            }

            if (string.IsNullOrEmpty(summaryReportPath) || string.IsNullOrWhiteSpace(summaryReportPath))
            {
                Errors.Add("Summary report path is missing or not set");
                IndicateError();
                return;
            }

            // Report success with location of file
            Results = summaryReportPath;
            IsComplete = true;
            Status = "File generated.";
            return;
        }

        private string GenerateSummaryReport(Guid networkId, Guid simulationId, IWorkQueueLog workQueueLog, CancellationToken? cancellationToken = null)
        {
            var reportDetailDto = new SimulationReportDetailDTO { SimulationId = simulationId };
            checkCancelled(cancellationToken, simulationId);
            reportDetailDto.Status = $"Generating...";

            UpdateStatusMessage(workQueueLog, reportDetailDto, simulationId);
            var functionReturnValue = "";
            var reportOutputData = _unitOfWork.SimulationOutputRepo.GetSimulationOutputViaJson(simulationId);

            using var excelPackage = new ExcelPackage(new FileInfo("GeneralSummaryReport.xlsx"));
            var generalWorksheet = excelPackage.Workbook.Worksheets.Add("General Summary");

            CurrentCell currentCell = new CurrentCell { Row = 1, Column = 1 };

            //TO-DO
            // Insert actual scenario name here
            generalWorksheet.Cells[currentCell.Row, currentCell.Column].Value = "Scenario Name General Summary Report";
            currentCell.Row += 2;

            
            UpdateStatusMessage(workQueueLog, reportDetailDto, simulationId);
            var targetBudgets = _unitOfWork.BudgetRepo.GetBudgetYearsBySimulationId(simulationId);
            _generalBudgetSummary.FillTargetBudgets(generalWorksheet, reportOutputData, currentCell);


            //Deficient Condition Goals Table
            UpdateStatusMessage(workQueueLog, reportDetailDto, simulationId);
            var deficientConditoinGoals = _unitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(simulationId);
            GeneralDeficientConditionGoals.Fill(generalWorksheet, reportOutputData, deficientConditoinGoals, currentCell);
            currentCell.Row += 2;


            //Target Condition Goals Table
            UpdateStatusMessage(workQueueLog, reportDetailDto, simulationId);
            var targetConditionGoals = _unitOfWork.TargetConditionGoalRepo.GetScenarioTargetConditionGoals(simulationId);

            GeneralTargetConditionGoals.Fill(generalWorksheet, reportOutputData, targetConditionGoals, currentCell);
            currentCell.Row += 2;

            // Work Done Tab
            var workDoneWorksheet = excelPackage.Workbook.Worksheets.Add("Work Done");
            var simulationOutput = _unitOfWork.SimulationOutputRepo.GetSimulationOutputViaJson(simulationId);

            // Sort data
            simulationOutput.InitialAssetSummaries.Sort(
                    (a, b) => _reportHelper.CheckAndGetValue<double>(a.ValuePerNumericAttribute, "BRKEY_").CompareTo(_reportHelper.CheckAndGetValue<double>(b.ValuePerNumericAttribute, "BRKEY_"))
                    );

            foreach (var yearlySectionData in simulationOutput.Years)
            {
                yearlySectionData.Assets.Sort(
                    (a, b) => _reportHelper.CheckAndGetValue<double>(a.ValuePerNumericAttribute, "BRKEY_").CompareTo(_reportHelper.CheckAndGetValue<double>(b.ValuePerNumericAttribute, "BRKEY_"))
                    );
            }

            var explorer = _unitOfWork.AttributeRepo.GetExplorer();
            var network = _unitOfWork.NetworkRepo.GetSimulationAnalysisNetwork(networkId, explorer);
            _unitOfWork.SimulationRepo.GetSimulationInNetwork(simulationId, network);
            var simulation = network.Simulations.First();
            _unitOfWork.InvestmentPlanRepo.GetSimulationInvestmentPlan(simulation);
            _unitOfWork.AnalysisMethodRepo.GetSimulationAnalysisMethod(simulation, null);
            var attributeNameLookup = _unitOfWork.AttributeRepo.GetAttributeNameLookupDictionary();
            _unitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulation, attributeNameLookup);
            _unitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulation);
            var performanceCurvesAttributes = _reportHelper.GetPerformanceCurvesAttributes(simulation);
            //HashSet<string> performanceCurvesAttributes = new HashSet<string>(attributeNameLookup.Values);
            var primaryKeyField = _unitOfWork.AdminSettingsRepo.GetKeyFields();
            _generalWorkDoneTab.Fill(workDoneWorksheet, reportOutputData, simulation, performanceCurvesAttributes);

            //check and generate folder
            var folderPathForSimulation = $"Reports\\{simulationId}";
            Directory.CreateDirectory(folderPathForSimulation);
            var filePath = Path.Combine(folderPathForSimulation, "GeneralSummaryReport.xlsx");


            checkCancelled(cancellationToken, simulationId);
            var bin = excelPackage.GetAsByteArray();
            File.WriteAllBytes(filePath, bin);

            //set return value
            functionReturnValue = filePath;

            return functionReturnValue;
        }

        private void UpdateStatusMessage(IWorkQueueLog workQueueLog, SimulationReportDetailDTO reportDetailDto, Guid simulationId)
        {
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            UpdateSimulationAnalysisDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto, simulationId);
        }

        private void UpsertSimulationReportDetail(SimulationReportDetailDTO dto) => _unitOfWork.SimulationReportDetailRepo.UpsertSimulationReportDetail(dto);

        private void UpdateSimulationAnalysisDetail(SimulationReportDetailDTO dto) => _unitOfWork.SimulationReportDetailRepo.UpsertSimulationReportDetail(dto);

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
            UpsertSimulationReportDetail(reportDetailDto);
        }
        private void IndicateError(string status = null)
        {
            Status = status ?? "Summary output report completed with errors";
            IsComplete = true;
        }
    }
}
