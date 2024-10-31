using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using OfficeOpenXml;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.DistrictCountyTotals;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.Parameters;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.ShortNameGlossary;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeData;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.UnfundedTreatmentFinalList;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.UnfundedTreatmentTime;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeWorkSummary;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeWorkSummaryByBudget;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.GraphTabs;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using BridgeCareCore.Services;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.FundedTreatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.Reporting.Services;
using System.Threading;
using AppliedResearchAssociates.iAM.Common.Logging;
using AppliedResearchAssociates.iAM.WorkQueue;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class BAMSSummaryReport : IReport
    {
        protected readonly IHubService _hubService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly BridgeDataForSummaryReport _bridgeDataForSummaryReport;
        private readonly FundedTreatmentList _fundedTreatmentList;
        private readonly UnfundedTreatmentFinalList _unfundedTreatmentFinalList;
        private readonly UnfundedTreatmentTime _unfundedTreatmentTime;
        private readonly BridgeWorkSummary _bridgeWorkSummary;
        private readonly BridgeWorkSummaryByBudget _bridgeWorkSummaryByBudget;
        private readonly SummaryReportGlossary _summaryReportGlossary;
        private readonly SummaryReportParameters _summaryReportParameters;
        private readonly AddGraphsInTabs _addGraphsInTabs;
        private readonly ReportHelper _reportHelper;

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

        public BAMSSummaryReport(IUnitOfWork unitOfWork, string name, ReportIndexDTO results, IHubService hubService)
        {
            //store passed parameter   
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
            ReportTypeName = name;
            Warnings = new List<string>();

            //create summary report objects
            _bridgeDataForSummaryReport = new BridgeDataForSummaryReport(_unitOfWork);
            if (_bridgeDataForSummaryReport == null) { throw new ArgumentNullException(nameof(_bridgeDataForSummaryReport)); }

            _fundedTreatmentList = new FundedTreatmentList(_unitOfWork);
            if (_fundedTreatmentList == null) { throw new ArgumentNullException(nameof(_fundedTreatmentList)); }

            _unfundedTreatmentFinalList = new UnfundedTreatmentFinalList(_unitOfWork);
            if (_unfundedTreatmentFinalList == null) { throw new ArgumentNullException(nameof(_unfundedTreatmentFinalList)); }

            _unfundedTreatmentTime = new UnfundedTreatmentTime(_unitOfWork);
            if (_unfundedTreatmentTime == null) { throw new ArgumentNullException(nameof(_unfundedTreatmentTime)); }
                      
            _bridgeWorkSummary = new BridgeWorkSummary(Warnings, _unitOfWork);
            if (_bridgeWorkSummary == null) { throw new ArgumentNullException(nameof(_bridgeWorkSummary)); }

            _bridgeWorkSummaryByBudget = new BridgeWorkSummaryByBudget(_unitOfWork);
            _summaryReportGlossary = new SummaryReportGlossary();
            _summaryReportParameters = new SummaryReportParameters(_unitOfWork);                        
            _addGraphsInTabs = new AddGraphsInTabs();
            _reportHelper = new ReportHelper(_unitOfWork);

            //check for existing report id
            var reportId = results?.Id; if(reportId == null) { reportId = Guid.NewGuid(); }

            //set report return default parameters
            ID = (Guid)reportId;
            Errors = new List<string>();
            if (Warnings == null) { Warnings = new List<string>(); }
            Status = "Report definition created.";
            Results = string.Empty;
            IsComplete = false;
        }

        public async Task Run(string parameters, CancellationToken? cancellationToken = null, IWorkQueueLog workQueueLog = null)
        {            
            workQueueLog ??= new DoNothingWorkQueueLog();            
            //check for the parameters string
            if (string.IsNullOrEmpty(parameters) || string.IsNullOrWhiteSpace(parameters)) {
                Errors.Add("Parameters string is empty OR there are no parameters defined");
                IndicateError();
                return;
            }

            // Determine the Guid for the simulation and set simulation id
            string simulationId = ReportHelper.GetSimulationId(parameters);
            if (!Guid.TryParse(simulationId, out Guid _simulationId)) {
                Errors.Add("Simulation ID could not be parsed to a Guid");
                IndicateError();
                return;
            }
            SimulationID = _simulationId;

            var simulationName = "";
            try
            {

                checkCancelled(cancellationToken, _simulationId);
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

            // Generate Summary report 
            var summaryReportPath = "";
            try
            {
                checkCancelled(cancellationToken, _simulationId);
                Criteria = ReportHelper.GetCriteria(parameters);
                summaryReportPath = GenerateSummaryReport(_networkId, _simulationId, workQueueLog, cancellationToken);
                if(!string.IsNullOrEmpty(Criteria) && string.IsNullOrEmpty(summaryReportPath))
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
                Errors.Add("Failed to generate summary report");
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
            var reportDetailDto = new SimulationReportDetailDTO { SimulationId = simulationId, ReportType = ReportTypeName };

            checkCancelled(cancellationToken, simulationId);
            reportDetailDto.Status = $"Generating...";

            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            UpdateSimulationAnalysisDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto, simulationId);

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

            var logger = new CallbackLogger(str => UpdateSimulationAnalysisDetailWithStatus(reportDetailDto, str));
            var reportOutputData = _unitOfWork.SimulationOutputRepo.GetSimulationOutputViaJson(simulationId);
                        
            // reportOutputData will be having all assets data, filter it based on criteria expression
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

            var initialSectionValues = reportOutputData.InitialAssetSummaries[0].ValuePerNumericAttribute;
            reportDetailDto.Status = $"Checking initial sections";

            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            foreach (var item in requiredSections)
            {
                checkCancelled(cancellationToken, simulationId);
                if (!initialSectionValues.ContainsKey(item))
                {
                    reportDetailDto.Status = $"{item} was not found in initial section";
                    UpdateSimulationAnalysisDetail(reportDetailDto);
                    _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto, simulationId);

                    Errors.Add(reportDetailDto.Status);
                    throw new KeyNotFoundException($"{item} was not found in initial section");
                }
            }

            var sectionValueAttribute = reportOutputData.Years[0].Assets[0].ValuePerNumericAttribute;
            reportDetailDto.Status = $"Checking sections";

            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);

            new QueuedWorkStatusUpdateModel() { Id = WorkQueueWorkIdFactory.CreateId(simulationId, DTOs.Enums.WorkType.ReportGeneration), Status = reportDetailDto.Status };
            foreach (var item in requiredSections)
            {
                checkCancelled(cancellationToken, simulationId);
                if (!sectionValueAttribute.ContainsKey(item))
                {
                    reportDetailDto.Status = $"{item} was not found in sections";
                    UpdateSimulationAnalysisDetail(reportDetailDto);

                    workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
                    _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto, simulationId);                   
                    Errors.Add(reportDetailDto.Status);
                    throw new KeyNotFoundException($"{item} was not found in sections");
                }
            }

            reportOutputData.InitialAssetSummaries.Sort(
                    (a, b) => _reportHelper.CheckAndGetValue<double>(a.ValuePerNumericAttribute, "BRKEY_").CompareTo(_reportHelper.CheckAndGetValue<double>(b.ValuePerNumericAttribute, "BRKEY_"))
                    );

            reportDetailDto.Status = $"Sorting yearly section data";

            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            foreach (var yearlySectionData in reportOutputData.Years)
            {

                checkCancelled(cancellationToken, simulationId);
                yearlySectionData.Assets.Sort(
                    (a, b) => _reportHelper.CheckAndGetValue<double>(a.ValuePerNumericAttribute, "BRKEY_").CompareTo(_reportHelper.CheckAndGetValue<double>(b.ValuePerNumericAttribute, "BRKEY_"))
                    );
            }

            var simulationYears = new List<int>();

            reportDetailDto.Status = $"Adding simulation years";

            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            foreach (var item in reportOutputData.Years)
            {

                checkCancelled(cancellationToken, simulationId);
                simulationYears.Add(item.Year);
            }

            var simulationYearsCount = simulationYears.Count;
            var attributeNameLookup = _unitOfWork.AttributeRepo.GetAttributeNameLookupDictionary();
            var simulationDto = _unitOfWork.SimulationRepo.GetSimulation(simulationId);            
            var investmentPlanDto = _unitOfWork.InvestmentPlanRepo.GetInvestmentPlan(simulationId);
            var budgetsDtos = _unitOfWork.BudgetRepo.GetScenarioBudgets(simulationId);
            var simpleBudgetDetailDtos = _unitOfWork.BudgetRepo.GetScenarioSimpleBudgetDetails(simulationId);
            var analysisMethodDto = _unitOfWork.AnalysisMethodRepo.GetAnalysisMethod(simulationId);
            var performanceCurvesDtos = _unitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var scenarioSelectableTreatmentsDtos = _unitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulationId);
            var committedProjectsDtos = _unitOfWork.CommittedProjectRepo.GetSectionCommittedProjectDTOs(simulationId);
            var BudgetPrioritiesDtos = _unitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(simulationId);
            var cashFlowRulesDtos = _unitOfWork.CashFlowRuleRepo.GetScenarioCashFlowRules(simulationId);            
                        
            var yearlyBudgets = new Dictionary<string, BudgetDTO>();
            reportDetailDto.Status = $"Adding yearly budget amounts";
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            foreach (var budget in budgetsDtos)
            {
                var budgetToAdd = budget;
                var budgetAmounts = budgetToAdd.BudgetAmounts.OrderBy(_ => _.Year)?.ToList();
                budgetToAdd.BudgetAmounts = budgetAmounts;
                checkCancelled(cancellationToken, simulationId);
                if (!yearlyBudgets.ContainsKey(budget.Name))
                {
                    yearlyBudgets.Add(budget.Name, budgetToAdd);
                }
                else
                {
                    yearlyBudgets[budget.Name] = budgetToAdd;
                }
            }

            //get treatment category lookup
            var treatmentCategoryLookup = new Dictionary<string, string>();
            var treatmentList = _unitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulationId);
            if (treatmentList?.Any() == true)
            {

                reportDetailDto.Status = $"Checking treatment list";

                workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
                foreach (var treatmentObject in treatmentList)
                {

                    checkCancelled(cancellationToken, simulationId);
                    if (!treatmentCategoryLookup.ContainsKey(treatmentObject.Name))
                    {
                        var treatmentCategory = SummaryReportHelper.GetCategory(treatmentObject.Category);
                        treatmentCategoryLookup.Add(treatmentObject.Name, treatmentCategory.ToString());
                    }
                }
            }

            // Pull best guess on committed project treatment categories here
            var committedProjectsForWorkOutsideScope = _unitOfWork.CommittedProjectRepo.GetCommittedProjectsForExport(simulationId);
            var committedProjectList = _unitOfWork.CommittedProjectRepo.GetCommittedProjectsForExport(simulationId);
            var treatmentsToAdd = committedProjectList.Select(_ => _.Treatment).Where(_ => !treatmentCategoryLookup.ContainsKey(_));
            foreach (var newTreatment in treatmentsToAdd)
            {
                var bestTreatmentEntry = committedProjectList.Where(_ => _.Treatment == newTreatment)
                    .GroupBy(_ => _.Category)
                    .Select(_ => new { Category = _.Key, Count = _.Count() })
                    .OrderByDescending(_ => _.Count)
                    .Select(_ => _.Category)
                    .FirstOrDefault();                
                if (!treatmentCategoryLookup.ContainsKey(newTreatment))
                {
                    bestTreatmentEntry = SummaryReportHelper.GetCategory(bestTreatmentEntry);
                    treatmentCategoryLookup.Add(newTreatment, bestTreatmentEntry.ToString());
                }
            }

            using var excelPackage = new ExcelPackage(new FileInfo("SummaryReportTestData.xlsx"));

            // Parameters TAB
            reportDetailDto.Status = $"Creating Parameters TAB";
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            UpdateSimulationAnalysisDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto, simulationId);
            // Create
            var parametersWorksheet = excelPackage.Workbook.Worksheets.Add("Parameters");
            checkCancelled(cancellationToken, simulationId);

            // Simulation Legend TAB
            var legendWorksheet = excelPackage.Workbook.Worksheets.Add(SummaryReportTabNames.Legend);
            _summaryReportGlossary.Fill(legendWorksheet);
            checkCancelled(cancellationToken, simulationId);

            // Bridge Data TAB
            reportDetailDto.Status = $"Creating Bridge Data TAB";
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            UpdateSimulationAnalysisDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto, simulationId);
            var bridgeDataWorksheet = excelPackage.Workbook.Worksheets.Add(SummaryReportTabNames.BridgeData);
            var allowFundingFromMultipleBudgets = analysisMethodDto.ShouldUseExtraFundsAcrossBudgets;
            var shouldBundleFeasibleTreatments = analysisMethodDto.ShouldAllowMultipleTreatments;
            var workSummaryModel = _bridgeDataForSummaryReport.Fill(bridgeDataWorksheet, reportOutputData, treatmentCategoryLookup, allowFundingFromMultipleBudgets, shouldBundleFeasibleTreatments, committedProjectList);
            checkCancelled(cancellationToken, simulationId);

            // Fill Simulation parameters TAB
            _summaryReportParameters.Fill(parametersWorksheet, simulationYearsCount, workSummaryModel.ParametersModel, simulationDto, analysisMethodDto, investmentPlanDto, scenarioSelectableTreatmentsDtos, committedProjectsDtos, BudgetPrioritiesDtos, cashFlowRulesDtos, budgetsDtos, reportOutputData);
            checkCancelled(cancellationToken, simulationId);            

            // Funded Treatment List TAB
            reportDetailDto.Status = $"Creating Funded Treatment List TAB";
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            UpdateSimulationAnalysisDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto, simulationId);
            var fundedTreatmentWorksheet = excelPackage.Workbook.Worksheets.Add("Funded Treatment List");
            _fundedTreatmentList.Fill(fundedTreatmentWorksheet, reportOutputData, shouldBundleFeasibleTreatments);
            checkCancelled(cancellationToken, simulationId);

            // Unfunded Treatment - Final List TAB
            reportDetailDto.Status = $"Creating Unfunded Treatment - Final List TAB";
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            UpdateSimulationAnalysisDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto, simulationId);
            var unfundedTreatmentFinalListWorksheet = excelPackage.Workbook.Worksheets.Add("Unfunded Treatment - Final List");
            _unfundedTreatmentFinalList.Fill(unfundedTreatmentFinalListWorksheet, reportOutputData);
            checkCancelled(cancellationToken, simulationId);

            // Unfunded Treatment - Time TAB
            reportDetailDto.Status = $"Creating Unfunded Treatment - Time TAB";
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            UpdateSimulationAnalysisDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto, simulationId);
            var unfundedTreatmentTimeWorksheet = excelPackage.Workbook.Worksheets.Add("Unfunded Treatment - Time");
            _unfundedTreatmentTime.Fill(unfundedTreatmentTimeWorksheet, reportOutputData);
            checkCancelled(cancellationToken, simulationId);

            // Bridge work summary TAB
            reportDetailDto.Status = $"Creating Bridge Work Summary TAB";
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            UpdateSimulationAnalysisDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto, simulationId);
            var bridgeWorkSummaryWorksheet = excelPackage.Workbook.Worksheets.Add("Bridge Work Summary");
            var chartRowModel = _bridgeWorkSummary.Fill(bridgeWorkSummaryWorksheet, reportOutputData, simulationYears, workSummaryModel, yearlyBudgets, scenarioSelectableTreatmentsDtos, treatmentCategoryLookup, committedProjectsForWorkOutsideScope, shouldBundleFeasibleTreatments);
            checkCancelled(cancellationToken, simulationId);

            // Bridge work summary by Budget TAB
            reportDetailDto.Status = $"Creating Bridge Work Summary by Budget TAB";
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            UpdateSimulationAnalysisDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto, simulationId);
            var summaryByBudgetWorksheet = excelPackage.Workbook.Worksheets.Add("Bridge Work Summary By Budget");
            _bridgeWorkSummaryByBudget.Fill(summaryByBudgetWorksheet, reportOutputData, simulationYears, yearlyBudgets, scenarioSelectableTreatmentsDtos, treatmentCategoryLookup, committedProjectList, committedProjectsForWorkOutsideScope, shouldBundleFeasibleTreatments, simpleBudgetDetailDtos);
            checkCancelled(cancellationToken, simulationId);

            // District County Totals TAB
            reportDetailDto.Status = $"Creating District County Totals TAB";
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            UpdateSimulationAnalysisDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto, simulationId);
            var districtCountyTotalsModel = DistrictTotalsModels.DistrictTotals(reportOutputData);
            ExcelWorksheetAdder.AddWorksheet(excelPackage.Workbook, districtCountyTotalsModel);
            checkCancelled(cancellationToken, simulationId);

            // Graph tabs
            reportDetailDto.Status = $"Creating Graph TABs";
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            UpdateSimulationAnalysisDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto, simulationId);
            _addGraphsInTabs.Add(excelPackage, bridgeDataWorksheet, bridgeWorkSummaryWorksheet, chartRowModel, simulationYearsCount);
            checkCancelled(cancellationToken, simulationId);

            //check and generate folder
            var folderPathForSimulation = $"Reports\\{simulationId}";
            Directory.CreateDirectory(folderPathForSimulation);
            var filePath = Path.Combine(folderPathForSimulation, "SummaryReport.xlsx");
            checkCancelled(cancellationToken, simulationId);
            var bin = excelPackage.GetAsByteArray();
            File.WriteAllBytes(filePath, bin);

            //set return value
            functionReturnValue = filePath;

            reportDetailDto.Status = $"Report generation completed";
            workQueueLog.UpdateWorkQueueStatus(reportDetailDto.Status);
            UpdateSimulationAnalysisDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, reportDetailDto, simulationId);

            //return value
            return functionReturnValue;
        }        

        private void UpdateSimulationAnalysisDetail(SimulationReportDetailDTO dto) => _unitOfWork.SimulationReportDetailRepo.UpsertSimulationReportDetail(dto);

        private void UpdateSimulationAnalysisDetailWithStatus(SimulationReportDetailDTO dto, string message)
        {
            dto.Status = message;
            UpdateSimulationAnalysisDetail(dto);
            _hubService.SendRealTimeMessage(_unitOfWork.CurrentUser?.Username, HubConstant.BroadcastReportGenerationStatus, dto.Status, dto.SimulationId);
        }

        private void IndicateError(string status = null)
        {
            Status = status ?? "Summary output report completed with errors";
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
                Status = Status,
                ReportType = ReportTypeName
            };
            UpsertSimulationReportDetail(reportDetailDto);
        }
    }
}
