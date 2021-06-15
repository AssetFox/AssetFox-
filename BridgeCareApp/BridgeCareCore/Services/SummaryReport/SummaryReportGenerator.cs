using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Services.SummaryReport.DistrictTotals;
using BridgeCareCore.Services.SummaryReport.Parameters;
using BridgeCareCore.Services.SummaryReport.ShortNameGlossary;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport
{
    public class SummaryReportGenerator : ISummaryReportGenerator
    {
        private readonly ILogger<SummaryReportGenerator> _logger;
        private readonly IBridgeDataForSummaryReport _bridgeDataForSummaryReport;
        private readonly IUnfundedRecommendations _unfundedRecommendations;
        private readonly IBridgeWorkSummary _bridgeWorkSummary;
        private readonly IBridgeWorkSummaryByBudget _bridgeWorkSummaryByBudget;
        private readonly SummaryReportGlossary _summaryReportGlossary;
        private readonly SummaryReportParameters _summaryReportParameters;
        private readonly IHubService _hubService;
        private readonly IAddGraphsInTabs _addGraphsInTabs;
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public SummaryReportGenerator(IBridgeDataForSummaryReport bridgeDataForSummaryReport,
            ILogger<SummaryReportGenerator> logger,
            IUnfundedRecommendations unfundedRecommendations,
            IBridgeWorkSummary bridgeWorkSummary, IBridgeWorkSummaryByBudget workSummaryByBudget,
            SummaryReportGlossary summaryReportGlossary, SummaryReportParameters summaryReportParameters,
            IHubService hubService,
            IAddGraphsInTabs addGraphsInTabs,
            UnitOfDataPersistenceWork unitOfWork)
        {
            _bridgeDataForSummaryReport = bridgeDataForSummaryReport ?? throw new ArgumentNullException(nameof(bridgeDataForSummaryReport));
            _unfundedRecommendations = unfundedRecommendations ?? throw new ArgumentNullException(nameof(unfundedRecommendations));
            _bridgeWorkSummary = bridgeWorkSummary ?? throw new ArgumentNullException(nameof(bridgeWorkSummary));
            _bridgeWorkSummaryByBudget = workSummaryByBudget ?? throw new ArgumentNullException(nameof(workSummaryByBudget));
            _summaryReportGlossary = summaryReportGlossary ?? throw new ArgumentNullException(nameof(summaryReportGlossary));
            _summaryReportParameters = summaryReportParameters ?? throw new ArgumentNullException(nameof(summaryReportParameters));
            _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
            _addGraphsInTabs = addGraphsInTabs ?? throw new ArgumentNullException(nameof(addGraphsInTabs));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public byte[] GenerateReport(Guid networkId, Guid simulationId)
        {
            var requiredSections = new HashSet<string>()
            {
                $"{Properties.Resources.DeckSeeded}",
                $"{Properties.Resources.SupSeeded}",
                $"{Properties.Resources.SubSeeded}",
                $"{Properties.Resources.CulvSeeded}",
                $"{Properties.Resources.DeckDurationN}",
                $"{Properties.Resources.SupDurationN}",
                $"{Properties.Resources.SubDurationN}",
                $"{Properties.Resources.CulvDurationN}"
            };

            var reportOutputData = _unitOfWork.SimulationOutputRepo.GetSimulationOutput(simulationId);

            var initialSectionValues = reportOutputData.InitialSectionSummaries[0].ValuePerNumericAttribute;

            var reportDetailDto = new SimulationReportDetailDTO {SimulationId = simulationId};

            foreach (var item in requiredSections)
            {
                if (!initialSectionValues.ContainsKey(item))
                {
                    reportDetailDto.Status = $"{item} was not found in initial section";
                    UpdateSimulationAnalysisDetail(reportDetailDto);
                    _hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto);
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
                    _hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto);
                    throw new KeyNotFoundException($"{item} was not found in sections");
                }
            }

            // sorting the sections based on facility name. This is helpful throughout the report
            // generation process
            reportOutputData.InitialSectionSummaries.Sort(
                    (a, b) => int.Parse(a.FacilityName).CompareTo(int.Parse(b.FacilityName))
                    );

            foreach (var yearlySectionData in reportOutputData.Years)
            {
                yearlySectionData.Sections.Sort(
                    (a, b) => int.Parse(a.FacilityName).CompareTo(int.Parse(b.FacilityName))
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
            _unitOfWork.PerformanceCurveRepo.SimulationPerformanceCurves(simulation);
            _unitOfWork.SelectableTreatmentRepo.GetSimulationTreatments(simulation);

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
            _hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto);

            // Bridge Data TAB
            var worksheet = excelPackage.Workbook.Worksheets.Add("Bridge Data");
            var workSummaryModel = _bridgeDataForSummaryReport.Fill(worksheet, reportOutputData);

            // Filling up parameters tab
            _summaryReportParameters.Fill(parametersWorksheet, simulationYearsCount, workSummaryModel.ParametersModel, simulation);
            reportDetailDto.Status = $"Creating Unfunded Recommendations TAB";
            UpdateSimulationAnalysisDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto);
            // Unfunded Recommendations TAB
            var unfundedRecommendationWorksheet = excelPackage.Workbook.Worksheets.Add("Unfunded Recommendations");
            _unfundedRecommendations.Fill(unfundedRecommendationWorksheet, reportOutputData);

            // Simulation Legend TAB
            var shortNameWorksheet = excelPackage.Workbook.Worksheets.Add("Legend");
            _summaryReportGlossary.Fill(shortNameWorksheet);
            reportDetailDto.Status = $"Creating Bridge Work Summary TAB";
            UpdateSimulationAnalysisDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto);
            // Bridge work summary TAB
            var bridgeWorkSummaryWorksheet = excelPackage.Workbook.Worksheets.Add("Bridge Work Summary");
            var chartRowModel = _bridgeWorkSummary.Fill(bridgeWorkSummaryWorksheet, reportOutputData,
                simulationYears, workSummaryModel, yearlyBudgetAmount);
            reportDetailDto.Status = $"Creating Bridge Work Summary by Budget TAB";
            UpdateSimulationAnalysisDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto);
            // Bridge work summary by Budget TAB
            var summaryByBudgetWorksheet = excelPackage.Workbook.Worksheets.Add("Bridge Work Summary By Budget");
            _bridgeWorkSummaryByBudget.Fill(summaryByBudgetWorksheet, reportOutputData, simulationYears, yearlyBudgetAmount);
            var writer = new ExcelWriter();
            var districtTotalsModel = DistrictTotalsModels.DistrictTotals;
            writer.AddWorksheet(excelPackage.Workbook, districtTotalsModel);
            reportDetailDto.Status = $"Creating Graph TABs";
            UpdateSimulationAnalysisDetail(reportDetailDto);
            _hubService.SendRealTimeMessage(_unitOfWork.UserEntity?.Username, HubConstant.BroadcastSummaryReportGenerationStatus, reportDetailDto);

            _addGraphsInTabs.Add(excelPackage, worksheet, bridgeWorkSummaryWorksheet, chartRowModel, simulationYearsCount);

            var folderPathForSimulation = $"DownloadedNewReports\\{simulationId}";
            var relativeFolderPath = Path.Combine(Environment.CurrentDirectory, folderPathForSimulation);
            Directory.CreateDirectory(relativeFolderPath);
            var filePath = Path.Combine(Environment.CurrentDirectory, folderPathForSimulation, "SummaryReportTestData.xlsx");
            var bin = excelPackage.GetAsByteArray();
            File.WriteAllBytes(filePath, bin);

            return bin;
        }

        private void UpdateSimulationAnalysisDetail(SimulationReportDetailDTO dto) =>
            _unitOfWork.SimulationReportDetailRepo.UpsertSimulationReportDetail(dto);
    }
}
