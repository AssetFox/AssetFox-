using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OfficeOpenXml;

using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

using AppliedResearchAssociates.iAM.ExcelHelpers;

using AppliedResearchAssociates.iAM.Reporting.Interfaces.PAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport;

using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PamsData;
using System.IO;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class PAMSSummaryReport : IReport
    {
        //private readonly IHubService _hubService;
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private readonly IPamsDataForSummaryReport _pamsDataForSummaryReport;

        private Guid _networkId;
        private Guid _simulationId;

        public Guid ID { get; set; }

        public Guid? SimulationID { get; set; }

        public string Results { get; private set; }

        public ReportType Type => ReportType.File;

        public string ReportTypeName { get; private set; }

        public List<string> Errors { get; private set; }

        public bool IsComplete { get; private set; }

        public string Status { get; private set; }

        public PAMSSummaryReport(UnitOfDataPersistenceWork unitOfWork, string name, ReportIndexEntity results)
        {
            //store passed parameter   
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            ReportTypeName = name;

            //generate network id
            _networkId = _unitOfWork.NetworkRepo.GetMainNetwork().Id;

            //create summary report objects
            _pamsDataForSummaryReport = new PamsDataForSummaryReport();
            if (_pamsDataForSummaryReport == null) { throw new ArgumentNullException(nameof(_pamsDataForSummaryReport)); }

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

            // Check for simulation existence            
            var simulationName = _unitOfWork.SimulationRepo.GetSimulationName(_simulationId);
            if (simulationName == null)
            {
                IndicateError();
                Errors.Add($"Failed to find name using simulation ID {_simulationId}.");
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

            var reportOutputData = _unitOfWork.SimulationOutputRepo.GetSimulationOutput(simulationId);
            var reportDetailDto = new SimulationReportDetailDTO { SimulationId = simulationId };

            var simulationYears = new List<int>();
            foreach (var item in reportOutputData.Years) {
                simulationYears.Add(item.Year);
            }

            var simulationYearsCount = simulationYears.Count;

            var explorer = _unitOfWork.AttributeRepo.GetExplorer();
            var network = _unitOfWork.NetworkRepo.GetSimulationAnalysisNetwork(networkId, explorer, false);
            _unitOfWork.SimulationRepo.GetSimulationInNetwork(simulationId, network);

            //var simulation = network.Simulations.First();
            //_unitOfWork.InvestmentPlanRepo.GetSimulationInvestmentPlan(simulation);
            //_unitOfWork.AnalysisMethodRepo.GetSimulationAnalysisMethod(simulation, null); // to do: bimal
            //_unitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulation);
            //_unitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulation);

            //var yearlyBudgetAmount = new Dictionary<string, Budget>();
            //foreach (var budget in simulation.InvestmentPlan.Budgets)
            //{
            //    if (!yearlyBudgetAmount.ContainsKey(budget.Name))
            //    {
            //        yearlyBudgetAmount.Add(budget.Name, budget);
            //    }
            //    else
            //    {
            //        yearlyBudgetAmount[budget.Name] = budget;
            //    }
            //}

            using var excelPackage = new ExcelPackage(new FileInfo("SummaryReportTestData.xlsx"));

            //// Simulation parameters TAB
            //var parametersWorksheet = excelPackage.Workbook.Worksheets.Add("Parameters");
            //reportDetailDto.Status = $"Creating Pams Data TAB";
            //UpdateSimulationAnalysisDetail(reportDetailDto);

            // PAMS Data TAB
            var worksheet = excelPackage.Workbook.Worksheets.Add(SummaryReportTabNames.PamsData);
            _pamsDataForSummaryReport.Fill(worksheet, reportOutputData);

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
                Errors.Add(reportDetailDto.Status);

                byte[] summaryReportData = File.ReadAllBytes(filePath);
                return summaryReportData;
            }

            reportDetailDto.Status = $"Summary report is not available in the path {filePath}";
            UpdateSimulationAnalysisDetail(reportDetailDto);
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
