using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Common.Logging;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Reporting.Services;
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
        private readonly IUnitOfWork _unitOfWork;

        private readonly SummaryReportParameters _summaryReportParameters;
        private readonly PamsDataForSummaryReport _pamsDataForSummaryReport;
        private readonly PavementWorkSummary _pavementWorkSummary;
        private readonly PavementWorkSummaryByBudget _pavementWorkSummaryByBudget;
        private readonly UnfundedPavementProjects _unfundedPavementProjects;
        private readonly CountySummary _countySummary;
        private readonly AddGraphsInTabs _addGraphsInTabs;
        private readonly SummaryReportGlossary _summaryReportGlossary;
        private readonly ReportHelper _reportHelper;

        private Guid _networkId;

        public Guid ID { get; set; }

        public Guid? SimulationID { get; set; }

        public Guid? NetworkID { get; set; }

        public string Results { get; private set; }

        public ReportType Type => ReportType.File;

        public string ReportTypeName { get; private set; }

        public List<string> Errors { get; private set; }

        public bool IsComplete { get; private set; }

        public string Status { get; private set; }

        public string Criteria { get; set; }

        public string Suffix => throw new NotImplementedException();

        public PAMSSummaryReport(IUnitOfWork unitOfWork, string name, ReportIndexDTO results, IHubService hubService)
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
            _reportHelper = new ReportHelper(_unitOfWork);

            //check for existing report id
            var reportId = results?.Id; if (reportId == null) { reportId = Guid.NewGuid(); }

            //set report return default parameters
            ID = (Guid)reportId;
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
            checkCancelled(cancellationToken, simulationId);
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

            // reportOutputData will have all assets data, filter it based on criteria expression
            if (!string.IsNullOrEmpty(Criteria))
            {
                var criteriaValidationResult = _reportHelper.FilterReportOutputData(reportOutputData, networkId, Criteria);

                if (!reportOutputData.InitialAssetSummaries.Any())
                {
                    reportDetailDto.Status = "Failed to generate report due to no assets found for given criteria";
                    workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
                    UpdateSimulationAnalysisDetail(reportDetailDto);

                    return string.Empty;
                }
            }
                        
            var simulationYears = new List<int>();
            foreach (var item in reportOutputData.Years) {
                simulationYears.Add(item.Year);
            }

            var simulationYearsCount = simulationYears.Count;
            var explorer = _unitOfWork.AttributeRepo.GetExplorer();
            var network = _unitOfWork.NetworkRepo.GetSimulationAnalysisNetwork(networkId, explorer);
            _unitOfWork.SimulationRepo.GetSimulationInNetwork(simulationId, network);

            var simulation = network.Simulations.First();
            _unitOfWork.InvestmentPlanRepo.GetSimulationInvestmentPlan(simulation);
            _unitOfWork.AnalysisMethodRepo.GetSimulationAnalysisMethod(simulation, null);
            var attributeNameLookup = _unitOfWork.AttributeRepo.GetAttributeNameLookupDictionary();
            _unitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulation, attributeNameLookup);
            _unitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulation);
            _unitOfWork.CommittedProjectRepo.GetSimulationCommittedProjects(simulation);

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

            checkCancelled(cancellationToken, simulationId);
            // Parameters TAB
            reportDetailDto.Status = $"Creating Parameters TAB";
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            UpdateSimulationAnalysisDetail(reportDetailDto);
            var parametersWorksheet = excelPackage.Workbook.Worksheets.Add(PAMSConstants.Parameters_Tab);

            checkCancelled(cancellationToken, simulationId);
            // PAMS Data TAB
            reportDetailDto.Status = $"Creating Pams Data TAB";
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            UpdateSimulationAnalysisDetail(reportDetailDto);
            var worksheet = excelPackage.Workbook.Worksheets.Add(PAMSConstants.PAMSData_Tab);
            var workSummaryModel = _pamsDataForSummaryReport.Fill(worksheet, reportOutputData);

            checkCancelled(cancellationToken, simulationId);
            //Filling up parameters tab
            _summaryReportParameters.Fill(parametersWorksheet, simulationYearsCount, workSummaryModel.ParametersModel, simulation);

            checkCancelled(cancellationToken, simulationId);
            //// Pavement Work Summary TAB
            reportDetailDto.Status = $"Creating Pavement Work Summary TAB";
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            UpdateSimulationAnalysisDetail(reportDetailDto);
            var pamsWorkSummaryWorksheet = excelPackage.Workbook.Worksheets.Add(PAMSConstants.PavementWorkSummary_Tab);
            var chartRowModel = _pavementWorkSummary.Fill(pamsWorkSummaryWorksheet, reportOutputData, simulationYears, workSummaryModel, yearlyBudgetAmount, simulation.Treatments, simulation.CommittedProjects);

            checkCancelled(cancellationToken, simulationId);
            //// Pavement Work Summary By Budget TAB
            reportDetailDto.Status = $"Creating Pavement Work Summary By Budget TAB";
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            UpdateSimulationAnalysisDetail(reportDetailDto);
            var pavementWorkSummaryByBudgetWorksheet = excelPackage.Workbook.Worksheets.Add(PAMSConstants.PavementWorkSummaryByBudget_Tab);
            _pavementWorkSummaryByBudget.Fill(pavementWorkSummaryByBudgetWorksheet, reportOutputData, simulationYears, yearlyBudgetAmount, simulation.Treatments, simulation.CommittedProjects);

            checkCancelled(cancellationToken, simulationId);
            // Unfunded Pavement Projects TAB
            reportDetailDto.Status = $"Unfunded Pavement Projects TAB";
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            var _unfundedPavementProjectsWorksheet = excelPackage.Workbook.Worksheets.Add(PAMSConstants.UnfundedPavementProjects_Tab);
            _unfundedPavementProjects.Fill(_unfundedPavementProjectsWorksheet, reportOutputData);

            checkCancelled(cancellationToken, simulationId);
            // County Summary TAB
            reportDetailDto.Status = $"County Summary TAB";
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            var _countySummaryWorksheet = excelPackage.Workbook.Worksheets.Add(PAMSConstants.CountySummary_Tab);
            _countySummary.Fill(_countySummaryWorksheet, reportOutputData, simulationYears, simulation);

            checkCancelled(cancellationToken, simulationId);
            //Graph TABs
            reportDetailDto.Status = $"Creating Graph TABs";
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            UpdateSimulationAnalysisDetail(reportDetailDto);
            _addGraphsInTabs.Add(excelPackage, worksheet, pamsWorkSummaryWorksheet, chartRowModel, simulationYearsCount);

            checkCancelled(cancellationToken, simulationId);
            // Legend TAB
            reportDetailDto.Status = $"Creating Legends TAB";
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            var shortNameWorksheet = excelPackage.Workbook.Worksheets.Add(PAMSConstants.Legend_Tab);
            _summaryReportGlossary.Fill(shortNameWorksheet);
            checkCancelled(cancellationToken, simulationId);
            //check and generate folder            
            var folderPathForSimulation = $"Reports\\{simulationId}";
            if (Directory.Exists(folderPathForSimulation) == false) { Directory.CreateDirectory(folderPathForSimulation); }
            checkCancelled(cancellationToken, simulationId);
            var filePath = Path.Combine(folderPathForSimulation, "SummaryReport.xlsx");
            var bin = excelPackage.GetAsByteArray();
            File.WriteAllBytes(filePath, bin);

            //set return value
            functionReturnValue = filePath;

            reportDetailDto.Status = $"Report generation completed";
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            UpdateSimulationAnalysisDetail(reportDetailDto);

            //return value
            return functionReturnValue;
        }


        private void UpdateSimulationAnalysisDetail(SimulationReportDetailDTO dto) => _unitOfWork.SimulationReportDetailRepo.UpsertSimulationReportDetail(dto);

        private void IndicateError()
        {
            Status = "Summary output report completed with errors";
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
