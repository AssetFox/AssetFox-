﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport;

using AppliedResearchAssociates.iAM.Reporting.Hubs;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.DistrictTotals;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.Parameters;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.ShortNameGlossary;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.Visitors;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeData;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.UnfundedTreatmentFinalList;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.UnfundedTreatmentTime;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeWorkSummary;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeWorkSummaryByBudget;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.GraphTabs;
using System.Reflection;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class BAMSSummaryReport : IReport
    {
        //private readonly IHubService _hubService;
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private readonly IBridgeDataForSummaryReport _bridgeDataForSummaryReport;
        private readonly IUnfundedTreatmentFinalList _unfundedTreatmentFinalList;
        private readonly IUnfundedTreatmentTime _unfundedTreatmentTime;
        private readonly IBridgeWorkSummary _bridgeWorkSummary;
        private readonly IBridgeWorkSummaryByBudget _bridgeWorkSummaryByBudget;
        private readonly SummaryReportGlossary _summaryReportGlossary;
        private readonly SummaryReportParameters _summaryReportParameters;
        private readonly IAddGraphsInTabs _addGraphsInTabs;

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


        public BAMSSummaryReport(UnitOfDataPersistenceWork unitOfWork, string name, ReportIndexEntity results)
        {
            //store passed parameter   
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            ReportTypeName = name;

            //generate network id
            _networkId = _unitOfWork.NetworkRepo.GetMainNetwork().Id;

            //create summary report objects
            _bridgeDataForSummaryReport = new BridgeDataForSummaryReport();
            if (_bridgeDataForSummaryReport == null) { throw new ArgumentNullException(nameof(_bridgeDataForSummaryReport)); }

            _unfundedTreatmentFinalList = new UnfundedTreatmentFinalList();
            if (_unfundedTreatmentFinalList == null) { throw new ArgumentNullException(nameof(_unfundedTreatmentFinalList)); }

            _unfundedTreatmentTime = new UnfundedTreatmentTime();
            if (_unfundedTreatmentTime == null) { throw new ArgumentNullException(nameof(_unfundedTreatmentTime)); }

            _bridgeWorkSummary = new BridgeWorkSummary();
            if (_bridgeWorkSummary == null) { throw new ArgumentNullException(nameof(_bridgeWorkSummary)); }

            _bridgeWorkSummaryByBudget = new BridgeWorkSummaryByBudget();
            if (_bridgeWorkSummaryByBudget == null) { throw new ArgumentNullException(nameof(_bridgeWorkSummaryByBudget)); }

            _summaryReportGlossary = new SummaryReportGlossary();
            if (_summaryReportGlossary == null) { throw new ArgumentNullException(nameof(_summaryReportGlossary)); }

            _summaryReportParameters = new SummaryReportParameters();
            if (_summaryReportParameters == null) { throw new ArgumentNullException(nameof(_summaryReportParameters)); }
                        
            _addGraphsInTabs = new AddGraphsInTabs();
            if (_addGraphsInTabs == null) { throw new ArgumentNullException(nameof(_addGraphsInTabs)); }

            //check for existing report id
            var reportId = results?.Id; if(reportId == null) { reportId = Guid.NewGuid(); }

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
            if(string.IsNullOrEmpty(parameters) || string.IsNullOrWhiteSpace(parameters)) {
                Errors.Add("Parameters string is empty OR there are no parameters defined");
                IndicateError();
                return;
            }

            // Determine the Guid for the simulation and set simulation id
            if (!Guid.TryParse(parameters, out Guid _simulationId)) {
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

            var requiredSections = new HashSet<string>()
            {
                $"{BAMSConstants.DeckSeeded}",
                $"{BAMSConstants.SupSeeded}",
                $"{BAMSConstants.SubSeeded}",
                $"{BAMSConstants.CulvSeeded}",
                $"{BAMSConstants.DeckDurationN}",
                $"{BAMSConstants.SupDurationN}",
                $"{BAMSConstants.SubDurationN}",
                $"{BAMSConstants.CulvDurationN}"
            };

            var reportOutputData = _unitOfWork.SimulationOutputRepo.GetSimulationOutput(simulationId);

            var initialSectionValues = reportOutputData.InitialSectionSummaries[0].ValuePerNumericAttribute;

            var reportDetailDto = new SimulationReportDetailDTO { SimulationId = simulationId };

            foreach (var item in requiredSections)
            {
                if (!initialSectionValues.ContainsKey(item))
                {
                    reportDetailDto.Status = $"{item} was not found in initial section";
                    UpdateSimulationAnalysisDetail(reportDetailDto);
                    Errors.Add(reportDetailDto.Status);
                    //_hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto);
                    throw new KeyNotFoundException($"{item} was not found in initial section");
                }
            }

            var sectionValueAttribute = reportOutputData.Years[0].Sections[0].ValuePerNumericAttribute;

            foreach (var item in requiredSections)
            {
                if (!sectionValueAttribute.ContainsKey(item))
                {
                    reportDetailDto.Status = $"{item} was not found in sections";
                    UpdateSimulationAnalysisDetail(reportDetailDto);
                    Errors.Add(reportDetailDto.Status);
                    //_hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto);
                    throw new KeyNotFoundException($"{item} was not found in sections");
                }
            }

            // sorting the sections based on facility name. This is helpful throughout the report

            // generation process
            reportOutputData.InitialSectionSummaries.Sort(
                    (a, b) => Int64.Parse(a.FacilityName.Split('-')[0]).CompareTo(Int64.Parse(b.FacilityName.Split('-')[0]))
                    );

            foreach (var yearlySectionData in reportOutputData.Years)
            {
                yearlySectionData.Sections.Sort(
                    (a, b) => Int64.Parse(a.FacilityName.Split('-')[0]).CompareTo(Int64.Parse(b.FacilityName.Split('-')[0]))
                    );
            }

            var simulationYears = new List<int>();
            foreach (var item in reportOutputData.Years)
            {
                simulationYears.Add(item.Year);
            }

            var simulationYearsCount = simulationYears.Count;

            var explorer = _unitOfWork.AttributeRepo.GetExplorer();
            var network = _unitOfWork.NetworkRepo.GetSimulationAnalysisNetwork(networkId, explorer, false);
            _unitOfWork.SimulationRepo.GetSimulationInNetwork(simulationId, network);

            var simulation = network.Simulations.First();
            _unitOfWork.InvestmentPlanRepo.GetSimulationInvestmentPlan(simulation);
            _unitOfWork.AnalysisMethodRepo.GetSimulationAnalysisMethod(simulation);
            _unitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulation);
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

            // Simulation parameters TAB
            var parametersWorksheet = excelPackage.Workbook.Worksheets.Add("Parameters");
            reportDetailDto.Status = $"Creating Bridge Data TAB";
            UpdateSimulationAnalysisDetail(reportDetailDto);
            //_hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto);

            // Bridge Data TAB
            var worksheet = excelPackage.Workbook.Worksheets.Add(SummaryReportTabNames.BridgeData);
            var workSummaryModel = _bridgeDataForSummaryReport.Fill(worksheet, reportOutputData);

            // Filling up parameters tab
            _summaryReportParameters.Fill(parametersWorksheet, simulationYearsCount, workSummaryModel.ParametersModel, simulation);


            // unfunded tab will be uncommented and redone in a future release

            //// Unfunded Treatment - Final List TAB
            reportDetailDto.Status = $"Creating Unfunded Treatment - Final List TAB";
            UpdateSimulationAnalysisDetail(reportDetailDto);
            //_hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto);
            var unfundedTreatmentFinalListWorksheet = excelPackage.Workbook.Worksheets.Add("Unfunded Treatment - Final List");
            _unfundedTreatmentFinalList.Fill(unfundedTreatmentFinalListWorksheet, reportOutputData);

            //// Unfunded Treatment - Time TAB
            reportDetailDto.Status = $"Creating Unfunded Treatment - Time TAB";
            UpdateSimulationAnalysisDetail(reportDetailDto);
            //_hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto);
            var unfundedTreatmentTimeWorksheet = excelPackage.Workbook.Worksheets.Add("Unfunded Treatment - Time");
            _unfundedTreatmentTime.Fill(unfundedTreatmentTimeWorksheet, reportOutputData);

            reportDetailDto.Status = $"Creating Bridge Work Summary TAB";
            UpdateSimulationAnalysisDetail(reportDetailDto);
            //_hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto);
            // Bridge work summary TAB
            var bridgeWorkSummaryWorksheet = excelPackage.Workbook.Worksheets.Add("Bridge Work Summary");
            var chartRowModel = _bridgeWorkSummary.Fill(bridgeWorkSummaryWorksheet, reportOutputData,
                simulationYears, workSummaryModel, yearlyBudgetAmount, simulation.Treatments);

            reportDetailDto.Status = $"Creating Bridge Work Summary by Budget TAB";
            UpdateSimulationAnalysisDetail(reportDetailDto);
            //_hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto);
            // Bridge work summary by Budget TAB
            var summaryByBudgetWorksheet = excelPackage.Workbook.Worksheets.Add("Bridge Work Summary By Budget");
            _bridgeWorkSummaryByBudget.Fill(summaryByBudgetWorksheet, reportOutputData, simulationYears, yearlyBudgetAmount, simulation.Treatments);
            var districtTotalsModel = DistrictTotalsModels.DistrictTotals(reportOutputData);
            ExcelWorksheetAdder.AddWorksheet(excelPackage.Workbook, districtTotalsModel);

            reportDetailDto.Status = $"Creating Graph TABs";
            UpdateSimulationAnalysisDetail(reportDetailDto);
            //_hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto);

            _addGraphsInTabs.Add(excelPackage, worksheet, bridgeWorkSummaryWorksheet, chartRowModel, simulationYearsCount);

            // Simulation Legend TAB
            var shortNameWorksheet = excelPackage.Workbook.Worksheets.Add(SummaryReportTabNames.Legend);
            _summaryReportGlossary.Fill(shortNameWorksheet);

            //check and generate folder            
            var folderPathForSimulation = $"Reports\\{simulationId}";
            if (Directory.Exists(folderPathForSimulation) == false) { Directory.CreateDirectory(folderPathForSimulation);  }

            var filePath = Path.Combine(folderPathForSimulation, "SummaryReport.xlsx");
            var bin = excelPackage.GetAsByteArray();
            File.WriteAllBytes(filePath, bin);

            //set return value
            functionReturnValue = filePath;

            reportDetailDto.Status = $"Report generation completed";
            UpdateSimulationAnalysisDetail(reportDetailDto);
            //_hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto, true);

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
                //_hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto);

                byte[] summaryReportData = File.ReadAllBytes(filePath);
                return summaryReportData;
            }

            reportDetailDto.Status = $"Summary report is not available in the path {filePath}";
            UpdateSimulationAnalysisDetail(reportDetailDto);
            Errors.Add(reportDetailDto.Status);
            //_hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto);

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
