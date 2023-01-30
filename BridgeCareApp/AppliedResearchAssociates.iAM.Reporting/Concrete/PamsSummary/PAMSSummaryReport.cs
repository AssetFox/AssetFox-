using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Reporting.Interfaces.PAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Logging;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.CountySummary;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.GraphTabs;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PamsData;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.Parameters;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PavementWorkSummary;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PavementWorkSummaryByBudget;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.ShortNameGlossary;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.UnfundedPavementProjects;
using BridgeCareCore.Services;
using OfficeOpenXml;


namespace AppliedResearchAssociates.iAM.Reporting
{
    public class PAMSSummaryReport : IReport
    {
        private readonly IHubService _hubService;
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        private readonly SummaryReportParameters _summaryReportParameters;
        private readonly IPamsDataForSummaryReport _pamsDataForSummaryReport;
        private readonly IPavementWorkSummary _pavementWorkSummary;
        private readonly IPavementWorkSummaryByBudget _pavementWorkSummaryByBudget;
        private readonly UnfundedPavementProjects _unfundedPavementProjects;

        private readonly ICountySummary _countySummary;

        private readonly IAddGraphsInTabs _addGraphsInTabs;
        private readonly SummaryReportGlossary _summaryReportGlossary;

        private Guid _networkId;

        public Guid ID { get; set; }

        public Guid? SimulationID { get; set; }

        public string Results { get; private set; }

        public ReportType Type => ReportType.File;

        public string ReportTypeName { get; private set; }

        public List<string> Errors { get; private set; }

        public bool IsComplete { get; private set; }

        public string Status { get; private set; }

        public PAMSSummaryReport(UnitOfDataPersistenceWork unitOfWork, string name, ReportIndexDTO results, IHubService hubService)
        {
            //store passed parameter   
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
            ReportTypeName = name;

            //create summary report objects
            _pamsDataForSummaryReport = new PamsDataForSummaryReport();
            _summaryReportParameters = new SummaryReportParameters();
            _pavementWorkSummary = new PavementWorkSummary();
            _pavementWorkSummaryByBudget = new PavementWorkSummaryByBudget();
            _unfundedPavementProjects = new UnfundedPavementProjects();
            _countySummary = new CountySummary();
            _addGraphsInTabs = new AddGraphsInTabs();
            _summaryReportGlossary = new SummaryReportGlossary();

            //check for existing report id
            var reportId = results?.Id; if (reportId == null) { reportId = Guid.NewGuid(); }

            //set report return default parameters
            ID = (Guid)reportId;
            Errors = new List<string>();
            Status = "Report definition created.";
            Results = String.Empty;
            IsComplete = false;
        }

        public async Task Run(string parameters)
        {
            //check for the parameters string
            if (string.IsNullOrEmpty(parameters) || string.IsNullOrWhiteSpace(parameters))
            {
                Errors.Add("Parameters string is empty OR there are no parameters defined");
                IndicateError();
                return;
            }

            // Determine the Guid for the simulation and set simulation id
            if (!Guid.TryParse(parameters, out Guid _simulationId))
            {
                Errors.Add("Simulation ID could not be parsed to a Guid");
                IndicateError();
                return;
            }
            SimulationID = _simulationId;

            var simulationName = ""; 
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
            var summaryReportPath = "";
            try
            {
                summaryReportPath = GenerateSummaryReport(_networkId, _simulationId);
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

        private string GenerateSummaryReport(Guid networkId, Guid simulationId)
        {
            var functionReturnValue = "";

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
            foreach (var item in reportOutputData.Years) {
                simulationYears.Add(item.Year);
            }

            var simulationYearsCount = simulationYears.Count;

            var explorer = _unitOfWork.AttributeRepo.GetExplorer();
            var network = _unitOfWork.NetworkRepo.GetSimulationAnalysisNetwork(networkId, explorer, false);
            _unitOfWork.SimulationRepo.GetSimulationInNetwork(simulationId, network);

            var simulation = network.Simulations.First();
            _unitOfWork.InvestmentPlanRepo.GetSimulationInvestmentPlan(simulation);
            _unitOfWork.AnalysisMethodRepo.GetSimulationAnalysisMethod(simulation, null);
            var attributeNameLookup = _unitOfWork.AttributeRepo.GetAttributeNameLookupDictionary();
            _unitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulation, attributeNameLookup);
            _unitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulation); 

            var yearlyBudgetAmount = new Dictionary<string, Budget>();
            foreach (var budget in simulation.InvestmentPlan.Budgets)
            {
                if (!yearlyBudgetAmount.ContainsKey(budget.Name))
                {
                    yearlyBudgetAmount.Add(budget.Name, budget);
                }
                else
                {
                    yearlyBudgetAmount[budget.Name] = budget;
                }
            }

            using var excelPackage = new ExcelPackage(new FileInfo("SummaryReportTestData.xlsx"));

            // Parameters TAB
            reportDetailDto.Status = $"Creating Parameters TAB";
            UpdateSimulationAnalysisDetail(reportDetailDto);
            var parametersWorksheet = excelPackage.Workbook.Worksheets.Add(PAMSConstants.Parameters_Tab);

            // PAMS Data TAB
            reportDetailDto.Status = $"Creating Pams Data TAB";
            UpdateSimulationAnalysisDetail(reportDetailDto);
            var worksheet = excelPackage.Workbook.Worksheets.Add(PAMSConstants.PAMSData_Tab);
            var workSummaryModel = _pamsDataForSummaryReport.Fill(worksheet, reportOutputData);

            //Filling up parameters tab
            _summaryReportParameters.Fill(parametersWorksheet, simulationYearsCount, workSummaryModel.ParametersModel, simulation);


            //// Pavement Work Summary TAB
            reportDetailDto.Status = $"Creating Pavement Work Summary TAB";
            UpdateSimulationAnalysisDetail(reportDetailDto);
            var pamsWorkSummaryWorksheet = excelPackage.Workbook.Worksheets.Add(PAMSConstants.PavementWorkSummary_Tab);
            var chartRowModel = _pavementWorkSummary.Fill(pamsWorkSummaryWorksheet, reportOutputData, simulationYears, workSummaryModel, yearlyBudgetAmount, simulation.Treatments);


            //// Pavement Work Summary By Budget TAB
            reportDetailDto.Status = $"Creating Pavement Work Summary By Budget TAB";
            UpdateSimulationAnalysisDetail(reportDetailDto);
            var pavementWorkSummaryByBudgetWorksheet = excelPackage.Workbook.Worksheets.Add(PAMSConstants.PavementWorkSummaryByBudget_Tab);
            _pavementWorkSummaryByBudget.Fill(pavementWorkSummaryByBudgetWorksheet, reportOutputData, simulationYears, yearlyBudgetAmount, simulation.Treatments);


            // Unfunded Pavement Projects TAB
            reportDetailDto.Status = $"Unfunded Pavement Projects TAB";
            var _unfundedPavementProjectsWorksheet = excelPackage.Workbook.Worksheets.Add(PAMSConstants.UnfundedPavementProjects_Tab);
            _unfundedPavementProjects.Fill(_unfundedPavementProjectsWorksheet, reportOutputData);

            // County Summary TAB
            reportDetailDto.Status = $"County Summary TAB";
            var _countySummaryWorksheet = excelPackage.Workbook.Worksheets.Add(PAMSConstants.CountySummary_Tab);
            _countySummary.Fill(_countySummaryWorksheet, reportOutputData, simulationYears, simulation);

            //Graph TABs
            reportDetailDto.Status = $"Creating Graph TABs";
            UpdateSimulationAnalysisDetail(reportDetailDto);
            _addGraphsInTabs.Add(excelPackage, worksheet, pamsWorkSummaryWorksheet, chartRowModel, simulationYearsCount);

            // Legend TAB
            reportDetailDto.Status = $"Creating Legends TAB";
            var shortNameWorksheet = excelPackage.Workbook.Worksheets.Add(PAMSConstants.Legend_Tab);
            _summaryReportGlossary.Fill(shortNameWorksheet);

            //check and generate folder            
            var folderPathForSimulation = $"Reports\\{simulationId}";
            if (Directory.Exists(folderPathForSimulation) == false) { Directory.CreateDirectory(folderPathForSimulation); }

            var filePath = Path.Combine(folderPathForSimulation, "SummaryReport.xlsx");
            var bin = excelPackage.GetAsByteArray();
            File.WriteAllBytes(filePath, bin);

            //set return value
            functionReturnValue = filePath;

            reportDetailDto.Status = $"Report generation completed";
            UpdateSimulationAnalysisDetail(reportDetailDto);

            //return value
            return functionReturnValue;
        }



        private byte[] FetchFromFileLocation(Guid networkId, Guid simulationId)
        {
            var folderPathForSimulation = $"Reports\\{simulationId}";
            var relativeFolderPath = Path.Combine(Environment.CurrentDirectory, folderPathForSimulation);
            var filePath = Path.Combine(relativeFolderPath, "SummaryReport.xlsx");
            var reportDetailDto = new SimulationReportDetailDTO { SimulationId = simulationId };

            if (File.Exists(filePath))
            {
                reportDetailDto.Status = $"Gathering summary report data";
                UpdateSimulationAnalysisDetail(reportDetailDto);
                _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto, simulationId);
                Errors.Add(reportDetailDto.Status);

                byte[] summaryReportData = File.ReadAllBytes(filePath);
                return summaryReportData;
            }

            reportDetailDto.Status = $"Summary report is not available in the path {filePath}";
            UpdateSimulationAnalysisDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto, simulationId);
            Errors.Add(reportDetailDto.Status);

            throw new FileNotFoundException($"Summary report is not available in the path {filePath}", "SummaryReport.xlsx");
        }

        private void UpdateSimulationAnalysisDetail(SimulationReportDetailDTO dto) => _unitOfWork.SimulationReportDetailRepo.UpsertSimulationReportDetail(dto);

        private void IndicateError()
        {
            Status = "Summary output report completed with errors";
            IsComplete = true;
        }
    }
}
